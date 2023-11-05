using System;
using System.Runtime.InteropServices;
using CauldronOnlineCommon;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineServer.Interfaces;
using CauldronOnlineServer.Services.Combat;
using CauldronOnlineServer.Services.Player;
using CauldronOnlineServer.Services.SystemEvents;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

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
            this.SubscribeWithFilter<ClientPlayerRosterRequestMessage>(ClientPlayerRosterRequest, WorldId);
        }

        private void OnObjectCreated(string id, string displayName, string zone, WorldVector2Int pos)
        {
            SystemEventService.SendMessage($"{displayName} has joined the world");
            WorldServer.SendToClient(new ClientCreateCharacterResultMessage{Success = true, ObjectId = id, Zone = zone, Position = pos}, ConnectionId);
            PlayerService.UpdatePlayerPosition(WorldId, zone, pos);
            this.SendMessageWithFilter(PlayerEnteredWorldMessage.INSTANCE, ZoneService.GLOBAL_ZONE_FILTER);
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
            PlayerService.UpdatePlayer(WorldId, msg.Data.DisplayName, msg.Data.Colors);
            zone.ObjectManager.RequestPlayerObject(msg.Data, pos, ConnectionId, WorldId, OnObjectCreated);
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

        private void ClientPlayerRosterRequest(ClientPlayerRosterRequestMessage msg)
        {
            var roster = PlayerService.GetRoster();
            WorldServer.SendToClient(new ClientPlayerRosterResponseMessage{Players = roster, Success = true}, ConnectionId);
        }

        public void Destroy()
        {
            this.SendMessageWithFilter(PlayerDisconnectedMessage.INSTANCE, WorldId);
            ConnectionId = -1;
            PlayerService.UnregisterPlayer(WorldId);
            WorldId = null;
            this.UnsubscribeFromAllMessages();
        }
    }
}