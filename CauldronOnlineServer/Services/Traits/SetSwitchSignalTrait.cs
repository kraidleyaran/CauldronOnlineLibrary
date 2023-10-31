using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class SetSwitchSignalTrait : WorldTrait
    {
        public override bool Instant => true;

        private int _signal = 0;

        public SetSwitchSignalTrait(WorldTraitData data) : base(data)
        {
            if (data is SetSwitchSignalTraitData switchData)
            {
                _signal = switchData.Signal;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            this.SendMessageTo(new SetSwitchSignalMessage{Signal = _signal, OverrideLock = true}, _parent);
        }
    }
}