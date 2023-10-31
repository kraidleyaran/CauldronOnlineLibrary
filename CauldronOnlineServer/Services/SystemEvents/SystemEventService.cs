using CauldronOnlineCommon;
using CauldronOnlineCommon.Data;
using CauldronOnlineServer.Services.Client;
using Newtonsoft.Json;

namespace CauldronOnlineServer.Services.SystemEvents
{
    public class SystemEventService : WorldService
    {
        public const string NAME = "SystemEvent";

        private static SystemEventService _instance = null;

        public override string Name => NAME;

        

        public override void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                base.Start();
            }
        }

        public static void SendMessage(string message, SystemEventType type = SystemEventType.Message)
        {
            var systemEvent = new SystemEvent {Message = message, Type = type};
            _instance.Log(message);
            var clientMsg = new ClientSystemEventMessage {SystemEvent = systemEvent.ToEventData()};
            WorldServer.SendToAllClients(clientMsg);
        }
    }
}