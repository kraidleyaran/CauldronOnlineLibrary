using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CauldronOnlineCommon.Data.Zones;
using CauldronOnlineServer.Logging;
using FileDataLib;
using MessageBusLib;

namespace CauldronOnlineServer.Services.Zones
{
    public class ZoneService : WorldService
    {
        public static WorldZone DefaultZone { get; private set; }

        public const string NAME = "Zone";
        public const string ZONE = "Zone";

        private static ZoneService _instance = null;

        public override string Name => NAME;

        private string _zoneDataPath = string.Empty;

        private Dictionary<string, WorldZone> _zones = new Dictionary<string, WorldZone>();
        private Dictionary<string, WorldZone> _reverseLookup = new Dictionary<string, WorldZone>();

        private Thread _zoneGenerationThread = null;

        public ZoneService(string zoneDataPath)
        {
            _zoneDataPath = zoneDataPath;
        }

        public override void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                _zoneGenerationThread = new Thread(GenerateZones);
                _zoneGenerationThread.Start();
                base.Start();
            }

        }

        public override void Stop()
        {
            var zones = _zones.Values.ToArray();
            foreach (var zone in zones)
            {
                zone.Destroy();
            }
            _zones = new Dictionary<string, WorldZone>();
            _reverseLookup = new Dictionary<string, WorldZone>();
            base.Stop();
        }

        private string GenerateId()
        {
            var id = $"{ZONE}-{Guid.NewGuid().ToString()}";
            while (_zones.ContainsKey(id))
            {
                id = $"{ZONE}-{Guid.NewGuid().ToString()}";
            }

            return id;
        }

        public static WorldZone GetZoneById(string id)
        {
            if (_instance._zones.TryGetValue(id, out var zone))
            {
                return zone;
            }

            return null;
        }

        public static WorldZone GetZoneByName(string name)
        {
            if (_instance._reverseLookup.TryGetValue(name, out var zone))
            {
                return zone;
            }

            return null;
        }

        public static void SetDefaultZone(string zone)
        {
            if (_instance._reverseLookup.TryGetValue(zone, out var worldZone))
            {
                DefaultZone = worldZone;
                _instance.Log($"Default zone set to {worldZone.Name} - {worldZone.Id} - {worldZone.DefaultSpawn}");
            }
            else
            {
                _instance.Log($"Unable to Set Default Zone - {zone} not found", LogType.Warning);
            }
        }

        private void GenerateZones()
        {
            var zoneCount = 0;
            if (Directory.Exists(_zoneDataPath))
            {
                var files = Directory.GetFiles(_zoneDataPath, $"*.{WorldZoneData.EXTENSION}");
                zoneCount = files.Length;
                var generating = false;
                this.Subscribe<ZoneCheckInMessage>(msg =>
                {
                    Log($"Zone Check In - {msg.Zone} - {_zones.Count} of {files.Length} - {DateTime.Now:HH:mm:ss.fff}");
                    generating = false;
                });
                foreach (var file in files)
                {
                    generating = true;
                    var result = FileData.LoadData<WorldZoneData>(file);
                    if (result.Success)
                    {
                        var id = GenerateId();
                        var zone = new WorldZone(result.Data, id);
                        _zones.Add(id, zone);
                        _reverseLookup.Add(result.Data.Name, zone);
                        foreach (var alias in result.Data.Aliases)
                        {
                            if (!_reverseLookup.ContainsKey(alias))
                            {
                                _reverseLookup.Add(alias, zone);
                            }
                            else
                            {
                                Log($"Duplicate alias detected for zone {zone.Name} - Alias: {alias}", LogType.Warning);
                            }
                        }
                        Log($"Zone generated for {result.Data.Name} - {zone.Id}");
                    }

                    while (generating)
                    {
                        Thread.Sleep(25);
                    }
                }
                
            }
            Log($"{_zones.Count} of {zoneCount} zones created");
            this.SendMessage(ZonesFinishedMessage.INSTANCE);
            _zoneGenerationThread = null;
        }

        
    }
}