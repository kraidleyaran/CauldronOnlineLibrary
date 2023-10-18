using System;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Items;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class CrafterTraitData : WorldTraitData
    {
        public const string TYPE = "Crafter";
        public override string Type => TYPE;

        public ItemRecipeData[] Recipes;
        public HitboxData Hitbox;
    }
}