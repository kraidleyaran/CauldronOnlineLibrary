using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class ActivateTriggerEventsTrait : WorldTrait
    {
        public override bool Instant => true;

        private string[] _triggerEvents = new string[0];

        public ActivateTriggerEventsTrait(WorldTraitData data) : base(data)
        {
            if (data is ActivateTriggerEventsTraitData triggerData)
            {
                _triggerEvents = triggerData.TriggerEvents;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            foreach (var trigger in _triggerEvents)
            {
                TriggerEventService.TriggerEvent(trigger);
            }
        }
    }
}