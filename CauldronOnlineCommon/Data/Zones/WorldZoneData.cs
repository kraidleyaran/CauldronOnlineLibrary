using System;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data.Zones
{
    [Serializable]
    public class WorldZoneData
    {
        public const string EXTENSION = "wzd";

        public string Name;
        public string[] Aliases = new string[0];
        public WorldVector2Int Size = WorldVector2Int.Zero;
        public ZoneTileData[] Tiles = new ZoneTileData[0];
        public ZoneSpawnData[] Spawns = new ZoneSpawnData[0];
        public WorldVector2Int Offset = WorldVector2Int.Zero;
        public WorldVector2Int DefaultSpawn = WorldVector2Int.Zero;
        public string[] ResetEvents = new string[0];
    }
}