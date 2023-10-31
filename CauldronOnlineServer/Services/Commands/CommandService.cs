using System;
using System.Collections.Generic;
using System.Linq;
using CauldronOnlineCommon;
using CauldronOnlineServer.Services.SystemEvents;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Commands
{
    public class CommandService : WorldService
    {
        public const string NAME = "Command";

        private static CommandService _instance = null;

        private Dictionary<string, Action<string[]>> _commands = new Dictionary<string, Action<string[]>>
        {
            {"commands",  Commands },
            {"?",  Commands },
            {"setdefaultzone",  SetDefaultZone },
            {"say", Say}
        };

        public override string Name => NAME;

        public override void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                base.Start();
            }
            
        }

        public static void ProcessCommand(string command, string[] args)
        {
            if (_instance._commands.TryGetValue(command.ToLower(), out var action))
            {
                _instance.Log($"Processing {command}");
                action?.Invoke(args);
            }
            else
            {
                _instance.Log($"Unable to find comnand {command} - use commands to see a list of available commands");
            }
        }

        private static void Commands(string[] args)
        {
            var commands = _instance._commands.Keys.ToArray().OrderBy(c => c);
            var printString = string.Empty;
            foreach (var command in commands)
            {
                printString = string.IsNullOrEmpty(printString) ? command : $"{printString}, {command}";
            }
            _instance.Log(printString);
        }

        private static void SetDefaultZone(string[] args)
        {
            if (args.Length > 0)
            {
                var zone = args[0].ToLower();
                ZoneService.SetDefaultZone(zone);
            }
            else
            {
                _instance.Log("Usage: setdefaultzone [zone] - Alias can be used in place of [zone]");
            }
        }

        private static void Say(string[] args)
        {
            if (args.Length > 0)
            {
                SystemEventService.SendMessage(args.ToSingleLineString());
            }
            
        }
    }
}