using System.Linq;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Quests;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Quests;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class ZoneQuestTrait : WorldTrait
    {
        private QuestObjective[] _questObjectives = new QuestObjective[0];
        private ZoneQuestParameter _parameter = new ZoneQuestParameter();
        private bool _completed = false;
        private bool _started = false;

        private TickTimer _resetTimer = null;

        

        public ZoneQuestTrait(WorldTraitData data) : base(data)
        {
            if (data is ZoneQuestTraitData zoneQuest)
            {
                _parameter.Objectives = zoneQuest.Objectives;
                _parameter.ApplyOnComplete = zoneQuest.ApplyOnComplete;
                _parameter.TriggerEventOnComplete = zoneQuest.TriggerEventsOnComplete;
                _parameter.Range = zoneQuest.Range;
                _parameter.ResetQuest = zoneQuest.ResetQuest;
                _parameter.ResetTicks = zoneQuest.ResetTicks;
                _parameter.UsePov = zoneQuest.UsePov;
                _parameter.SpawnEvent = zoneQuest.SpawnEvent;
            }
        }

        public ZoneQuestTrait(ZoneQuestParameter parameter)
        {
            Name = parameter.Name;
            _parameter = parameter;
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _questObjectives = QuestService.GetObjectives(_parameter.Objectives);
            SubscribeToMessages();
            if (string.IsNullOrEmpty(_parameter.SpawnEvent))
            {
                StartQuest();
            }
        }

        private void StartQuest()
        {
            _completed = false;
            _started = true;
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                var tiles = _parameter.UsePov ? zone.GetTilesInPovArea(_parent.Tile, _parameter.Range) : zone.GetTilesInSquareArea(_parent.Tile, _parameter.Range);
                foreach (var objective in _questObjectives)
                {
                    objective.SpawnRequiredObjects(zone, tiles, _parent, Name);
                }
            }

        }

        private void ResetTimerFinished()
        {
            WorldServer.Log($"Quest Reset");
            _resetTimer.Destroy();
            _resetTimer = null;
            StartQuest();
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<ApplyObjectiveItemCompletetionMessage>(ApplyObjectiveItemCompletetion, _id);
            if (!string.IsNullOrEmpty(_parameter.SpawnEvent))
            {
                this.SubscribeWithFilter<ZoneEventTriggerMessage>(ZoneEventTrigger, TriggerEventService.GetFilter(_parent.ZoneId, _parameter.SpawnEvent));
            }
        }

        private void ApplyObjectiveItemCompletetion(ApplyObjectiveItemCompletetionMessage msg)
        {
            if (!_completed && _questObjectives.Contains(msg.Objective))
            {
                WorldServer.Log("Objective Item Completed");
                if (msg.Objective.ApplyObjectiveCount(1))
                {
                    var completed = _questObjectives.Count(o => o.CurrentAmount >= o.RequiredAmount);
                    if (completed >= _questObjectives.Length)
                    {
                        WorldServer.Log("Quest Completed");
                        _completed = true;
                        _started = false;
                        var traits = TraitService.GetWorldTraits(_parameter.ApplyOnComplete);
                        foreach (var trait in traits)
                        {
                            _parent.AddTrait(trait);
                        }

                        foreach (var triggerEvent in _parameter.TriggerEventOnComplete)
                        {
                            TriggerEventService.TriggerEvent(triggerEvent);
                        }

                        if (_parameter.ResetQuest)
                        {
                            _resetTimer = new TickTimer(_parameter.ResetTicks.Roll(true), 0, _parent.ZoneId);
                            _resetTimer.OnComplete += ResetTimerFinished;
                        }
                    }
                }
            }
        }

        private void ZoneEventTrigger(ZoneEventTriggerMessage msg)
        {
            if (!_started)
            {
                StartQuest();
            }
        }
    }
}