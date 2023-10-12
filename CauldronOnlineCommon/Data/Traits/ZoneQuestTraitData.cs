using System;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Quests;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class ZoneQuestTraitData : WorldTraitData
    {
        public const string TYPE = "ZoneQuest";
        public override string Type => TYPE;

        public QuestObjectiveData[] Objectives;
        public int Range;
        public string[] ApplyOnComplete;
        public string[] TriggerEventsOnComplete;
        public WorldIntRange ResetTicks;
    }
}