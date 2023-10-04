using System;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Items;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class DoorTraitData : WorldTraitData
    {
        public const string TYPE = "Door";
        public override string Type => TYPE;

        public bool Open;
        public WorldItemStackData[] RequiredItems;
        public HitboxData Hitbox;
        public string[] TriggerEvents;
        public float Rotation;
        public bool AllowOpenWithNoItems;
    }
}