using System;

namespace CauldronOnlineCommon.Data
{
    [Serializable]
    public class OnSwitchSignalData
    {
        public string[] ApplyOnSignal;
        public string Switch;
        public int Signal;
    }
}