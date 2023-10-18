using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class SetObjectActiveStateTraitData : WorldTraitData
    {
        public const string TYPE = "SetObjectActiveState";
        public override string Type => TYPE;

        public bool Active;
    }
}