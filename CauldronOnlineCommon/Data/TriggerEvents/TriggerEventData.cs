using System;

namespace CauldronOnlineCommon.Data.TriggerEvents
{
    [Serializable]
    public class TriggerEventData
    {
        public const string EXTENSION = "ted";
        public string Name;
        public int MaxActivations;
    }
}