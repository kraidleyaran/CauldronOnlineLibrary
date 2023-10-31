using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class SetSwitchLockStateTrait : WorldTrait
    {
        public override bool Instant => true;

        private bool _locked = false;

        public SetSwitchLockStateTrait(WorldTraitData data) : base(data)
        {
            if (data is SetSwitchLockStateTraitData lockData)
            {
                _locked = lockData.Lock;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            this.SendMessageTo(new SetSwitchLockStateMessage{Locked = _locked}, _parent);
        }
    }
}