using System;

namespace CauldronOnlineCommon.Data.Quests
{
    [Serializable]
    public class QuestObjectiveData
    {
        public const string DEFAULT = "Default";
        public virtual string Type => DEFAULT;

        public int RequiredAmount;
    }
}