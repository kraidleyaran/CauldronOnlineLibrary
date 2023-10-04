using System;
using CauldronOnlineServer.Services.Commands;

namespace CauldronOnlineServer
{
    [Serializable]
    public class WorldSettings
    {
        public int Port { get; set; }
        public int MessageTick { get; set; }
        public int ZoneTick { get; set; }
        public string BaseFolder { get; set; }
        public string ZoneSubFolder { get; set; }
        public string TraitSubFolder { get; set; }
        public string LootTableSubFolder { get; set; }
        public string TriggerEventsSubFolder { get; set; }
        public string CombatSettingsSubPath { get; set; }
        public int TileSize { get; set; }
        public WorldCommand[] StartupCommands { get; set; }
        
    }
}