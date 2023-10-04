using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Zones;
using CauldronOnlineServer.Interfaces;
using RogueSharp;

namespace CauldronOnlineServer.Services.Zones
{
    public class ZoneTile : IDestroyable
    {
        public WorldVector2Int WorldPosition;
        public WorldVector2Int Position;
        public ICell Cell;
        public WorldObject BlockingObject;
        public bool Blocked => BlockingObject != null;

        public ZoneTile(ZoneTileData data, ICell cell)
        {
            WorldPosition = data.WorldPosition;
            Position = data.Position;
            Cell = cell;
        }

        public void Destroy()
        {
            Cell = null;
            WorldPosition = WorldVector2Int.Zero;
            Position = WorldVector2Int.Zero;
        }
    }
}