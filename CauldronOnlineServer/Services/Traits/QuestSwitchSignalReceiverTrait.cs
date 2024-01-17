using System.Collections.Generic;
using CauldronOnlineCommon.Data.Switches;
using CauldronOnlineServer.Services.Quests;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class QuestSwitchSignalReceiverTrait : WorldTrait
    {
        public const string NAME = "QuestSignalReceiver";

        private SwitchSignalObjective _objective = null;
        private List<RequiredSwitchSignalData> _passedSignals = new List<RequiredSwitchSignalData>();

        private WorldObject _questParent = null;

        private bool _passed = false;

        public QuestSwitchSignalReceiverTrait(string questName, SwitchSignalObjective objective, WorldObject questParent)
        {
            Name = $"{questName}-{NAME}";
            _objective = objective;
            _questParent = questParent;
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            foreach (var signal in _objective.RequiredSignals)
            {
                _parent.SubscribeWithFilter<UpdateSignalMessage>(msg => UpdateSignal(msg, signal), SwitchTrait.GenerateFilter(signal.Switch, _parent.ZoneId));
            }            
        }

        private void UpdateSignal(UpdateSignalMessage msg, RequiredSwitchSignalData signal)
        {
            if (signal.Signal == msg.Signal)
            {
                if (!_passedSignals.Contains(signal))
                {
                    _passedSignals.Add(signal);
                    if (_passedSignals.Count == _objective.RequiredSignals.Length && !_passed)
                    {
                        _passed = true;
                        this.SendMessageTo(new ApplyObjectiveItemCompletetionMessage{Objective = _objective, Count = 1}, _questParent);
                    }
                }
            }
            else if (_passedSignals.Contains(signal))
            {
                _passedSignals.Remove(signal);
                if (_passed)
                {
                    this.SendMessageTo(new ApplyObjectiveItemCompletetionMessage { Objective = _objective, Count = -1 }, _questParent);
                }
            }
        }
    }
}