using System;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.Math;

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
        public bool RequireAllTriggerEvents;
        public float Rotation;
        public bool AllowOpenWithNoItems;
        public WorldVector2Int TrappedSpawnPosition;
    }
}