using System;
using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class LootTraitData : WorldTraitData
    {
        public const string TYPE = "Loot";
        public override string Type => TYPE;

        public string LootTable;
        public WorldIntRange Drops;
    }
}