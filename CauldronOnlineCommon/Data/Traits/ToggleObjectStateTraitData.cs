using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class ToggleObjectStateTraitData : WorldTraitData
    {
        public const string TYPE = "ToggleObjectState";
        public override string Type => TYPE;
    }
}