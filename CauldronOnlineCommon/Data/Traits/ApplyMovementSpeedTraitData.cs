using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class ApplyMovementSpeedTraitData : WorldTraitData
    {
        public const string TYPE = "ApplyMovementSpeed";
        public override string Type => TYPE;

        public int Amount;
        public bool Bonus;
    }
}