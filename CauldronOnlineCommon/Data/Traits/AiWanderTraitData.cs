using System;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class AiWanderTraitData : WorldTraitData
    {
        public const string TYPE = "AiWander";
        public override string Type => TYPE;

        public int WanderRange;
        public bool Anchor;
        public WorldIntRange IdleTicks;
        public float ChanceToIdle;
        public float DiagonalCost;
    }
}