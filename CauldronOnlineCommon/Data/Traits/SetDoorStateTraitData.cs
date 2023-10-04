using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class SetDoorStateTraitData : WorldTraitData
    {
        public const string TYPE = "SetDoorState";
        public override string Type => TYPE;

        public bool Open;
    }
}