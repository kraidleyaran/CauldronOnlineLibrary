using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones;
using MessageBusLib;

namespace CauldronOnlineServer.Services.Traits
{
    public class TriggerEventReceiverTrait : WorldTrait
    {
        private string[] _applyOnTriggerEvent = new string[0];
        private string[] _triggerEvents = new string[0];

        public TriggerEventReceiverTrait(WorldTraitData data) : base(data)
        {
            if (data is TriggerEventReceiverTraitData triggerEvent)
            {
                _applyOnTriggerEvent = triggerEvent.ApplyOnTriggerEvent;
                _triggerEvents = triggerEvent.TriggerEvents;
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
                this.SubscribeWithFilter<ZoneEventTriggerMessage>(ZoneEventTrigger, TriggerEventService.GetFilter(_parent.ZoneId, triggerEvent));
            }
        }

        private void ZoneEventTrigger(ZoneEventTriggerMessage msg)
        {
            var traits = TraitService.GetWorldTraits(_applyOnTriggerEvent);
            foreach (var trait in traits)
            {
                _parent.AddTrait(trait, _parent);
            }
        }
    }
}