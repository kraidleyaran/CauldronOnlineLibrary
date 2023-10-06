using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.TriggerEvents;
using CauldronOnlineServer.Logging;
using FileDataLib;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.TriggerEvents
{
    public class TriggerEventService : WorldService
    {
        private static TriggerEventService _instance = null;

        public const string NAME = "TriggerEvent";
        public override string Name => NAME;

        private string _worldEventPath = string.Empty;

        private Dictionary<string, TriggerEventData> _events = new Dictionary<string, TriggerEventData>();
        private ConcurrentDictionary<string, TriggerEvent> _triggered = new ConcurrentDictionary<string, TriggerEvent>();

        public TriggerEventService(string path)
        {
            _worldEventPath = path;
        }

        public override void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                if (Directory.Exists(_worldEventPath))
                {
                    var files = Directory.GetFiles(_worldEventPath, $"*.{TriggerEventData.EXTENSION}");
                    foreach (var file in files)
                    {
                        var result = FileData.LoadData<TriggerEventData>(file);
                        if (result.Success)
                        {
                            if (!_instance._events.ContainsKey(result.Data.Name))
                            {
                                _instance._events.Add(result.Data.Name, result.Data);
                            }
                            else
                            {
                                Log($"Duplicate Trigger Event Detected - {result.Data.Name} - {file}", LogType.Warning);
                            }
                        }
                        else
                        {
                            Log($"Error while loading world event at path {file} - ${(result.HasException ? $"{result.Exception}" : "Unknown error")}");
                        }
                    }
                }
                Log($"Loaded {_events.Count} Trigger Events");
                base.Start();
            }
            
        }

        public static void TriggerEvent(string eventName)
        {
            if (!_instance._triggered.TryGetValue(eventName, out var worldEvent) && _instance._events.TryGetValue(eventName, out var eventData))
            {
                worldEvent = new TriggerEvent {Name = eventData.Name, MaxActivations = eventData.MaxActivations};
                _instance._triggered.TryAdd(eventName, worldEvent);
            }

            if (worldEvent != null && worldEvent.Activations < worldEvent.MaxActivations)
            {
                _instance.Log($"Trigger Activated - {worldEvent.Name}");
                worldEvent.Activations++;
                _instance.SendMessage(new EventTriggeredMessage{Event = worldEvent.Name});
            }
        }

        public static string GetFilter(string zoneId, string eventName)
        {
            return $"{zoneId}-{eventName}";
        }

        public static bool CanActivate(string eventName)
        {
            if (_instance._triggered.TryGetValue(eventName, out var triggerEvent))
            {
                return triggerEvent.MaxActivations <= 0 || triggerEvent.Activations < triggerEvent.MaxActivations;
            }
            return _instance._events.ContainsKey(eventName);
        }
    }
}