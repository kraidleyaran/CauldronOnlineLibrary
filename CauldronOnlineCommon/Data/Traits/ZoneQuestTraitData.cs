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
        public bool UsePov;
        public string[] ApplyOnComplete;
        public string[] TriggerEventsOnComplete;
        public int CompletionDelay;
        public string[] TriggerEventsOnReset;
        public bool ResetQuest;
        public WorldIntRange ResetTicks;
        public string SpawnEvent;
        public WorldVector2Int[] IgnoreTiles = new WorldVector2Int[0];
    }
}