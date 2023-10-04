using System;
using CauldronOnlineCommon.Data.Combat;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class TerrainTraitData : WorldTraitData
    {
        public const string TYPE = "Terrain";
        public override string Type => TYPE;

        public HitboxData Hitbox;
    }
}