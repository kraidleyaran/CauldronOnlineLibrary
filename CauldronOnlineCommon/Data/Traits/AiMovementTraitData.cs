using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class AiMovementTraitData : WorldTraitData
    {
        public const string TYPE = "AiMovement";
        public override string Type => TYPE;

        public int MoveSpeed;
    }
}