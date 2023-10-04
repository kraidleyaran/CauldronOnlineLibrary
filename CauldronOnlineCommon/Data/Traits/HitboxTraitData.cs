using System;
using CauldronOnlineCommon.Data.Combat;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class HitboxTraitData : WorldTraitData
    {
        public const string TYPE = "Hitbox";
        public override string Type => TYPE;

        public ApplyHitboxData[] Hitboxes;
    }
}