using System;
using CauldronOnlineCommon.Data.Zones;

namespace CauldronOnlineCommon.Data.Quests
{
    [Serializable]
    public class EliminateQuestObjectiveData : QuestObjectiveData
    {
        public const string TYPE = "Eliminate";
        public override string Type => TYPE;

        public ZoneSpawnData Template;
    }
}