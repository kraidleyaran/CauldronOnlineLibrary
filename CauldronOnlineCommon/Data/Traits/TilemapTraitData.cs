using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class TilemapTraitData : WorldTraitData
    {
        public const string TYPE = "Tilemap";
        public override string Type => TYPE;

        public string Tilemap;
    }
}