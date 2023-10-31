using System;
using CauldronOnlineCommon.Data.Combat;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class WalledTraitData : WorldTraitData
    {
        public const string TYPE = "Walled";
        public override string Type => TYPE;

        public HitboxData Hitbox;
        public bool IgnoreGround;
        public bool CheckForPlayer;
    }
}