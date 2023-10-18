using System.Collections.Generic;
using CauldronOnlineCommon.Data.Switches;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class MultiSwitchSignalReceiverTrait : WorldTrait
    {
        private OnMultiSwitchSignalData _signalData = null;

        private Dictionary<string, int> _requiredSignals = new Dictionary<string, int>();
        private List<string> _matchingSignals = new List<string>();

        public MultiSwitchSignalReceiverTrait(WorldTraitData data) : base(data)
        {
            if (data is MultiSwitchSignalReceiverTraitData receiverData)
            {
                _signalData = receiverData.Data;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            foreach (var signal in _signalData.RequiredSignals)
            {
                if (!_requiredSignals.ContainsKey(signal.Switch))
                {
                    _requiredSignals.Add(signal.Switch, signal.Signal);
                }
            }
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            foreach (var signal in _requiredSignals.Keys)
            {
                this.SubscribeWithFilter<UpdateSignalMessage>(msg => UpdateSignal(msg, signal), SwitchTrait.GenerateFilter(signal, _parent.ZoneId));
            }
        }

        private void UpdateSignal(UpdateSignalMessage msg, string switchSignal)
        {
            if (_requiredSignals.TryGetValue(switchSignal, out var required))
            {
                if (required == msg.Signal)
                {
                    if (!_matchingSignals.Contains(switchSignal))
                    {
                        _matchingSignals.Add(switchSignal);
                    }
                }
                else if (_matchingSignals.Contains(switchSignal))
                {
                    _matchingSignals.Remove(switchSignal);
                }

                if (_matchingSignals.Count == _requiredSignals.Count)
                {
                    if (_signalData.ApplyOnSignal.Length > 0)
                    {
                        var traits = TraitService.GetWorldTraits(_signalData.ApplyOnSignal);
                        foreach (var trait in traits)
                        {
                            _parent.AddTrait(trait);
                        }
                    }

                    if (_signalData.ApplyTriggerEventsOnSignal.Length > 0)
                    {
                        foreach (var triggerEvent in _signalData.ApplyTriggerEventsOnSignal)
                        {
                            TriggerEventService.TriggerEvent(triggerEvent);
                        }
                    }
                }
            }
        }
    }
}