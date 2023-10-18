using System;
using CauldronOnlineCommon.Data.Switches;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class MultiSwitchSignalReceiverTraitData : WorldTraitData
    {
        public const string TYPE = "MultiSwitchSignalReceiver";
        public override string Type => TYPE;

        public OnMultiSwitchSignalData Data;
    }
}