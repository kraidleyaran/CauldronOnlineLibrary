using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class ChargeTrait : WorldTrait
    {
        private int _chargeSpeed = 1;
        private int _chargeDistance = 1;

        private int _distanceTravelled = 0;

        private WorldVector2Int _direction = WorldVector2Int.Zero;

        public ChargeTrait(WorldTraitData data) : base(data)
        {
            if (data is ChargeTraitData chargeData)
            {
                _chargeSpeed = chargeData.Speed;
                _chargeDistance = chargeData.Distance;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            this.SendMessageTo(new QueryFaceDirectionMessage{DoAfter = UpdateDirection}, _parent);
            if (_direction != WorldVector2Int.Zero)
            {
                _parent.SetObjectState(WorldObjectState.Charging);
                SubscribeToMessages();
            }
            else
            {
                _parent.RemoveTrait(this);
            }
        }

        private void UpdateDirection(WorldVector2Int direction)
        {
            _direction = direction;
        }

        private void StopCharge()
        {
            _parent.SetObjectState(WorldObjectState.Active);
            _parent.RemoveTrait(this);
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<ZoneUpdateTickMessage>(ZoneUpdateTick, _parent.ZoneId);
        }

        private void ZoneUpdateTick(ZoneUpdateTickMessage msg)
        {
            if (_direction != WorldVector2Int.Zero)
            {
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    var newPos = _parent.Data.Position + _direction * _chargeSpeed;
                    var tile = zone.GetTileByWorldPosition(newPos);
                    if (tile != null)
                    {
                        _parent.SetPosition(newPos);
                        zone.EventManager.RegisterEvent(new MovementEvent
                        {
                            Id = _parent.Data.Id,
                            Position = newPos,
                            Speed = _chargeSpeed
                        });

                        _distanceTravelled += _chargeSpeed;
                        if (_distanceTravelled >= _chargeDistance)
                        {
                            StopCharge();
                        }
                    }
                    else
                    {
                        StopCharge();
                    }
                }
                else
                {
                    StopCharge();
                }
            }
        }

        




    }
}