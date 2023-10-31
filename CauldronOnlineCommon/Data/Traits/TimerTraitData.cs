using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class TimerTraitData : WorldTraitData
    {
        public const string TYPE = "Timer";
        public override string Type => TYPE;

        public string[] ApplyOnStart;
        public string[] ApplyOnLoop;
        public int TotalTicks;
        public int TotalLoops;
        public bool ShowOnClient;
    }
}