using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class RestoreManaTrait : WorldTrait
    {
        public override bool Instant => true;

        private int _amount = 1;

        public RestoreManaTrait(WorldTraitData data) : base(data)
        {
            if (data is RestoreManaTraitData restore)
            {
                _amount = restore.Amount;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            this.SendMessageTo(new RestoreManaMessage{Amount = _amount}, _parent);
        }
    }
}