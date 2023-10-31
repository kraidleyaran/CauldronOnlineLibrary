using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class SetSwitchLockStateTraitData : WorldTraitData
    {
        public const string TYPE = "SetSwitchLockState";
        public override string Type => TYPE;

        public bool Lock;
    }
}