using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class BridgeTrait : WorldTrait
    {
        private BridgeParameter _parameter = new BridgeParameter();

        public BridgeTrait(BridgeParameter parameter)
        {
            Name = parameter.Type;
            _parameter = parameter;
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<SetBridgeStateMessage>(SetBridgeState, _id);
        }

        private void SetBridgeState(SetBridgeStateMessage msg)
        {
            _parameter.Active = msg.Active;
            _parent.RefreshParameters();
            if (!msg.IsEvent)
            {
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    zone.EventManager.RegisterEvent(new BridgeStateEvent{Active = _parameter.Active, TargetId = _parent.Data.Id});
                }
            }
        }
    }

}