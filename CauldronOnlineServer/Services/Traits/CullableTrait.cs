using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class CullableTrait : WorldTrait
    {
        public CullableTrait(WorldTraitData data) : base(data)
        {

        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(new CullableParameter());
        }
    }
}