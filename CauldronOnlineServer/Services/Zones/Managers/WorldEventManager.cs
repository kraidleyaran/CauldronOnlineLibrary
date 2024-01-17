using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CauldronOnlineCommon;
using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Traits;
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

        private void DroppedItemCreated(WorldObject obj, PlayerDroppedItemEvent itemEvent)
        {
            obj.AddTrait(new PlayerDroppedItemTrait(itemEvent.Item, itemEvent.Stack));
            itemEvent.ObjectId = obj.Data.Id;
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<UpdateCurrentZoneTickMessage>(UpdateCurrentZoneTick, _zoneId);
            this.SubscribeWithFilter<ZoneEventProcessTickMessage>(ZoneEventProcessTick, _zoneId);
            this.SubscribeWithFilter<ZonePlayerUpdateTickMessage>(ZonePlayerUpdateTick, _zoneId);
            this.SubscribeWithFilter<PlayerEnteredWorldMessage>(PlayerEnteredWorld, ZoneService.GLOBAL_ZONE_FILTER);
            this.SubscribeWithFilter<PlayerLeftWorldMessage>(PlayerLeftWorld, ZoneService.GLOBAL_ZONE_FILTER);
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
                                        if (zone.ObjectManager.TryGetObjectById(movement.Id, out var obj))
                                        {
                                            obj.SetPosition(movement.Position);
                                            obj.Data.Direction = movement.Direction;
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
                                            obj.SendMessageTo(new SetDoorStateMessage{Open = true, IsEvent = true}, obj);
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
                                case ChestOpenEvent.ID:
                                    if (worldEvent is ChestOpenEvent chest)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(chest.TargetId, out var obj))
                                        {
                                            obj.SendMessageTo(new OpenChestMessage{Player = chest.PlayerName},  obj);
                                        }
                                    }
                                    break;
                                case TeleportEvent.ID:
                                    if (worldEvent is TeleportEvent teleport)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(teleport.ObjectId, out var obj))
                                        {
                                            obj.SetPosition(teleport.Position);
                                        }
                                    }
                                    break;
                                case MovableEvent.ID:
                                    if (worldEvent is MovableEvent movable)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(movable.OwnerId, out var player) && zone.ObjectManager.TryGetObjectById(movable.MovableId, out var movableObj))
                                        {

                                            switch (movable.Type)
                                            {
                                                case MovableType.Grab:
                                                    var ownerId = string.Empty;
                                                    var queryOwnerIdMsg = new QueryOwnerIdMessage();
                                                    queryOwnerIdMsg.DoAfter = id => ownerId = id;
                                                    this.SendMessageTo(queryOwnerIdMsg, movableObj);

                                                    if (string.IsNullOrEmpty(ownerId))
                                                    {
                                                        player.SendMessageTo(new SetOwnerIdMessage { Id = movable.OwnerId }, movableObj);
                                                    }
                                                    break;
                                                case MovableType.Release:
                                                    player.SendMessageTo(new RemoveOwnerIdMessage { Id = movable.OwnerId }, movableObj);
                                                    break;
                                            }
                                            player.SetPosition(movable.OwnerPosition);
                                            movableObj.SetPosition(movable.MovablePosition);

                                        }
                                    }
                                    break;
                                case RollEvent.ID:
                                    if (worldEvent is RollEvent roll)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(roll.OwnerId, out var obj))
                                        {
                                            obj.SetPosition(roll.Position);
                                        }
                                    }
                                    break;
                                case PlayerEnteredWorldEvent.ID:
                                    this.SendMessageWithFilter(PlayerEnteredWorldMessage.INSTANCE, _zoneId);
                                    break;
                                case PlayerLeftWorldEvent.ID:
                                    this.SendMessageWithFilter(PlayerLeftWorldMessage.INSTANCE, _zoneId);
                                    break;
                                case UpdateCombatStatsEvent.ID:
                                    if (worldEvent is UpdateCombatStatsEvent updateCombatStats)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(updateCombatStats.OwnerId, out var obj))
                                        {
                                            obj.SendMessageTo(new SetCombatStatsMessage{Stats = updateCombatStats.Stats, Secondary = updateCombatStats.Secondary, Vitals = updateCombatStats.Vitals}, obj);
                                        }
                                    }
                                    break;
                                case PlayerDroppedItemEvent.ID:
                                    if (worldEvent is PlayerDroppedItemEvent droppedItem)
                                    {
                                        zone.ObjectManager.RequestObject(droppedItem.Item, new string[0], false, new ObjectParameter[0], droppedItem.Position, false, obj => DroppedItemCreated(obj, droppedItem), true, true);
                                    }
                                    break;
                                case PlayerClaimItemEvent.ID:
                                    if (worldEvent is PlayerClaimItemEvent playerClaimItem)
                                    {
                                        if (zone.ObjectManager.TryGetObjectById(playerClaimItem.OwnerId, out var playerObj))
                                        {
                                            if (zone.ObjectManager.TryGetObjectById(playerClaimItem.ObjectId, out var itemObj))
                                            {
                                                playerObj.SendMessageTo(new ClaimItemMessage { Owner = playerObj }, itemObj);
                                            }
                                            else
                                            {
                                                playerObj.SendMessageTo(new ClientClaimItemResultMessage{Success = false, ObjectId = playerClaimItem.ObjectId}, playerObj);
                                            }
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

        private void PlayerEnteredWorld(PlayerEnteredWorldMessage msg)
        {
            _worldEvents.Enqueue(new PlayerEnteredWorldEvent{Tick = _currentTick, Order = _processedEvents.Count + _worldEvents.Count});
        }


        private void PlayerLeftWorld(PlayerLeftWorldMessage msg)
        {
            _worldEvents.Enqueue(new PlayerLeftWorldEvent { Tick = _currentTick, Order = _processedEvents.Count + _worldEvents.Count });
        }

        public override void Destroy()
        {
            _worldEvents = new ConcurrentQueue<WorldEvent>();
            _processedEvents = new ConcurrentQueue<WorldEvent>();
            _currentTick = new WorldTick();
            _playerWorldEventsUpdateMsg = null;
            _zoneId = string.Empty;
            base.Destroy();
        }
    }
}