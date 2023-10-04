using System.Collections.Concurrent;
using System.Collections.Generic;
using CauldronOnlineServer.Services.TriggerEvents;
using MessageBusLib;

namespace CauldronOnlineServer.Services.Zones.Managers
{
    public class TriggerEventManager : WorldManager
    {
        private ConcurrentQueue<string> _eventQueue = new ConcurrentQueue<string>();

        private string _zoneId = string.Empty;

        public TriggerEventManager(string zoneId)
        {
            _zoneId = zoneId;
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            this.Subscribe<EventTriggeredMessage>(EventTriggered);
            this.SubscribeWithFilter<ZoneUpdateTickMessage>(ZoneUpdateTick, _zoneId);
        }

        private void ZoneUpdateTick(ZoneUpdateTickMessage msg)
        {
            if (_eventQueue.Count > 0)
            {
                while (_eventQueue.TryDequeue(out var triggerEvent))
                {
                    this.SendMessageWithFilter(ZoneEventTriggerMessage.INSTANCE, TriggerEventService.GetFilter(_zoneId, triggerEvent));
                }
            }
        }

        private void EventTriggered(EventTriggeredMessage msg)
        {
            _eventQueue.Enqueue(msg.Event);
        }
    }
}