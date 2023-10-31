using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class SetSwitchSignalTraitData : WorldTraitData
    {
        public const string TYPE = "SetSwitchSignal";
        public override string Type => TYPE;

        public int Signal;


    }
}