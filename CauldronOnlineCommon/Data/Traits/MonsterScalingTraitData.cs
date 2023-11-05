using System;
using CauldronOnlineCommon.Data.Combat;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class MonsterScalingTraitData : WorldTraitData
    {
        public const string TYPE = "MonsterScaling";
        public override string Type => TYPE;

        public string[] ApplyPerPlayer = new string[0];
        public CombatStats Stats;
    }
}