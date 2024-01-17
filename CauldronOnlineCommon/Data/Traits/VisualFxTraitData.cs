using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class VisualFxTraitData : WorldTraitData
    {
        public const string TYPE = "VisualFx";
        public override string Type => TYPE;

        public string VisualFx;
    }
}