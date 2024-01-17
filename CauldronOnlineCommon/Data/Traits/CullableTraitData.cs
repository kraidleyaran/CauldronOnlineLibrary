using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class CullableTraitData : WorldTraitData
    {
        public const string TYPE = "Cullable";
        public override string Type => TYPE;
    }
}