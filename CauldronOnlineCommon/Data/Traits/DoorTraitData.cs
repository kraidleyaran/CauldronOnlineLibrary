using System;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Switches;

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
        public RequiredSwitchSignalData[] Signals;
        public bool RequireAllTriggerEvents;
        public float Rotation;
        public bool AllowOpenWithNoItems;
        public bool ApplyTrapSpawn;
        public WorldVector2Int TrappedSpawnPosition;
    }
}