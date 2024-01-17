using System;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Items;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class WorldQuestTraitData : WorldTraitData
    {
        public const string TYPE = "WorldQuest";
        public override string Type => TYPE;

        public string QuestName;
        public WorldItemStackData[] RequiredItems = new WorldItemStackData[0];
        public string[] RequiredTriggerEvents = new string[0];
        public string[] TriggerEventsOnStarted = new string[0];
        public string[] TriggerEventsOnComplete = new string[0];
        public string[] StartingDialogue = new string[0];
        public string[] InProgressDialogue = new string[0];
        public string[] CompletedDialogue = new string[0];
        public HitboxData Hitbox = null;
    }
}