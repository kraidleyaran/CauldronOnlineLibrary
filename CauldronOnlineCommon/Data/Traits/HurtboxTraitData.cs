using System;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class HurtboxTraitData : WorldTraitData
    {
        public const string TYPE = "Hurtbox";
        public override string Type => TYPE;

        public WorldVector2Int Size;
        public WorldVector2Int Offset;
        public bool Knockback;
    }
}