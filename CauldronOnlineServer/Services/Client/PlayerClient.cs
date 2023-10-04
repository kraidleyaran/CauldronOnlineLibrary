using System;
using System.Runtime.InteropServices;
using CauldronOnlineCommon;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineServer.Interfaces;
using CauldronOnlineServer.Services.Combat;
using CauldronOnlineServer.Services.Zones;
using MessageBusLib;

namespace CauldronOnlineServer.Services.Client
{
    public class PlayerClient : IDestroyable
    {
        public int ConnectionId;
        public string WorldId;
        
        public PlayerClient(int connectionId, string worldId)
        {
            ConnectionId = connectionId;
            WorldId = worldId;
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<ClientCreateCharacterRequestMessage>(ClientCreateCharacterRequest, WorldId);
            this.SubscribeWithFilter<ClientWorldSettingsRequestMessage>(ClientWorldSettingsRequest, WorldId);
            this.SubscribeWithFilter<ClientPingMessage>(ClientPing, WorldId);
        }

        private void OnObjectCreated(string id, string zone, WorldVector2Int pos)
        {
            WorldServer.SendToClient(new ClientCreateCharacterResultMessage{Success = true, ObjectId = id, Zone = zone, Position = pos}, ConnectionId);
        }

        private void ClientCreateCharacterRequest(ClientCreateCharacterRequestMessage msg)
        {
            var zone = ZoneService.GetZoneByName(msg.Zone);
            var pos = msg.Position;
            if (zone == null)
            {
                zone = ZoneService.DefaultZone;
                pos = zone.DefaultSpawn;
            }
            zone.ObjectManager.RequestPlayerObject(msg.Data,pos, ConnectionId, WorldId, OnObjectCreated);
        }

        private void ClientWorldSettingsRequest(ClientWorldSettingsRequestMessage msg)
        {
            WorldServer.SendToClient(new ClientWorldSettingsResultMessage{WorldTick = WorldServer.Settings.ZoneTick, CombatSettings = CombatService.Settings}, ConnectionId);
        }

        private void ClientPing(ClientPingMessage msg)
        {
            msg.Timestamp = DateTime.UtcNow;
            WorldServer.SendToClient(msg, ConnectionId);
        }

        public void Destroy()
        {
            this.SendMessageWithFilter(PlayerDisconnectedMessage.INSTANCE, WorldId);
            ConnectionId = -1;
            WorldId = null;
            this.UnsubscribeFromAllMessages();
        }
    }
}