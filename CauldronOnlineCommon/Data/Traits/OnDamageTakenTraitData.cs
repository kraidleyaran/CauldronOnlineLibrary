using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class OnDamageTakenTraitData : WorldTraitData
    {
        public const string TYPE = "OnDamageTaken";
        public override string Type => TYPE;

        public int RequiredAmount;
        public string[] ApplyOnAmount = new string[0];
        public bool ResetOnApply;
    }
}