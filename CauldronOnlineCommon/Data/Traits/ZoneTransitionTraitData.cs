using System;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class ZoneTransitionTraitData : WorldTraitData
    {
        public const string TYPE = "ZoneTransition";
        public override string Type => TYPE;

        public string Zone { get; set; }
        public WorldVector2Int Position { get; set; }
        public float Rotation { get; set; }
    }
}