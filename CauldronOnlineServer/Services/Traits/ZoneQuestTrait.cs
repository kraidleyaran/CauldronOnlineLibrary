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
        private TickTimer _completionTimer = null;

        public ZoneQuestTrait(WorldTraitData data) : base(data)
        {
            if (data is ZoneQuestTraitData zoneQuest)
            {
                _parameter.Objectives = zoneQuest.Objectives;
                _parameter.ApplyOnComplete = zoneQuest.ApplyOnComplete;
                _parameter.TriggerEventOnComplete = zoneQuest.TriggerEventsOnComplete;
                _parameter.CompletionDelay = zoneQuest.CompletionDelay;
                _parameter.TriggerEventOnReset = zoneQuest.TriggerEventsOnReset;
                _parameter.Range = zoneQuest.Range;
                _parameter.ResetQuest = zoneQuest.ResetQuest;
                _parameter.ResetTicks = zoneQuest.ResetTicks;
                _parameter.UsePov = zoneQuest.UsePov;
                _parameter.SpawnEvent = zoneQuest.SpawnEvent;
                _parameter.IgnoreTiles = zoneQuest.IgnoreTiles;
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
            foreach (var objective in _questObjectives)
            {
                objective.Reset();
            }
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                var tiles = _parameter.UsePov ? zone.GetTilesInPovArea(_parent.Tile, _parameter.Range) : zone.GetTilesInSquareArea(_parent.Tile, _parameter.Range).Where(t => !t.Blocked && !_parameter.IgnoreTiles.Contains(t.Position - _parent.Tile.Position)).ToArray();
                foreach (var objective in _questObjectives)
                {
                    objective.SpawnRequiredObjects(zone, tiles, _parent, Name);
                }
            }

        }

        private void ResetTimerFinished()
        {
            _resetTimer.Destroy();
            _resetTimer = null;
            if (_parameter.TriggerEventOnReset.Length > 0)
            {
                foreach (var trigger in _parameter.TriggerEventOnReset)
                {
                    TriggerEventService.TriggerEvent(trigger);
                }
            }

            StartQuest();
        }

        private void CompletionDelayFinished()
        {
            _completionTimer.Destroy();
            _completionTimer = null;
            ApplyCompletion();
        }

        private void ApplyCompletion()
        {
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
                if (msg.Objective.ApplyObjectiveCount(msg.Count))
                {
                    var completed = _questObjectives.Count(o => o.CurrentAmount >= o.RequiredAmount);
                    if (completed >= _questObjectives.Length)
                    {
                        _completed = true;
                        _started = false;
                        if (_parameter.CompletionDelay > 0)
                        {
                            _completionTimer = new TickTimer(_parameter.CompletionDelay, 0, _parent.ZoneId);
                            _completionTimer.OnComplete += CompletionDelayFinished;
                        }
                        else
                        {
                            ApplyCompletion();
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

        public override void Destroy()
        {
            if (_completionTimer != null)
            {
                _completionTimer.Destroy();
                _completionTimer = null;
            }

            if (_resetTimer != null)
            {
                _resetTimer.Destroy();
                _resetTimer = null;
            }
            base.Destroy();
        }
    }
}