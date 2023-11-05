using System;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class ProjectileRedirectTraitData : WorldTraitData
    {
        public const string TYPE = "ProjectileRedirect";
        public override string Type => TYPE;

        public WorldVector2Int Direction;
        public string[] Tags;
        public HitboxData Hitbox;
    }
}