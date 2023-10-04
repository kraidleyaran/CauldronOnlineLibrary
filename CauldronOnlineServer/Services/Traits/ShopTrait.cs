using System.Linq;
using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class ShopTrait : WorldTrait
    {
        private ShopParameter _shopParameter = null;

        public ShopTrait(WorldTraitData data) : base(data)
        {
            _shopParameter = new ShopParameter {Items = new ShopItemData[0]};
            if (data is ShopTraitData shopData)
            {
                _shopParameter.Items = shopData.Items.ToArray();
                _shopParameter.Hitbox = shopData.Hitbox;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_shopParameter);
        }
        
    }
}