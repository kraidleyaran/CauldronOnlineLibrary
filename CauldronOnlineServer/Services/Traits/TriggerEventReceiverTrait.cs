using System.Collections.Generic;
using System.Linq;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class TriggerEventReceiverTrait : WorldTrait
    {
        private string[] _applyOnTriggerEvent = new string[0];
        private string[] _triggerEvents = new string[0];
        private bool _requireAllEvents = false;

        private List<string> _triggeredEvents = new List<string>();

        public TriggerEventReceiverTrait(WorldTraitData data) : base(data)
        {
            if (data is TriggerEventReceiverTraitData triggerEvent)
            {
                _applyOnTriggerEvent = triggerEvent.ApplyOnTriggerEvent;
                _triggerEvents = triggerEvent.TriggerEvents;
                _requireAllEvents = triggerEvent.RequireAllEvents;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            foreach (var triggerEvent in _triggerEvents)
            {
                this.SubscribeWithFilter<ZoneEventTriggerMessage>(msg => ZoneEventTrigger(triggerEvent), TriggerEventService.GetFilter(_parent.ZoneId, triggerEvent));
            }
        }

        private void ZoneEventTrigger(string triggerEvent)
        {
            if (_requireAllEvents && !_triggeredEvents.Contains(triggerEvent))
            {
                _triggeredEvents.Add(triggerEvent);
            }

            if (!_requireAllEvents || _triggeredEvents.Count == _triggerEvents.Length)
            {
                var traits = TraitService.GetWorldTraits(_applyOnTriggerEvent);
                foreach (var trait in traits)
                {
                    _parent.AddTrait(trait, _parent);
                }
            }

        }
    }
}