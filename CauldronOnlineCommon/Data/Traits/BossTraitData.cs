using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class BossTraitData : WorldTraitData
    {
        public const string TYPE = "Boss";
        public override string Type => TYPE;

        public string DisplayName;
    }
}