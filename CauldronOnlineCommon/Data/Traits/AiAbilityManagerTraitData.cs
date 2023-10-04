using System;
using CauldronOnlineCommon.Data.Combat;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class AiAbilityManagerTraitData : WorldTraitData
    {
        public const string TYPE = "AiAbilityManager";
        public override string Type => TYPE;

        public AiAbilityData[] Abilities;
    }
}