﻿using System;
using CauldronOnlineCommon.Data.Switches;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class SwitchSignalReceiverTraitData : WorldTraitData
    {
        public const string TYPE = "SwitchSignalReceiver";
        public override string Type => TYPE;

        public OnSwitchSignalData[] OnSignals;
    }
}