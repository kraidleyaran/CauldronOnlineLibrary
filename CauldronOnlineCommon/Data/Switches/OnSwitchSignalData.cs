using System;

namespace CauldronOnlineCommon.Data.Switches
{
    [Serializable]
    public class OnSwitchSignalData
    {
        public string[] ApplyOnSignal;
        public string[] ApplyEventsOnSignal;
        public string Switch;
        public int Signal;
        
    }
}