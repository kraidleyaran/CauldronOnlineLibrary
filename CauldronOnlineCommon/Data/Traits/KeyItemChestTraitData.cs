using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Items;

namespace CauldronOnlineCommon.Data.Traits
{
    public class KeyItemChestTraitData : WorldTraitData
    {
        public const string TYPE = "KeyItemChest";
        public override string Type => TYPE;

        public WorldItemStackData Item;
        public string OpenSprite { get; set; }
        public string ClosedSprite { get; set; }
        public HitboxData Hitbox { get; set; }
        public string[] ApplyTriggerEventsOnOpen { get; set; }
    }
}