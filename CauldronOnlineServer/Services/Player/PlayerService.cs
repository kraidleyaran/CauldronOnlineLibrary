using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CauldronOnlineCommon;
using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineServer.Services.Player
{
    public class PlayerService : WorldService
    {
        public const string NAME = "Player";

        public static int PlayerCount => _instance._players.Count;

        private static PlayerService _instance = null;

        private ConcurrentDictionary<string, RegisteredPlayerData> _players = new ConcurrentDictionary<string, RegisteredPlayerData>();

        public override void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                base.Start();
            }
            
        }

        public static string RegisterPlayer(string clientId)
        {
            if (_instance._players.ContainsKey(clientId))
            {
                return _instance._players[clientId].PlayerId;
            }
            else
            {
                var id = GenerateId();
                _instance._players.TryAdd(clientId, new RegisteredPlayerData { PlayerId = id, Zone = string.Empty});
                WorldServer.SendToAllClients(new ClientPlayerRosterUpdateMessage { Player = _instance._players[clientId] });
                return id;
            }
        }

        public static void UnregisterPlayer(string clientId)
        {
            if (_instance._players.TryRemove(clientId, out var data) && data != null)
            {
                WorldServer.SendToAllClients(new ClientPlayerRosterRemoveMessage { PlayerId = data.PlayerId });
            }
            
        }

        public static void UpdatePlayer(string clientId, string displayName, SpriteColorData spriteColor)
        {
            if (_instance._players.TryGetValue(clientId, out var data))
            {
                data.DisplayName = displayName;
                data.SpriteColors = spriteColor;
                WorldServer.SendToAllClients(new ClientPlayerRosterUpdateMessage{Player = data});
            }
        }

        public static void UpdatePlayerPosition(string clientId, string zone, WorldVector2Int pos)
        {
            if (_instance._players.TryGetValue(clientId, out var data))
            {
                data.Position = pos;
                data.Zone = zone;
                WorldServer.SendToAllClients(new ClientPlayerRosterUpdateMessage { Player = data });
            }
        }

        public static RegisteredPlayerData[] GetRoster()
        {
            return _instance._players.Values.ToArray();
        }

        private static string GenerateId()
        {
            var id = $"{NAME}-{Guid.NewGuid().ToString()}";
            while (_instance._players.ContainsKey(id))
            {
                id = $"{NAME}-{Guid.NewGuid().ToString()}";
            }

            return id;
        }
        
    }
}