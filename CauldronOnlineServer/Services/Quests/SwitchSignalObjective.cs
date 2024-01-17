using CauldronOnlineCommon.Data.Quests;
using CauldronOnlineCommon.Data.Switches;
using CauldronOnlineServer.Services.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Quests
{
    public class SwitchSignalObjective : QuestObjective
    {
        public RequiredSwitchSignalData[] RequiredSignals = new RequiredSwitchSignalData[0];

        public SwitchSignalObjective(QuestObjectiveData data) : base(data)
        {
            if (data is SwitchSignalQuestObjectiveData switchData)
            {
                RequiredSignals = switchData.RequiredSignals;
            }
        }

        public override WorldObject[] SpawnRequiredObjects(WorldZone zone, ZoneTile[] tiles, WorldObject questParent, string questName)
        {
            questParent.AddTrait(new QuestSwitchSignalReceiverTrait(questName, this, questParent));
            return base.SpawnRequiredObjects(zone, tiles, questParent, questName);
        }
    }
}