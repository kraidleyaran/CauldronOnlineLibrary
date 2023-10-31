using System.Linq;
using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.Switches;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class SwitchSignalReceiverTrait : WorldTrait
    {
        private OnSwitchSignalData[] _onSignal = new OnSwitchSignalData[0];

        public SwitchSignalReceiverTrait(WorldTraitData data) : base(data)
        {
            if (data is SwitchSignalReceiverTraitData receiverData)
            {
                _onSignal = receiverData.OnSignals;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            foreach (var switchSignal in _onSignal)
            {
                this.SubscribeWithFilter<UpdateSignalMessage>(UpdateSignal, SwitchTrait.GenerateFilter(switchSignal.Switch, _parent.ZoneId));
            }
        }

        private void UpdateSignal(UpdateSignalMessage msg)
        {
            var data = _onSignal.FirstOrDefault(d => d.Switch == msg.SwitchName);
            if (data != null && data.Signal == msg.Signal)
            {
                var traits = TraitService.GetWorldTraits(data.ApplyOnSignal);
                foreach (var trait in traits)
                {
                    _parent.AddTrait(trait);
                }

                if (data.ApplyEventsOnSignal.Length > 0)
                {
                    foreach (var triggerEvent in data.ApplyEventsOnSignal)
                    {
                        TriggerEventService.TriggerEvent(triggerEvent);
                    }
                }

            }
        }
    }
}