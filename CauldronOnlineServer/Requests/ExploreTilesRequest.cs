using System.Linq;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineServer.Requests
{
    public class ExploreTilesRequest
    {
        public WorldVector2Int[] Tiles;

        public ExploreTilesRequest(WorldVector2Int[] tiles)
        {
            Tiles = tiles.ToArray();
        }
    }
}