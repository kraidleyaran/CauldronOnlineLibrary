using CauldronOnlineCommon;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class PlayerMovementTrait : WorldTrait
    {
        public const string NAME = "PlayerMovement";

        private string _worldId = string.Empty;

        public PlayerMovementTrait(string worldId)
        {
            Name = NAME;
            _worldId = worldId;
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<ClientMovementUpdateMessage>(ClientMovement, _worldId);

            _parent.SubscribeWithFilter<ApplyKnockbackMessage>(ApplyKnockback, _id);
            _parent.SubscribeWithFilter<UpdatePositionMessage>(UpdatePosition, _id);
        }

        private void ClientMovement(ClientMovementUpdateMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.EventManager.RegisterEvent(new MovementEvent { Id = _parent.Data.Id, Position = msg.Position, Direction = msg.Direction, Tick = msg.Tick, Speed = msg.Speed }, true);
            }
        }

        private void ApplyKnockback(ApplyKnockbackMessage msg)
        {
            _parent.SetPosition(msg.Position);
        }

        private void UpdatePosition(UpdatePositionMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                var tile = zone.GetTileByWorldPosition(msg.Position);
                if (tile != null && (_parent.Tile == null || _parent.Tile != tile))
                {
                    _parent.Tile = tile;
                    this.SendMessageTo(ZoneTileUpdatedMessage.INSTANCE, _parent);
                }
            }
        }
    }
}