using System;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data.Items
{
    [Serializable]
    public class LootRollData
    {
        public string Item;
        public WorldIntRange Stack;
        public float ChanceToDrop;
        public bool SpawnEachStack;
    }
}