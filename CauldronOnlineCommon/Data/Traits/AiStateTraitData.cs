using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class AiStateTraitData : WorldTraitData
    {
        public const string TYPE = "AiState";
        public override string Type => TYPE;
    }
}