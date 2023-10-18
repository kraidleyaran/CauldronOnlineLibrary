using System;

namespace CauldronOnlineCommon.Data.Switches
{
    [Serializable]
    public class OnMultiSwitchSignalData
    {
        public RequiredSwitchSignalData[] RequiredSignals;
        public string[] ApplyOnSignal;
        public string[] ApplyTriggerEventsOnSignal;
    }
}