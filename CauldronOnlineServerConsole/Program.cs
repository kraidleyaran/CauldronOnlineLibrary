using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CauldronOnlineServer;
using CauldronOnlineServer.Services.Commands;
using Newtonsoft.Json;

namespace CauldronOnlineServerConsole
{
    class Program
    {
        private static WorldSettings _settings = new WorldSettings
        {
            BaseFolder = $"C:{Path.DirectorySeparatorChar}CauldronOnline{Path.DirectorySeparatorChar}",
            Port = 42069,
            MessageTick = 10,
            ZoneTick = 100
        };

        private static WorldServer _server = null;
        private static bool _active = true;

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (File.Exists(args[0]))
                {
                    var json = File.ReadAllText(args[0]);
                    try
                    {
                        _settings = JsonConvert.DeserializeObject<WorldSettings>(json);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"**ERROR ** - Exception while loading settings file {ex} {Environment.NewLine}Using default settings");
                    }
                }
                else
                {
                    Console.WriteLine($"**WARNING** - Unable to find World Settings at path {args[0]} - Using default settings");
                }

            }
            else
            {
                Console.WriteLine($"**WARNING** - No settings file path argument - Using default settings");
            }
            _server = new WorldServer(_settings);
            WorldServer.Logging.OnInfo += OnLog;
            WorldServer.Logging.OnWarning += OnLog;
            WorldServer.Logging.OnError += OnLog;
            WorldServer.Logging.OnDebug += OnLog;
            _server.Start(_settings.Port);
            while (_active)
            {
                var input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    var split = input.Split(' ').ToList();
                    var command = split[0];
                    split.RemoveAt(0);
                    CommandService.ProcessCommand(command, split.ToArray());
                }
                Thread.Sleep(100);
            }
        }

        private static void OnLog(string message)
        {
            Console.WriteLine(message);
        }
    }
}
