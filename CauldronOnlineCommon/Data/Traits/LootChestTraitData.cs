using System;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class LootChestTraitData : WorldTraitData
    {
        public const string TYPE = "LootChest";
        public override string Type => TYPE;

        public string LootTable;
        public WorldIntRange Drops;
        public string OpenSprite;
        public string ClosedSprite;
        public HitboxData Hitbox;
    }
}