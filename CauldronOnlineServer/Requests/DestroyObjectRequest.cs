using System;

namespace CauldronOnlineServer.Requests
{
    public class DestroyObjectRequest
    {
        public string ObjectId;
        public string PlayerId;
        public Action DoAfter;

        public DestroyObjectRequest(string id, string playerId = "", Action doAfter = null)
        {
            ObjectId = id;
            PlayerId = playerId;
            DoAfter = doAfter;
        }
    }
}