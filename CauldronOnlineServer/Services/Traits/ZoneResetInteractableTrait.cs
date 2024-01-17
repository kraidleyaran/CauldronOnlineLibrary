using System.Collections.Concurrent;
using System.Collections.Generic;
using CauldronOnlineCommon;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class ZoneResetInteractableTrait : WorldTrait
    {
        private const string INVALID_ZONE = "Invalid Zone";
        private const string PLAYERS_ZONED_IN = "Cannot reset while players are zoned in";

        private ZoneResetInteractionParameter _parameter = new ZoneResetInteractionParameter();

        private ConcurrentQueue<int> _resetRequests = new ConcurrentQueue<int>();

        private bool _resetProcessed = false;

        public ZoneResetInteractableTrait(ZoneResetInteractionParameter parameter)
        {
            Name = parameter.Type;
            _parameter = parameter;
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<ZoneUpdateTickMessage>(ZoneUpdateTick, _parent.ZoneId);
            _parent.SubscribeWithFilter<ZoneResetRequestMessage>(ZoneResetRequest, _id);
        }

        private void ZoneResetRequest(ZoneResetRequestMessage msg)
        {
            _resetRequests.Enqueue(msg.ConnectionId);
        }

        private void ZoneUpdateTick(ZoneUpdateTickMessage msg)
        {
            if (_resetRequests.Count > 0)
            {
                
                var zone = ZoneService.GetZoneByName(_parameter.Zone);
                var validZone = zone != null;
                var clientZoneResetResultMsg = new ClientResetZoneResultMessage{Success = validZone};
                if (validZone)
                {
                    if (zone.ObjectManager.ContainsPlayers)
                    {
                        validZone = false;
                        clientZoneResetResultMsg.Success = false;
                        clientZoneResetResultMsg.Message = PLAYERS_ZONED_IN;
                    }
                }
                else
                {
                    clientZoneResetResultMsg.Message = $"{INVALID_ZONE}";
                }
                if (validZone)
                {
                    ZoneService.ResetZone(zone.Id);
                }

                while (_resetRequests.TryDequeue(out var connectionId))
                {
                    WorldServer.SendToClient(clientZoneResetResultMsg, connectionId);
                }

            }
        }
    }
}