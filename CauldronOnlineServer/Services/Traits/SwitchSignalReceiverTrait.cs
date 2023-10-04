using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using MessageBusLib;

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
                this.SubscribeWithFilter<UpdateSignalMessage>(msg => UpdateSignal(msg.Signal, switchSignal), SwitchTrait.GenerateFilter(switchSignal.Switch, _parent.ZoneId));
            }
        }

        private void UpdateSignal(int signal, OnSwitchSignalData data)
        {
            if (data.Signal == signal)
            {
                var traits = TraitService.GetWorldTraits(data.ApplyOnSignal);
                foreach (var trait in traits)
                {
                    _parent.AddTrait(trait);
                }
            }
        }
    }
}