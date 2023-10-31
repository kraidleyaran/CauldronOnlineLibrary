using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class AdvanceSwitchSignalTrait : WorldTrait
    {
        public override bool Instant => true;

        public AdvanceSwitchSignalTrait(WorldTraitData data) : base(data)
        {

        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            this.SendMessageTo(AdvanceSwitchSignalMessage.INSTANCE, parent);
        }
    }
}