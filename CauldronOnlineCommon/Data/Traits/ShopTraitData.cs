using System;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Items;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class ShopTraitData : WorldTraitData
    {
        public const string TYPE = "Shop";
        public override string Type => TYPE;
        public ShopItemData[] Items;
        public HitboxData Hitbox;
    }
}