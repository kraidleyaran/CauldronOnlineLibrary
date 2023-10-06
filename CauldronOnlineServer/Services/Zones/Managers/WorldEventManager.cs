using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CauldronOnlineCommon;
using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.WorldEvents;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Zones.Managers
{
    public class WorldEventManager : WorldManager
    {
        private string _zoneId = string.Empty;

        private WorldTick _currentTick;

        private ConcurrentQueue<WorldEvent> _worldEvents = new ConcurrentQueue<WorldEvent>();
        private ConcurrentQueue<WorldEvent> _processedEvents = new ConcurrentQueue<WorldEvent>();
        private PlayerWorldEventsUpdateMessage _playerWorldEventsUpdateMsg = new PlayerWorldEventsUpdateMessage();

        public WorldEventManager(string zoneId)
        {
            _zoneId = zoneId;
            SubscribeToMessages();
        }

        public void RegisterEvent<T>(T worldEvent, bool playerEvent = false) where T : WorldEvent
        {
            worldEvent.Order = _processedEvents.Count + _worldEvents.Count;
            worldEvent.Tick = playerEvent ? worldEvent.Tick : _currentTick;
            if (playerEvent)
            {
                _worldEvents.Enqueue(worldEvent);
            }
            else
            {
                _processedEvents.Enqueue(worldEvent);
            }
            
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<UpdateCurrentZoneTickMessage>(UpdateCurrentZoneTick, _zoneId);
            this.SubscribeWithFilter<ZoneEventProcessTickMessage>(ZoneEventProcessTick, _zoneId);
            this.SubscribeWithFilter<ZonePlayerUpdateTickMessage>(ZonePlayerUpdateTick, _zoneId);
        }

        private void UpdateCurrentZoneTick(UpdateCurrentZoneTickMessage msg)
        {
            _currentTick = msg.Tick;
        }

        private void ZoneEventProcessTick(ZoneEventProcessTickMessage msg)
        {
            if (_worldEvents.Count > 0)
            {
                var zone = ZoneService.GetZoneById(_zoneId);
                var processEvents = new List<WorldEvent>();
                if (zone != null && _worldEvents.Count > 0)
                {
                    while (_worldEvents.TryDequeue(out var worldEvent))
                    {
                        processEvents.Add(worldEvent);
                    }

                    if (processEvents.Count > 0)
                    {
                        var orderedEvents = processEvents.OrderBy(e => e.Tick).ThenBy(e => e.Order).ToArray();
                        foreach(var worldEvent in orderedEvents)
                        {
                            switch (worldEvent.EventId)
                            {
                                case MovementEvent.ID:
                                    if (worldEvent is MovementEvent movement)
                                    {
                                        if (zone.IsValidPosition(movement.Position) && zone.ObjectManager.TryGetObjectById(movement.Id, out var obj))
                                        {
                                            obj.SetPosition(movement.Position);
                                        }
                                    }
                                    break;
                                case DamageEvent.ID:
                                    if (worldEvent is DamageEvent damage)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(damage.TargetId, out var obj))
                                        {
                                            this.SendMessageTo(new TakeDamageMessage{Amount = damage.Amount, OwnerId = damage.OwnerId}, obj);
                                        }
                                    }
                                    break;
                                case KnockbackEvent.ID:
                                    if (worldEvent is KnockbackEvent knockback)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(knockback.TargetId, out var obj))
                                        {
                                            this.SendMessageTo(new ApplyKnockbackMessage{Position = knockback.EndPosition, Time = knockback.Time}, obj);
                                        }
                                    }
                                    break;
                                case ObjectDeathEvent.ID:
                                    if (worldEvent is ObjectDeathEvent death)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(death.Id, out var obj))
                                        {
                                            this.SendMessageTo(new ObjectDeathMessage{OwnerId = death.OwnerId}, obj);
                                        }
                                    }
                                    break;
                                case DestroyObjectEvent.ID:
                                    if (worldEvent is DestroyObjectEvent destroy)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(destroy.ObjectId, out var obj))
                                        {
                                            obj.SetObjectState(WorldObjectState.Destroying);
                                            zone.ObjectManager.RequestDestroyObject(destroy.ObjectId);
                                        }
                                    }
                                    break;
                                case RespawnEvent.ID:
                                    if (worldEvent is RespawnEvent respawn)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(respawn.OwnerId, out var obj))
                                        {
                                            obj.SetPosition(respawn.Position);
                                            obj.SetObjectState(WorldObjectState.Active);
                                        }
                                    }
                                    break;
                                case HealEvent.ID:
                                    if (worldEvent is HealEvent heal)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(heal.TargetId, out var obj))
                                        {
                                            obj.SendMessageTo(new HealMessage{Amount = heal.Amount, IsEvent = true, OwnerId = heal.OwnerId}, obj);
                                        }
                                    }
                                    break;
                                case DoorEvent.ID:
                                    if (worldEvent is DoorEvent door)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(door.TargetId, out var obj))
                                        {
                                            obj.SendMessageTo(new SetDoorStateMessage{Open = true}, obj);
                                        }
                                    }
                                    break;
                                case SwitchSignalEvent.ID:
                                    if (worldEvent is SwitchSignalEvent switchSignal)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(switchSignal.TargetId, out var obj))
                                        {
                                            obj.SendMessageTo(new SetSwitchSignalMessage{Signal = switchSignal.Signal, IsEvent = true}, obj);
                                        }
                                    }
                                    break;
                                case DoorCheckEvent.ID:
                                    if (worldEvent is DoorCheckEvent doorCheck)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(doorCheck.TargetId, out var obj))
                                        {
                                            obj.SendMessageTo(DoorCheckMessage.INSTANCE, obj);
                                        }
                                    }
                                    break;
                                case ChestEvent.ID:
                                    if (worldEvent is ChestEvent chest)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(chest.TargetId, out var obj))
                                        {
                                            obj.SendMessageTo(OpenChestMessage.INSTANCE, obj);
                                        }
                                    }
                                    break;
                            }
                            _processedEvents.Enqueue(worldEvent);
                        }
                    }
                }
            }
        }

        private void ZonePlayerUpdateTick(ZonePlayerUpdateTickMessage msg)
        {
            if (_processedEvents.Count > 0)
            {
                var data = new List<byte[]>();
                while (_processedEvents.TryDequeue(out var worldEvent))
                {
                    data.Add(worldEvent.ToData());
                }
                _playerWorldEventsUpdateMsg.Message = new ClientWorldEventsUpdateMessage { Events = data.ToArray() }.ToByteArray();
                this.SendMessageWithFilter(_playerWorldEventsUpdateMsg, _zoneId);
            }

        }

    }
}