using System;
using CauldronOnlineCommon.Data.Combat;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class CombatStatsTraitData : WorldTraitData
    {
        public const string TYPE = "CombatStats";
        public override string Type => TYPE;

        public CombatStats Stats;
    }
}