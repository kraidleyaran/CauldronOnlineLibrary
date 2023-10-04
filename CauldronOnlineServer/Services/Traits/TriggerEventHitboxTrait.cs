using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class TriggerEventHitboxTrait : WorldTrait
    {
        public override bool Instant => true;

        private TriggerEventHitboxParameter _parameter = new TriggerEventHitboxParameter();

        public TriggerEventHitboxTrait(TriggerEventHitboxParameter parameter)
        {
            Name = parameter.Type;
            _parameter = parameter;
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
        }
    }
}