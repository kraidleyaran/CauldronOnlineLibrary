using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class RestoreManaTraitData : WorldTraitData
    {
        public const string TYPE = "RestoreMana";
        public override string Type => TYPE;

        public int Amount;
    }
}