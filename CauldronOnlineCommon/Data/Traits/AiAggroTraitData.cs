using System;
using CauldronOnlineCommon.Data.Combat;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class AiAggroTraitData : WorldTraitData
    {
        public const string TYPE = "AiAggro";
        public override string Type => TYPE;

        public int AggroRange;
        public int DefaultAggro;
        public string[] ApplyOnAggro = new string[0];
        public float DiagonalCost = 0f;
        public AggroType AggroType;
        public bool HealOnAggroLoss;
    }
}