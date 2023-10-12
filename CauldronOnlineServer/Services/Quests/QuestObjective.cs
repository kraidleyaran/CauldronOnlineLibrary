using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Quests;
using CauldronOnlineServer.Services.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Quests
{
    public class QuestObjective
    {
        public int RequiredAmount;
        public int CurrentAmount;

        public QuestObjective(QuestObjectiveData data)
        {
            RequiredAmount = data.RequiredAmount;
        }

        public virtual bool ApplyObjectiveCount(int count)
        {
            CurrentAmount += count;
            return CurrentAmount >= RequiredAmount;
        }

        public virtual bool IsComplete()
        {
            return true;
        }

        public virtual WorldObject[] SpawnRequiredObjects(WorldZone zone, ZoneTile[] tiles, WorldObject questParent, string questName)
        {
            return new WorldObject[0];
        }
    }
}