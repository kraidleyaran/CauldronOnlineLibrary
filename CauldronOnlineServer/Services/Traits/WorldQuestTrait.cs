using System.Collections.Concurrent;
using System.Collections.Generic;
using CauldronOnlineCommon;
using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Quests;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Items;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class WorldQuestTrait : WorldTrait
    {
        private WorldItemStackData[] _requiredItems = new WorldItemStackData[0];
        private string[] _requiredTriggerEvents = new string[0];
        private string[] _triggerOnComplete = new string[0];
        private string[] _triggerOnStart = new string[0];

        private List<string> _triggeredEvents = new List<string>();

        private ConcurrentQueue<int> _interactionQueue = new ConcurrentQueue<int>();

        private WorldQuestParameter _parameter = new WorldQuestParameter();

        public WorldQuestTrait(WorldTraitData data) : base(data)
        {
            if (data is WorldQuestTraitData worldQuest)
            {
                _requiredItems = worldQuest.RequiredItems;
                _requiredTriggerEvents = worldQuest.RequiredTriggerEvents;
                _triggerOnStart = worldQuest.TriggerEventsOnStarted;
                _triggerOnComplete = worldQuest.TriggerEventsOnComplete;

                _parameter = new WorldQuestParameter
                {
                    QuestName = worldQuest.QuestName,
                    StartingDialogue = worldQuest.StartingDialogue,
                    InProgressDialogue = worldQuest.InProgressDialogue,
                    CompletedDialogue = worldQuest.CompletedDialogue,
                    Hitbox = worldQuest.Hitbox,
                    State = QuestState.Available
                };
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter, _parameter.QuestName);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            foreach (var trigger in _requiredTriggerEvents)
            {
                this.SubscribeWithFilter<ZoneEventTriggerMessage>(msg => TriggerEvent(trigger), TriggerEventService.GetFilter(_parent.ZoneId, trigger));
            }
            this.SubscribeWithFilter<ZoneUpdateTickMessage>(ZoneUpdateTick, _parent.ZoneId);
            _parent.SubscribeWithFilter<PlayerQuestInteractionMessage>(PlayerQuestInteraction, _id);
            
        }

        private void TriggerEvent(string triggerEvent)
        {
            if (!_triggeredEvents.Contains(triggerEvent))
            {
                _triggeredEvents.Add(triggerEvent);
            }
        }

        private void ZoneUpdateTick(ZoneUpdateTickMessage msg)
        {
            if (_interactionQueue.Count > 0)
            {
                while (_interactionQueue.TryDequeue(out var connectionId))
                {
                    if (_parameter.State != QuestState.Completed)
                    {
                        var pass = _requiredTriggerEvents.Length <= 0 || _requiredTriggerEvents.Length == _triggeredEvents.Count;
                        if (pass && _requiredItems.Length > 0)
                        {
                            foreach (var item in _requiredItems)
                            {
                                if (!ItemService.HasKeyItem(item.Item, item.Stack))
                                {
                                    pass = false;
                                    break;
                                }
                            }
                        }

                        var updateState = false;
                        var questState = QuestState.InProgress;
                        if (pass)
                        {
                            _parameter.State = QuestState.Completed;
                            updateState = true;
                            foreach (var trigger in _triggerOnComplete)
                            {
                                TriggerEventService.TriggerEvent(trigger);
                            }

                            questState = QuestState.Completed;
                        }
                        else if (_parameter.State == QuestState.Available)
                        {
                            questState = QuestState.Available;
                            _parameter.State = QuestState.InProgress;
                            if (_triggerOnStart.Length > 0)
                            {
                                foreach (var trigger in _triggerOnStart)
                                {
                                    TriggerEventService.TriggerEvent(trigger);
                                }
                            }
                            updateState = true;
                        }

                        if (updateState)
                        {
                            var zone = ZoneService.GetZoneById(_parent.ZoneId);
                            if (zone != null)
                            {
                                zone.EventManager.RegisterEvent(new WorldQuestUpdateEvent { ObjectId = _parent.Data.Id, State = _parameter.State });
                            }
                        }
                        WorldServer.SendToClient(new ClientQuestInteractionResultMessage { State = questState, ObjectId = _parent.Data.Id }, connectionId);
                    }
                }
            }
            
            
        }

        private void PlayerQuestInteraction(PlayerQuestInteractionMessage msg)
        {
            _interactionQueue.Enqueue(msg.ConnectionId);
        }
    }
}