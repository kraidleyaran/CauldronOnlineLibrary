using System;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class StaticTeleportTraitData : WorldTraitData
    {
        public const string TYPE = "StaticTeleport";
        public override string Type => TYPE;

        public WorldVector2Int[] Tiles;
        public int TeleportTicks;
        public bool IsLocalPosition;
        public string[] ApplyOnTeleportEnd;
        public int AfterTicks;
    }
}