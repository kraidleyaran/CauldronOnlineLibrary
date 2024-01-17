using System;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Quests;
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
        public bool ShowAppearance { get; set; }
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
        public WorldVector2Int Direction { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class UpdateCombatStatsEvent : WorldEvent
    {
        public const int ID = 3;
        public override int EventId => ID;

        public string OwnerId { get; set; }
        public CombatStats Stats { get; set; }
        public SecondaryStats Secondary { get; set; }
        public CombatVitals Vitals { get; set; }
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
        public bool IsMonster { get; set; }
        public int Players { get; set; }
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
        public bool Locked { get; set; }
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
    public class ChestOpenEvent : WorldEvent
    {
        public const int ID = 18;
        public override int EventId => ID;
        public string TargetId { get; set; }
        public string PlayerName { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ChestRefillEvent : WorldEvent
    {
        public const int ID = 19;
        public override int EventId => ID;
        public string TargetId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class TeleportEvent : WorldEvent
    {
        public const int ID = 20;
        public override int EventId => ID;
        public string ObjectId { get; set; }
        public WorldVector2Int Position { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class KeyItemLootEvent : WorldEvent
    {
        public const int ID = 21;
        public override int EventId => ID;
        public string TargetId { get; set; }
        public string Item { get; set; }
        public int Stack { get; set; }
        public string PlayerName { get; set; }
        public WorldVector2Int Position { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ObjectStateEvent : WorldEvent
    {
        public const int ID = 22;
        public override int EventId => ID;
        public string TargetId { get; set; }
        public bool Active { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class BridgeStateEvent : WorldEvent
    {
        public const int ID = 23;
        public override int EventId => ID;
        public bool Active { get; set; }
        public string TargetId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class TimerEvent : WorldEvent
    {
        public const int ID = 24;
        public override int EventId => ID;
        public string TargetId { get; set; }
        public WorldVector2Int Position { get; set; }
        public int Ticks { get; set; }
        public int Loops { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class MovableEvent : WorldEvent
    {
        public const int ID = 25;
        public override int EventId => ID;
        public string OwnerId { get; set; }
        public string MovableId { get; set; }
        public MovableType Type { get; set; }
        public WorldVector2Int OwnerPosition { get; set; }
        public WorldVector2Int MovablePosition { get; set; }
        public int Speed { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class RollEvent : WorldEvent
    {
        public const int ID = 26;
        public override int EventId => ID;
        public string OwnerId { get; set; }
        public int Speed { get; set; }
        public WorldVector2Int Position { get; set; }
        public WorldVector2Int Direction { get; set; }
        public bool Finished { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ReturnToOwnerEvent : WorldEvent
    {
        public const int ID = 27;
        public override int EventId => ID;
        public string OwnerId { get; set; }
        public string TargetId { get; set; }
        public int DetectDistance { get; set; }
        public WorldVector2Int DetectOffset { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class HasReturnedToOwnerEvent : WorldEvent
    {
        public const int ID = 28;
        public override int EventId => ID;
        public string TargetId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class PlayerEnteredWorldEvent : WorldEvent
    {
        public const int ID = 29;
        public override int EventId => ID;
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class PlayerLeftWorldEvent : WorldEvent
    {
        public const int ID = 30;
        public override int EventId => ID;
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class VisualFxEvent : WorldEvent
    {
        public const int ID = 31;
        public override int EventId => ID;

        public string Name { get; set; }
        public WorldVector2Int Position { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class WorldQuestUpdateEvent : WorldEvent
    {
        public const int ID = 32;
        public override int EventId => ID;

        public string ObjectId { get; set; }
        public QuestState State { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class PlayerDroppedItemEvent : WorldEvent
    {
        public const int ID = 33;
        public override int EventId => ID;

        public string Item { get; set; }
        public int Stack { get; set; }
        public WorldVector2Int Position { get; set; }
        public string ObjectId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class PlayerClaimItemEvent : WorldEvent
    {
        public const int ID = 34;
        public override int EventId => ID;

        public string OwnerId { get; set; }
        public string ObjectId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class MinimapExplorationUpdateEvent : WorldEvent
    {
        public const int ID = 35;
        public override int EventId => ID;

        public WorldVector2Int[] Positions { get; set; }
    }
}