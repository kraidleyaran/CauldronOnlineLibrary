using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class ActivateTriggerEventsTraitData : WorldTraitData
    {
        public const string TYPE = "ActivateTriggerEvents";
        public override string Type => TYPE;

        public string[] TriggerEvents = new string[0];
    }
}