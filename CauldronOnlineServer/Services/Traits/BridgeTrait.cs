using System.Linq;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.TriggerEvents;
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
            _parent.AddParameter(_parameter);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<SetBridgeStateMessage>(SetBridgeState, _id);
            foreach (var triggerEvent in _parameter.ToggleOnTriggerEvents)
            {
                this.SubscribeWithFilter<ZoneEventTriggerMessage>(ZoneEventTrigger, TriggerEventService.GetFilter(_parent.ZoneId, triggerEvent));
            }

            foreach (var signal in _parameter.ToggleOnSwitchSignals)
            {
                this.SubscribeWithFilter<UpdateSignalMessage>(UpdateSignal, SwitchTrait.GenerateFilter(signal.Switch, _parent.ZoneId));
            }
            
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

        private void ZoneEventTrigger(ZoneEventTriggerMessage msg)
        {
            _parameter.Active = !_parameter.Active;
            _parent.RefreshParameters();

            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.EventManager.RegisterEvent(new BridgeStateEvent { Active = _parameter.Active, TargetId = _parent.Data.Id });
            }
        }

        private void UpdateSignal(UpdateSignalMessage msg)
        {
            var requiredSignal = _parameter.ToggleOnSwitchSignals.FirstOrDefault(s => msg.SwitchName == s.Switch);
            if (requiredSignal != null && requiredSignal.Signal == msg.Signal)
            {
                _parameter.Active = !_parameter.Active;
                _parent.RefreshParameters();

                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    zone.EventManager.RegisterEvent(new BridgeStateEvent { Active = _parameter.Active, TargetId = _parent.Data.Id });
                }
            }
        }
    }

}