using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class TriggerEventReceiverTraitData : WorldTraitData
    {
        public const string TYPE = "TriggerEventReceiver";
        public override string Type => TYPE;

        public string[] ApplyOnTriggerEvent;
        public string[] TriggerEvents;
    }
}