using System;
using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineServer.Requests
{
    public class CreatePlayerObjectRequest
    {
        public ClientCharacterData Data;
        public int ConnectionId;
        public string WorldId;

        public WorldVector2Int Position;
        //Id, Zone, Pos
        public Action<string, string, WorldVector2Int> DoAfter;

        public CreatePlayerObjectRequest(ClientCharacterData data, WorldVector2Int position, int connctionId, string worldId, Action<string, string, WorldVector2Int> doAfter)
        {
            Data = data;
            ConnectionId = connctionId;
            WorldId = worldId;
            DoAfter = doAfter;
            Position = position;
        }
    }
}