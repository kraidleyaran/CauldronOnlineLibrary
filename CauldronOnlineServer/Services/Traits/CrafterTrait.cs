using System.Linq;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class CrafterTrait : WorldTrait
    {
        private CrafterParameter _parameter = new CrafterParameter();

        public CrafterTrait(CrafterParameter parameter)
        {
            Name = parameter.Type;
            _parameter = parameter;
        }

        public CrafterTrait(WorldTraitData data) : base(data)
        {
            if (data is CrafterTraitData crafterData)
            {
                _parameter.Recipes = crafterData.Recipes.ToArray();
                _parameter.Hitbox = crafterData.Hitbox;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
        }
    }
}