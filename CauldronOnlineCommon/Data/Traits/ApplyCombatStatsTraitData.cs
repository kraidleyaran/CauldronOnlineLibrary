using System;
using CauldronOnlineCommon.Data.Combat;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class ApplyCombatStatsTraitData : WorldTraitData
    {
        public const string TYPE = "ApplyCombatStats";
        public override string Type => TYPE;

        public CombatStats Stats;
        public bool Bonus;
    }
}