using System;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data.Zones
{
    [Serializable]
    public class ZoneSpawnData
    {
        public ObjectSpawnData Spawn;
        public WorldVector2Int Tile;
        public bool IsWorldPosition;
    }
}