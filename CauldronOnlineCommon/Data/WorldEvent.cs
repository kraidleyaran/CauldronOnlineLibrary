using System;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.Math;
using Newtonsoft.Json;

namespace CauldronOnlineCommon.Data.WorldEvents
{
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class WorldEvent
    {
        public virtual int EventId { get; set; }
        public int Order { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ObjectCreatedEvent : WorldEvent
    {
        public const int ID = 1;
        public override int EventId => ID;
        public ClientObjectData Data { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class MovementEvent : WorldEvent
    {
        public const int ID = 2;
        public override int EventId => ID;

        public string Id { get; set; }
        public int Speed { get; set; }
        public WorldVector2Int Position { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class UpdateCombatStatsEvent : WorldEvent
    {
        public const int ID = 3;
        public override int EventId => ID;

        public string Id { get; set; }
        public CombatStats Stats { get; set; }
        public int Health { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class DamageEvent : WorldEvent
    {
        public const int ID = 4;
        public override int EventId => ID;

        public string OwnerId { get; set; }
        public string TargetId { get; set; }
        public int Amount { get; set; }
        public DamageType Type { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ObjectDeathEvent : WorldEvent
    {
        public const int ID = 5;
        public override int EventId => ID;

        public string OwnerId { get; set; }
        public string Id { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class KnockbackEvent : WorldEvent
    {
        public const int ID = 6;
        public override int EventId => ID;

        public string OwnerId { get; set; }
        public string TargetId { get; set; }
        public int Time { get; set; }
        public WorldVector2Int EndPosition { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class AbilityEvent : WorldEvent
    {
        public const int ID = 7;
        public override int EventId => ID;

        public string OwnerId { get; set; }
        public string TargetId { get; set; }
        public string Ability { get; set; }
        public string[] Ids { get; set; }
        public WorldVector2Int Position { get; set; }
        public WorldVector2Int Direction { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class DestroyObjectEvent : WorldEvent
    {
        public const int ID = 8;
        public override int EventId => ID;
        public string OwnerId { get; set; }
        public string ObjectId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ShootProjectileEvent : WorldEvent
    {
        public const int ID = 9;
        public override int EventId => ID;
        public string Projectile { get; set; }
        public string OwnerId { get; set; }
        public WorldVector2Int Direction { get; set; }
        public WorldVector2Int Position { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class SpawnLootEvent : WorldEvent
    {
        public const int ID = 10;
        public override int EventId => ID;
        public string OwnerId { get; set; }
        public WorldVector2Int Position { get; set; }
        public LootTableData LootTable { get; set; }
        public WorldIntRange Drops { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class RespawnEvent : WorldEvent
    {
        public const int ID = 11;
        public override int EventId => ID;
        public string OwnerId { get; set; }
        public WorldVector2Int Position { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class UpdatePathEvent : WorldEvent
    {
        public const int ID = 12;
        public override int EventId => ID;
        public string OwnerId { get; set; }
        public WorldVector2Int[] Path { get; set; }
        public int Speed { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class HealEvent : WorldEvent
    {
        public const int ID = 13;
        public override int EventId => ID;
        public string OwnerId { get; set; }
        public string TargetId { get; set; }
        public int Amount { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class FullHealEvent : WorldEvent
    {
        public const int ID = 14;
        public override int EventId => ID;
        public string OwnerId { get; set; }
        public string TargetId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class DoorEvent : WorldEvent
    {
        public const int ID = 15;
        public override int EventId => ID;
        public string OwnerId { get; set; }
        public string TargetId { get; set; }
        public bool Open { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class SwitchSignalEvent : WorldEvent
    {
        public const int ID = 16;
        public override int EventId => ID;
        public int Signal { get; set; }
        public string TargetId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class DoorCheckEvent : WorldEvent
    {
        public const int ID = 17;
        public override int EventId => ID;
        public string TargetId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ChestEvent : WorldEvent
    {
        public const int ID = 18;
        public override int EventId => ID;
        public string TargetId { get; set; }
    }
}