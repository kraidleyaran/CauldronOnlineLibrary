using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CauldronOnlineCommon;
using CauldronOnlineServer.Logging;
using CauldronOnlineServer.Services;
using CauldronOnlineServer.Services.Client;
using CauldronOnlineServer.Services.Combat;
using CauldronOnlineServer.Services.Commands;
using CauldronOnlineServer.Services.Items;
using CauldronOnlineServer.Services.Player;
using CauldronOnlineServer.Services.Quests;
using CauldronOnlineServer.Services.SystemEvents;
using CauldronOnlineServer.Services.Traits;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;
using Telepathy;

namespace CauldronOnlineServer
{
    public class WorldServer
    {
        public static WorldSettings Settings => _instance._settings;
        public static WorldLogging Logging => _instance._logging;

        private static WorldServer _instance = null;

        private const string SERVER = "SERVER";

        private Server _server = null;
        private WorldLogging _logging = null;
        private WorldSettings _settings = null;
        private WorldService[] _services = new WorldService[0];

        private Thread _messagingThread = null;
        private bool _active = false;

        public WorldServer(WorldSettings settings)
        {
            _instance = this;
            _settings = settings;
            _logging = new WorldLogging($"{_settings.BaseFolder}{Path.DirectorySeparatorChar}{_settings.LogSubFolder}");
            _services = new WorldService[]
            {
                new SystemEventService(),
                new ClientService(),
                new RNGService(),
                new QuestService(),
                new CombatService($"{_settings.BaseFolder}{Path.DirectorySeparatorChar}{_settings.CombatSettingsSubPath}"), 
                new TraitService($"{_settings.BaseFolder}{Path.DirectorySeparatorChar}{_settings.TraitSubFolder}"),
                new TriggerEventService($"{_settings.BaseFolder}{Path.DirectorySeparatorChar}{_settings.TriggerEventsSubFolder}"),
                new ZoneService($"{_settings.BaseFolder}{Path.DirectorySeparatorChar}{_settings.ZoneSubFolder}"),
                new ItemService($"{_settings.BaseFolder}{Path.DirectorySeparatorChar}{_settings.LootTableSubFolder}"),
                new CommandService(),
                new PlayerService(),
            };
        }


        public bool Start(int port)
        {
            MessageBus.InitializeTypes(Assembly.GetExecutingAssembly());
            var typeCount = MessageBus.InitializeTypes(Assembly.GetAssembly(typeof(ClientMessage)), typeof(ClientMessage));
            _server = new Server{MaxMessageSize = CauldronUtils.MAX_MESSAGE_SIZE};
            _active = true;
            _messagingThread = new Thread(CheckMessages);
            _messagingThread.Start();
            this.Subscribe<ZonesFinishedMessage>(ZonesFinished);
            foreach (var service in _services)
            {
                service.Start();
            }

            Log($"{typeCount} Types registered");
            return _server.Start(port);
        }

        public void Stop()
        {
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }

            foreach (var service in _services)
            {
                service.Stop();
            }
            _active = false;

        }

        private void CheckMessages()
        {
            while (_active)
            {
                while (_server.GetNextMessage(out var message))
                {
                    switch (message.eventType)
                    {
                        case EventType.Connected:
                            Log($"Client connected - {message.connectionId} - {_server.GetClientAddress(message.connectionId)}");
                            ClientService.RegisterClient(message.connectionId);
                            break;
                        case EventType.Data:
                            var clientMsg = message.data.GenerateMessageFromData();
                            if (!string.IsNullOrEmpty(clientMsg.ClientId) && ClientService.IsValidWorldId(message.connectionId, clientMsg.ClientId))
                            {
                                this.SendMessageWithFilter(clientMsg, clientMsg.ClientId);
                            }
                            else
                            {
                                Log($"Invalid ClientId for message from {message.connectionId} - ClientId:{clientMsg.ClientId}");
                            }
                            break;
                        case EventType.Disconnected:
                            Log($"Client Disconnected - {message.connectionId} - {_server.GetClientAddress(message.connectionId)}");
                            ClientService.UnregisterClient(message.connectionId);
                            break;
                    }
                }
                Thread.Sleep(_settings.MessageTick);
            }

            _messagingThread = null;
        }

        private void ZonesFinished(ZonesFinishedMessage msg)
        {
            if (_settings.StartupCommands != null)
            {
                foreach (var command in _settings.StartupCommands)
                {
                    CommandService.ProcessCommand(command.Command, command.Args);
                }
            }
            this.Unsubscribe<ZonesFinishedMessage>();
        }

        public static void Log(string message, LogType type = LogType.Info)
        {
            var msg = $"[{SERVER}] - {message}";
            switch (type)
            {
                case LogType.Info:
                    _instance._logging.Info(msg);
                    break;
                case LogType.Warning:
                    _instance._logging.Warning(msg);
                    break;
                case LogType.Error:
                    _instance._logging.Error(msg);
                    break;
                case LogType.Debug:
                    _instance._logging.Debug(msg);
                    break;
            }
        }

        public static void SendToClient<T>(T clientMsg, int connectionId) where T : ClientMessage
        {
            if (ClientService.HasConnectionId(connectionId))
            {
                clientMsg.Sender = null;
                var data = clientMsg.ToByteArray();
                if (data.Length > _instance._server.MaxMessageSize)
                {
                    var multiParts = data.ToMultiPart(_instance._server.MaxMessageSize, clientMsg.MessageId);
                    foreach (var message in multiParts)
                    {
                        _instance._server.Send(connectionId, message.ToByteArray());
                    }
                }
                else
                {
                    _instance._server.Send(connectionId, clientMsg.ToByteArray());
                }
                
            }
        }

        public static void SendToClient<T>(T clientMsg, string worldId) where T : ClientMessage
        {
            var connectionId = ClientService.GetConnectionIdByWorldId(worldId);
            if (connectionId >= 0)
            {
                var data = clientMsg.ToByteArray();
                if (data.Length > _instance._server.MaxMessageSize)
                {
                    var multiParts = data.ToMultiPart(_instance._server.MaxMessageSize, clientMsg.MessageId);
                    foreach (var message in multiParts)
                    {
                        _instance._server.Send(connectionId, message.ToByteArray());
                    }
                }
                else
                {
                    _instance._server.Send(connectionId, clientMsg.ToByteArray());
                }
            }
        }

        public static void SendToClient(byte[] data, int connectionId)
        {
            if (ClientService.HasConnectionId(connectionId))
            {
                _instance._server.Send(connectionId, data);
            }
        }

        public static void SendToAllClients<T>(T message) where T : ClientMessage
        {
            var clients = ClientService.GetAllClients();
            var data = message.ToByteArray();
            foreach (var client in clients)
            {
                _instance._server.Send(client, data);
            }
        }
    }
}
