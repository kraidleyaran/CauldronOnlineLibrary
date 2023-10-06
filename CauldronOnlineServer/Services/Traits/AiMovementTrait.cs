using System.Collections.Generic;
using System.Linq;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class AiMovementTrait : WorldTrait
    {
        private int _moveSpeed = 1;
        private int _bonusSpeed = 0;

        private List<ZoneTile> _currentPath = new List<ZoneTile>();

        private bool _knockbackActive = false;

        private WorldVector2Int _direction = WorldVector2Int.Zero;

        //private ObjectPathParameter _pathParameter = new ObjectPathParameter();

        public AiMovementTrait(WorldTraitData data) : base(data)
        {
            if (data is AiMovementTraitData movementData)
            {
                _moveSpeed = movementData.MoveSpeed;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            //_parent.AddParameter(_pathParameter);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<ZoneUpdateTickMessage>(ZoneUpdateTick, _parent.ZoneId);
            _parent.SubscribeWithFilter<SetCurrentPathMessage>(SetCurrentPath, _id);
            _parent.SubscribeWithFilter<ClearCurrentPathMessage>(ClearCurrentPath, _id);
            _parent.SubscribeWithFilter<ObjectStateUpdatedMessage>(ObjectStateUpdated, _id);
            _parent.SubscribeWithFilter<ApplyKnockbackMessage>(ApplyKnockback, _id);
            _parent.SubscribeWithFilter<KnockbackFinishedMessage>(KnockbackFinished, _id);
            _parent.SubscribeWithFilter<ApplyMovementSpeedMessage>(ApplyMovementSpeed, _id);
            _parent.SubscribeWithFilter<QueryFaceDirectionMessage>(QueryFaceDirection, _id);
            _parent.SubscribeWithFilter<SetFaceDirectionMessage>(SetFaceDirection, _id);
        }

        private void SetCurrentPath(SetCurrentPathMessage msg)
        {
            _currentPath = msg.Path.ToList();
            _direction = _currentPath[0].Position - _parent.Tile.Position;
            //var path = _currentPath.Select(t => t.WorldPosition).ToArray();
            //var zone = ZoneService.GetZoneById(_parent.ZoneId);
            //zone?.EventManager.RegisterEvent(new UpdatePathEvent{Path = path, OwnerId = _parent.Data.Id, Speed = _moveSpeed});
            //_pathParameter.Positions = path;
            //_pathParameter.Speed = _moveSpeed;
            //_parent.AddParameter(_pathParameter);
        }

        private void ZoneUpdateTick(ZoneUpdateTickMessage msg)
        {
            if (_parent.State == WorldObjectState.Active && _currentPath.Count > 0 && !_knockbackActive)
            {
                var moveTo = _currentPath[0];
                var moveSpeed = _moveSpeed + _bonusSpeed;
                if (_parent.Data.Position.Distance(moveTo.WorldPosition) > moveSpeed)
                {
                    var moveToPos = _parent.Data.Position + _parent.Data.Position.Direction(moveTo.WorldPosition) * moveSpeed;
                    var zone = ZoneService.GetZoneById(_parent.ZoneId);
                    if (zone != null && zone.IsValidPosition(moveToPos))
                    {
                        _parent.SetPosition(moveToPos);
                        zone.EventManager.RegisterEvent(new MovementEvent { Id = _parent.Data.Id, Position = moveToPos, Speed = moveSpeed });
                    }
                }
                else
                {
                    _parent.SetPosition(moveTo.WorldPosition);
                    _parent.Tile = moveTo;
                    this.SendMessageTo(ZoneTileUpdatedMessage.INSTANCE, _parent);
                    _currentPath.RemoveAt(0);
                    var zone = ZoneService.GetZoneById(_parent.ZoneId);
                    if (zone != null)
                    {
                        zone.EventManager.RegisterEvent(new MovementEvent { Id = _parent.Data.Id, Position = moveTo.WorldPosition, Speed = moveSpeed });
                    }
                }
            }
        }

        private void ClearCurrentPath(ClearCurrentPathMessage msg)
        {
            if (_currentPath.Count > 0)
            {
                //var zone = ZoneService.GetZoneById(_parent.ZoneId);
                //if (zone != null)
                //{
                //    zone.EventManager.RegisterEvent(new UpdatePathEvent{OwnerId = _parent.Data.Id, Path = new WorldVector2Int[0]});
                //}
                _currentPath.Clear();
            }
            
        }

        private void ObjectStateUpdated(ObjectStateUpdatedMessage msg)
        {
            if (_parent.State != WorldObjectState.Active && _parent.State != WorldObjectState.Attacking)
            {
                if (_currentPath.Count > 0)
                {
                    //var zone = ZoneService.GetZoneById(_parent.ZoneId);
                    //if (zone != null)
                    //{
                    //    zone.EventManager.RegisterEvent(new UpdatePathEvent { OwnerId = _parent.Data.Id, Path = new WorldVector2Int[0] });
                    //}
                    _currentPath.Clear();
                }
                
            }
        }

        private void ApplyKnockback(ApplyKnockbackMessage msg)
        {
            _knockbackActive = true;
            _currentPath.Clear();
        }

        private void KnockbackFinished(KnockbackFinishedMessage msg)
        {
            _knockbackActive = false;
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            var tile = zone?.GetTileByWorldPosition(_parent.Data.Position);
            if (tile != null && tile != _parent.Tile)
            {
                _parent.Tile = tile;
                this.SendMessageTo(ZoneTileUpdatedMessage.INSTANCE, _parent);
            }
        }

        private void ApplyMovementSpeed(ApplyMovementSpeedMessage msg)
        {
            if (msg.Bonus)
            {
                _bonusSpeed += msg.Speed;
            }
            else
            {
                _moveSpeed += msg.Speed;
            }
        }

        private void SetFaceDirection(SetFaceDirectionMessage msg)
        {
            _direction = msg.Direction;
        }

        private void QueryFaceDirection(QueryFaceDirectionMessage msg)
        {
            msg.DoAfter.Invoke(_direction);
        }
    }
}