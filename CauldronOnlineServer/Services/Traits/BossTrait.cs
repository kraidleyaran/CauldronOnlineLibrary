using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class BossTrait : WorldTrait
    {
        private BossParameter _parameter = new BossParameter();

        public BossTrait(WorldTraitData data) : base(data)
        {
            if (data is BossTraitData bossData)
            {
                _parameter.DisplayName = bossData.DisplayName;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
        }
    }
}