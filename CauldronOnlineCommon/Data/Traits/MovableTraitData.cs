using System;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class MovableTraitData : WorldTraitData
    {
        public const string TYPE = "Movable";
        public override string Type => TYPE;

        public int MoveSpeed;
        public HitboxData Hitbox;
        public HitboxData HorizontalHitbox;
        public WorldOffset Offset;
    }
}