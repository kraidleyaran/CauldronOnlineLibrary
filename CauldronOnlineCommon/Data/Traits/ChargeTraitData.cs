using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class ChargeTraitData : WorldTraitData
    {
        public const string TYPE = "Charge";
        public override string Type => TYPE;

        public int Speed;
        public int Distance;
    }
}