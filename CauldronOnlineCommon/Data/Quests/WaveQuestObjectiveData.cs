using System;

namespace CauldronOnlineCommon.Data.Quests
{
    [Serializable]
    public class WaveQuestObjectiveData : QuestObjectiveData
    {
        public const string TYPE = "Wave";
        public override string Type => TYPE;

        public WaveData[] Waves = new WaveData[0];
        public int TicksBetweenWaves = 0;
    }
}