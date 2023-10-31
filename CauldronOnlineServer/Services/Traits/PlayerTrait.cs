using System.Collections.Generic;
using CauldronOnlineCommon;
using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Requests;
using CauldronOnlineServer.Services.Items;
using CauldronOnlineServer.Services.SystemEvents;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class PlayerTrait : WorldTrait
    {
        public const string NAME = "Player";

        private int _connectionId = -1;
        private string _worldId = string.Empty;

        private bool _disconnect = false;

        private ClientZoneTickMessage _clientZoneTickMsg = new ClientZoneTickMessage();

        private List<string> _aggrodMonsters = new List<string>();
        private PlayerParameter _playerParameter = new PlayerParameter();
        private string _movableId = string.Empty;

        public PlayerTrait(int connectionId, string worldId, SpriteColorData colors)
        {
            Name = NAME;
            _connectionId = connectionId;
            _worldId = worldId;
            _playerParameter.Colors = colors;
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_playerParameter);
            SubscribeToMessages();
        }

        private void OnAggroRequestCompleted(bool removed, string parentId)
        {
            if (!removed && !_aggrodMonsters.Contains(parentId))
            {
                _aggrodMonsters.Add(parentId);
            }
        }

        private void OnZoneTransferComplete(string objId, string playerName, string zone, WorldVector2Int pos)
        {
            WorldServer.SendToClient(new ClientZoneTransferResultMessage{Success = true, ObjectId = objId, Zone = zone, Position = pos}, _connectionId);
            this.UnsubscribeFromAllMessagesWithFilter(_worldId);
            var currentZone = ZoneService.GetZoneById(_parent.ZoneId);
            if (currentZone != null)
            {
                currentZone.EventManager.RegisterEvent(new DestroyObjectEvent{ObjectId = _parent.Data.Id, OwnerId = _parent.Data.Id});
                currentZone.ObjectManager.RequestDestroyObject(_parent.Data.Id, _worldId);
            }
        }

        private void OnRespawnInDifferentZoneComplete(string objId, string playerName, string zone, WorldVector2Int pos)
        {
            WorldServer.SendToClient(new ClientRespawnResultMessage { Success = true, ObjectId = objId, Zone = zone, Position = pos }, _connectionId);
            this.UnsubscribeFromAllMessagesWithFilter(_worldId);
            var currentZone = ZoneService.GetZoneById(_parent.ZoneId);
            if (currentZone != null)
            {
                currentZone.EventManager.RegisterEvent(new DestroyObjectEvent { ObjectId = _parent.Data.Id, OwnerId = _parent.Data.Id });
                currentZone.ObjectManager.RequestDestroyObject(_parent.Data.Id, _worldId);
            }
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<ZoneUpdateTickMessage>(ZoneUpdateTick, _parent.ZoneId);
            this.SubscribeWithFilter<PlayerWorldEventsUpdateMessage>(PlayerWorldEventsUpdate, _parent.ZoneId);

            this.SubscribeWithFilter<ClientObjectRequestMessage>(ClientObjectRequest, _worldId);
            this.SubscribeWithFilter<PlayerDisconnectedMessage>(PlayerDisconnected, _worldId);
            this.SubscribeWithFilter<ClientAggroMessage>(ClientAggro, _worldId);
            this.SubscribeWithFilter<ClientDamageMessage>(ClientDamage, _worldId);
            this.SubscribeWithFilter<ClientKnockbackMessage>(ClientKnockback, _worldId);
            this.SubscribeWithFilter<ClientUseAbilityMessage>(ClientUseAbility, _worldId);
            this.SubscribeWithFilter<ClientDeathMessage>(ClientDeath, _worldId);
            this.SubscribeWithFilter<ClientShootProjectileMessage>(ClientShootProjectile, _worldId);
            this.SubscribeWithFilter<ClientDestroyObjectMessage>(ClientDestroyObject, _worldId);
            this.SubscribeWithFilter<ClientZoneTransferRequestMessage>(ClientZoneTransferRequest, _worldId);
            this.SubscribeWithFilter<ClientRespawnRequestMessage>(ClientRespawnRequest, _worldId);
            this.SubscribeWithFilter<RemoveFromAggroMessage>(RemovedFromAggro, _worldId);
            this.SubscribeWithFilter<ClientOpenDoorMessage>(ClientOpenDoor, _worldId);
            this.SubscribeWithFilter<ClientTriggerEventMessage>(ClientTriggerEvent, _worldId);
            this.SubscribeWithFilter<ClientHealMessage>(ClientHeal, _worldId);
            this.SubscribeWithFilter<ClientSwitchSignalMessage>(ClientSwitchSignal, _worldId);
            this.SubscribeWithFilter<ClientAddKeyItemMessage>(ClientAddKeyItem, _worldId);
            this.SubscribeWithFilter<ClientDoorCheckMessage>(ClientDoorCheck, _worldId);
            this.SubscribeWithFilter<ClientOpenChestMessage>(ClientOpenChest, _worldId);
            this.SubscribeWithFilter<ClientTeleportMessage>(ClientTeleport, _worldId);
            this.SubscribeWithFilter<ClientProjectileMovementUpdateMessage>(ClientProjectileMovementUpdate, _worldId);
            this.SubscribeWithFilter<ClientPlayerLeveledMessage>(ClientPlayerLeveled, _worldId);
            this.SubscribeWithFilter<ClientMovableUpdateMessage>(ClientMovableUpdate, _worldId);
            this.SubscribeWithFilter<ClientRollUpdateMessage>(ClientRollUpdate, _worldId);
            
            _parent.SubscribeWithFilter<ApplyExperienceMessage>(ApplyExperience, _id);
        }

        private void PlayerWorldEventsUpdate(PlayerWorldEventsUpdateMessage msg)
        {
            if (!_disconnect)
            {
                WorldServer.SendToClient(msg.Message, _connectionId);
            }
        }

        private void ClientObjectRequest(ClientObjectRequestMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                var objs = zone.ObjectManager.GetObjectData();
                var multiParts = new ClientObjectResultMessage {Data = objs, Success = true}.ToByteArray().ToMultiPart(CauldronUtils.MAX_MESSAGE_SIZE, ClientObjectResultMessage.ID);
                foreach (var part in multiParts)
                {
                    WorldServer.SendToClient(part, _connectionId);
                }
                
            }
            else
            {
                WorldServer.SendToClient(new ClientObjectResultMessage{Success = false, Message = "Unable to find zone"}, _connectionId);
            }
        }

        private void ZoneUpdateTick(ZoneUpdateTickMessage msg)
        {
            if (!_disconnect)
            {
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    _clientZoneTickMsg.Tick = zone.Tick.CurrenTick;
                    WorldServer.SendToClient(_clientZoneTickMsg, _connectionId);
                }
            }
            
        }

        private void PlayerDisconnected(PlayerDisconnectedMessage msg)
        {
            if (!_disconnect)
            {
                _disconnect = true;

                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    if (!string.IsNullOrEmpty(_movableId))
                    {
                        if (zone.ObjectManager.TryGetObjectById(_movableId, out var movableObj))
                        {
                            zone.EventManager.RegisterEvent(new MovableEvent
                            {
                                MovableId = _movableId,
                                MovablePosition = movableObj.Data.Position,
                                OwnerId = _parent.Data.Id,
                                OwnerPosition = _parent.Data.Position,
                                Type = MovableType.Release,
                                Tick = zone.Tick.CurrenTick
                            });
                        }
                    }
                    SystemEventService.SendMessage($"{_parent.Data.DisplayName} has left the world", SystemEventType.Leave);
                    zone.EventManager.RegisterEvent(new DestroyObjectEvent { ObjectId = _parent.Data.Id, OwnerId = _parent.Data.Id });
                    zone.ObjectManager.RequestDestroyObject(_parent.Data.Id, _worldId);   
                }
            }   
        }



        private void ClientAggro(ClientAggroMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                if (zone.ObjectManager.TryGetObjectById(msg.AggrodObjectId, out var obj))
                {
                    this.SendMessageTo(new AggroRequestMessage{Request = new AggroRequest(_parent.Data.Id, OnAggroRequestCompleted, msg.Remove)}, obj);
                }
            }
        }

        private void ClientDamage(ClientDamageMessage msg)
        {
            if (!string.IsNullOrEmpty(msg.TargetId) && !string.IsNullOrEmpty(msg.OwnerId))
            {
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    var targetId = msg.TargetId;
                    if (targetId == _parent.Data.Id || !zone.ObjectManager.IsPlayer(targetId) && _parent.Data.Id == msg.OwnerId)
                    {
                        zone.EventManager.RegisterEvent(new DamageEvent{OwnerId = msg.OwnerId, TargetId = targetId, Amount = msg.Amount, Tick = msg.Tick, Type = msg.Type}, true);
                    }
                }
            }
        }

        private void ClientKnockback(ClientKnockbackMessage msg)
        {
            if (!string.IsNullOrEmpty(msg.TargetId) && !string.IsNullOrEmpty(msg.OwnerId))
            {
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    var targetId = msg.TargetId;
                    if (targetId == _parent.Data.Id || !zone.ObjectManager.IsPlayer(targetId) && _parent.Data.Id == msg.OwnerId)
                    {
                        zone.EventManager.RegisterEvent(new KnockbackEvent { OwnerId = msg.OwnerId, TargetId = targetId, EndPosition = msg.EndPosition, Time = msg.Time, Tick = msg.Tick}, true);
                    }
                }
            }
        }

        private void ClientUseAbility(ClientUseAbilityMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.EventManager.RegisterEvent(new AbilityEvent{OwnerId = _parent.Data.Id, Ability = msg.Ability, Direction = msg.Direction, Position = msg.Position, Ids = msg.Ids, Tick = msg.Tick});
            }
        }

        private void ClientDeath(ClientDeathMessage msg)
        {
            if (!string.IsNullOrEmpty(msg.Target))
            {
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    if (msg.Target == _parent.Data.Id || !zone.ObjectManager.IsPlayer(msg.Target))
                    {
                        if (msg.Target == _parent.Data.Id)
                        {
                            SystemEventService.SendMessage($"{_parent.Data.DisplayName} has died", SystemEventType.Death);
                        }
                        zone.EventManager.RegisterEvent(new ObjectDeathEvent{Id = msg.Target, OwnerId = _parent.Data.Id}, true);
                    }
                }
            }
            
        }

        private void ClientShootProjectile(ClientShootProjectileMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.EventManager.RegisterEvent(new ShootProjectileEvent{Direction = msg.Direction, Position = msg.Position, Projectile = msg.Projectile, Tick = msg.Tick});
            }
        }

        private void ClientDestroyObject(ClientDestroyObjectMessage msg)
        {
            if (!string.IsNullOrEmpty(msg.TargetId))
            {
                var zone = ZoneService.GetZoneByName(_parent.ZoneId);
                if (zone != null)
                {
                    if (!zone.ObjectManager.IsPlayer(msg.TargetId))
                    {
                        zone.EventManager.RegisterEvent(new DestroyObjectEvent{ObjectId = msg.TargetId, Tick = msg.Tick, OwnerId = _parent.Data.Id}, true);
                    }
                }
            }
        }

        private void ClientZoneTransferRequest(ClientZoneTransferRequestMessage msg)
        {
            if (_parent.State != WorldObjectState.Transferring)
            {
                var zone = ZoneService.GetZoneByName(msg.Zone);
                if (zone != null)
                {
                    if (zone.Id != _parent.ZoneId)
                    {
                        zone.ObjectManager.RequestPlayerObject(msg.Data, msg.Position, _connectionId, _worldId, OnZoneTransferComplete);
                    }
                    else if (zone.IsValidPosition(msg.Position))
                    {
                        zone.EventManager.RegisterEvent(new TeleportEvent{Position = msg.Position, Tick = msg.Tick, ObjectId = _parent.Data.Id}, true);
                        WorldServer.SendToClient(new ClientZoneTransferResultMessage { Success = true, Zone = zone.Name, Position = msg.Position, ObjectId = _parent.Data.Id}, _connectionId);
                    }
                    else
                    {
                        WorldServer.SendToClient(new ClientZoneTransferResultMessage { Success = false, Message = "Invalid Position" }, _connectionId);
                    }
                }
                else
                {
                    WorldServer.SendToClient(new ClientZoneTransferResultMessage{Success = false, Message = "Invalid Request"}, _connectionId);
                }
            }
        }

        private void ClientRespawnRequest(ClientRespawnRequestMessage msg)
        {
            var toZone = ZoneService.GetZoneByName(msg.Zone);
            var fromZone = ZoneService.GetZoneById(_parent.ZoneId);
            if (toZone != null && fromZone != null)
            {
                var pos = msg.Position;
                if (!toZone.IsValidPosition(pos))
                {
                    pos = toZone.DefaultSpawn;
                }
                if (toZone == fromZone)
                {
                    toZone.EventManager.RegisterEvent(new RespawnEvent{OwnerId = _parent.Data.Id, Position = pos});
                    WorldServer.SendToClient(new ClientRespawnResultMessage{Zone = toZone.Name, Position = pos, Success = true,}, _connectionId);
                }
                else
                {
                    toZone.ObjectManager.RequestPlayerObject(msg.Data, pos, _connectionId, _worldId, OnRespawnInDifferentZoneComplete);
                }
            }

        }

        private void RemovedFromAggro(RemoveFromAggroMessage msg)
        {
            if (_aggrodMonsters.Contains(msg.OwnerId))
            {
                _aggrodMonsters.Remove(msg.OwnerId);
            }
        }

        private void ApplyExperience(ApplyExperienceMessage msg)
        {
            WorldServer.SendToClient(msg.Message, _connectionId);
        }

        private void ClientOpenDoor(ClientOpenDoorMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                if (zone.ObjectManager.TryGetObjectById(msg.DoorId, out var door))
                {
                    zone.EventManager.RegisterEvent(new DoorEvent{Open = true, OwnerId = _parent.Data.Id, TargetId = door.Data.Id, Tick = msg.Tick}, true);
                }
            }
        }

        private void ClientTriggerEvent(ClientTriggerEventMessage msg)
        {
            foreach (var triggerEvent in msg.TriggerEvents)
            {
                TriggerEventService.TriggerEvent(triggerEvent);
            }
            
        }

        private void ClientHeal(ClientHealMessage msg)
        {
            if (msg.OwnerId == _parent.Data.Id || msg.TargetId == _parent.Data.Id)
            {
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    zone.EventManager.RegisterEvent(new HealEvent{OwnerId = msg.OwnerId, TargetId = msg.TargetId, Amount = msg.Amount, Tick = msg.Tick}, true);
                }
            }
        }

        private void ClientSwitchSignal(ClientSwitchSignalMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.EventManager.RegisterEvent(new SwitchSignalEvent{TargetId = msg.TargetId, Signal = msg.Signal, Locked = msg.Locked, Tick = msg.Tick}, true);
            }
        }

        private void ClientAddKeyItem(ClientAddKeyItemMessage msg)
        {
            ItemService.AddKeyItem(msg.Item, msg.Stack);
        }

        private void ClientOpenChest(ClientOpenChestMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.EventManager.RegisterEvent(new ChestOpenEvent{TargetId = msg.TargetId, Tick = msg.Tick}, true);
            }
        }

        private void ClientDoorCheck(ClientDoorCheckMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.EventManager.RegisterEvent(new DoorCheckEvent{TargetId = msg.TargetId,Tick = msg.Tick}, true);
            }
        }

        private void ClientProjectileMovementUpdate(ClientProjectileMovementUpdateMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null && !zone.ObjectManager.DoesObjectExist(msg.TargetId))
            {
                zone.EventManager.RegisterEvent(new MovementEvent{Id = msg.TargetId, Speed = msg.Speed, Position = msg.Position, Tick = msg.Tick}, true);
            }
        }

        private void ClientTeleport(ClientTeleportMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null && zone.IsValidPosition(msg.Position))
            {
                zone.EventManager.RegisterEvent(new TeleportEvent{ObjectId = _parent.Data.Id, Position = msg.Position, Tick = msg.Tick});
            }
        }

        private void ClientPlayerLeveled(ClientPlayerLeveledMessage msg)
        {
            SystemEventService.SendMessage($"{_parent.Data.DisplayName} has reached level {msg.Level + 1}!", SystemEventType.Level);
        }

        private void ClientMovableUpdate(ClientMovableUpdateMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                if (!zone.ObjectManager.IsPlayer(msg.MovableId))
                {
                    if (msg.Type == MovableType.Grab)
                    {
                        _movableId = msg.MovableId;
                    }
                    else if (!string.IsNullOrEmpty(_movableId) && _movableId == msg.MovableId)
                    {
                        _movableId = string.Empty;
                    }
                    zone.EventManager.RegisterEvent(new MovableEvent
                    {
                        MovableId = msg.MovableId,
                        OwnerId = _parent.Data.Id,
                        MovablePosition = msg.MovablePosition,
                        OwnerPosition = msg.Position,
                        Type = msg.Type,
                        Speed = msg.MoveSpeed,
                        Tick = msg.Tick
                    }, true);
                    
                }
            }
        }

        private void ClientRollUpdate(ClientRollUpdateMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.EventManager.RegisterEvent(
                    new RollEvent
                    {
                        OwnerId = _parent.Data.Id,
                        Position = msg.Position,
                        Direction = msg.Direction,
                        Speed = msg.MoveSpeed,
                        Finished = msg.Finished,
                        Tick = msg.Tick
                    }, true);
            }
        }

        public override void Destroy()
        {
            if (_aggrodMonsters.Count > 0)
            {
                var removeFromAggorMsg = new RemoveFromAggroMessage{OwnerId = _parent.Data.Id};
                foreach (var monster in _aggrodMonsters)
                {
                    this.SendMessageWithFilter(removeFromAggorMsg, monster);
                }
            }
            base.Destroy();
        }
    }
}