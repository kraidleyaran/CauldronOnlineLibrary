using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class AdvanceSwitchSignalTraitData : WorldTraitData
    {
        public const string TYPE = "AdvanceSwitchSignal";
        public override string Type => TYPE;
    }
}