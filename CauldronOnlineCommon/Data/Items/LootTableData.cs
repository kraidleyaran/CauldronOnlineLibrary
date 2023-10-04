using System;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data.Items
{
    [Serializable]
    public class LootTableData
    {
        public const string EXTENSION = "ltd";
        public string Name;
        public LootRollData[] LootRolls;
    }
}