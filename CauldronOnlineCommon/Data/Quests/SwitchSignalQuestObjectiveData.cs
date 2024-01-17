using System;
using CauldronOnlineCommon.Data.Switches;

namespace CauldronOnlineCommon.Data.Quests
{
    [Serializable]
    public class SwitchSignalQuestObjectiveData : QuestObjectiveData
    {
        public const string TYPE = "SwitchSignal";
        public override string Type => TYPE;

        public RequiredSwitchSignalData[] RequiredSignals;
    }
}