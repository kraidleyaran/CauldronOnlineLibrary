using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class WalledTrait : WorldTrait
    {
        private WalledParameter _parameter = new WalledParameter();

        public WalledTrait(WorldTraitData data) : base(data)
        {
            if (data is WalledTraitData walled)
            {
                _parameter.Hitbox = walled.Hitbox;
                _parameter.CheckForPlayer = walled.CheckForPlayer;
                _parameter.IgnoreGround = walled.IgnoreGround;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
        }
    }
}