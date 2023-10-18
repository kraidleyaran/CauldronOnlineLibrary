using System;
using System.Collections.Generic;
using System.Linq;
using CauldronOnlineCommon;

namespace CauldronOnlineServer.Services.Client
{
    public class ClientService : WorldService
    {
        private static ClientService _instance = null;

        private const string CLIENT = "Client";

        public override string Name => CLIENT;

        private Dictionary<string, PlayerClient> _clients = new Dictionary<string, PlayerClient>();
        private Dictionary<int, string> _reverseLookup = new Dictionary<int, string>();

        public override void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                base.Start();
            }
        }

        public override void Stop()
        {
            if (_instance != null && _instance == this)
            {
                _instance = null;
            }
            base.Stop();
        }

        private string GenerateId()
        {
            var id = $"{CLIENT}-{Guid.NewGuid().ToString()}";
            while (_clients.ContainsKey(id))
            {
                id = $"{CLIENT}-{Guid.NewGuid().ToString()}";
            }

            return id;
        }

        public static void RegisterClient(int connectionId)
        {
            var id = _instance.GenerateId();
            _instance._clients.Add(id, new PlayerClient(connectionId, id));
            _instance._reverseLookup.Add(connectionId, id);
            WorldServer.SendToClient(new ClientConnectResultMessage { Success = true, ClientId = id }, connectionId);
        }

        public static void UnregisterClient(int connectionId)
        {
            if (_instance._reverseLookup.TryGetValue(connectionId, out var id) && _instance._clients.TryGetValue(id, out var client))
            {
                _instance._clients.Remove(id);
                _instance._reverseLookup.Remove(connectionId);
                client.Destroy();
            }
        }

        public static bool IsValidWorldId(int connectionId, string worldId)
        {
            return _instance._reverseLookup.TryGetValue(connectionId, out var id) && worldId == id;
        }

        public static int GetConnectionIdByWorldId(string worldId)
        {
            if (_instance._clients.TryGetValue(worldId, out var client))
            {
                return client.ConnectionId;
            }

            return -1;
        }

        public static bool HasConnectionId(int connectionId)
        {
            return _instance._reverseLookup.ContainsKey(connectionId);
        }

        public static int[] GetAllClients()
        {
            return _instance._reverseLookup.Keys.ToArray();
        }
    }
}