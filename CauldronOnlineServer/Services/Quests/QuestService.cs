using System.Collections.Generic;
using CauldronOnlineCommon.Data.Quests;

namespace CauldronOnlineServer.Services.Quests
{
    public class QuestService : WorldService
    {
        public const string NAME = "Quest";

        private static QuestService _instance = null;

        public override string Name => NAME;

        public override void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                base.Start();
            }
            
        }

        public static QuestObjective ToQuestObjective(QuestObjectiveData data)
        {
            switch (data.Type)
            {
                case EliminateQuestObjectiveData.TYPE:
                    return new EliminateObjective(data);
                case WaveQuestObjectiveData.TYPE:
                    return new WaveObjective(data);
                case SwitchSignalQuestObjectiveData.TYPE:
                    return new SwitchSignalObjective(data);
                default:
                    return new QuestObjective(data);
            }
        }

        public static QuestObjective[] GetObjectives(QuestObjectiveData[] objectives)
        {
            var returnObjectives = new List<QuestObjective>();
            foreach (var objective in objectives)
            {
                returnObjectives.Add(ToQuestObjective(objective));
            }

            return returnObjectives.ToArray();
        }
    }
}