using System;
using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Quests;
using CauldronOnlineCommon.Data.WorldEvents;
using ConcurrentMessageBus;
using Newtonsoft.Json;

namespace CauldronOnlineCommon
{
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientMessage : EventMessage
    {
        public string ClientId { get; set; }
        public virtual int MessageId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientMultiPartMessage : ClientMessage
    {
        public const int ID = 1;
        public override int MessageId => ID;
        public int Id { get; set; }
        public byte[] Part { get; set; }
        public int PartOrder { get; set; }
        public string MultiPartId { get; set; }
        public int TotalParts { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientConnectRequestMessage : ClientMessage
    {
        public const int ID = 2;
        public override int MessageId => ID;
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientConnectResultMessage : ClientMessage
    {
        public const int ID = 3;
        public override int MessageId => ID;
        public bool Success { get; set; }
        public string Message { get; set; }
        public string PlayerId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientCreateCharacterRequestMessage : ClientMessage
    {
        public const int ID = 4;
        public override int MessageId => ID;
        public ClientCharacterData Data { get; set; }
        public string Zone { get; set; }
        public WorldVector2Int Position { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientCreateCharacterResultMessage : ClientMessage
    {
        public const int ID = 5;
        public override int MessageId => ID;
        public bool Success { get; set; }
        public string ObjectId { get; set; }
        public string Zone { get; set; }
        public WorldVector2Int Position { get; set; }
        public string Message { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientWorldEventsUpdateMessage : ClientMessage
    {
        public const int ID = 6;
        public override int MessageId => ID;
        public byte[][] Events { get; set; }
    }


    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientMovementUpdateMessage : ClientMessage
    {
        public const int ID = 7;
        public override int MessageId => ID;
        public WorldVector2Int Position { get; set; }
        public WorldVector2Int Direction { get; set; }
        public int Speed { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientObjectRequestMessage : ClientMessage
    {
        public const int ID = 8;
        public override int MessageId => ID;
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientObjectResultMessage : ClientMessage
    {
        public const int ID = 9;
        public override int MessageId => ID;
        public bool Success { get; set; }
        public ClientObjectData[] Data { get; set; }
        public string Message { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientWorldSettingsRequestMessage : ClientMessage
    {
        public const int ID = 10;
        public override int MessageId => ID;
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientWorldSettingsResultMessage : ClientMessage
    {
        public const int ID = 11;
        public override int MessageId => ID;
        public int WorldTick { get; set; }
        public CombatSettings CombatSettings { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientZoneTickMessage : ClientMessage
    {
        public const int ID = 12;
        public override int MessageId => ID;
        public WorldTick Tick { get; set; }
    }


    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientAggroMessage : ClientMessage
    {
        public const int ID = 13;
        public override int MessageId => ID;
        public string AggrodObjectId { get; set; }
        public bool Remove { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientDamageMessage : ClientMessage
    {
        public const int ID = 14;
        public override int MessageId => ID;
        public string OwnerId { get; set; }
        public string TargetId { get; set; }
        public int Amount { get; set; }
        public DamageType Type { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientKnockbackMessage : ClientMessage
    {
        public const int ID = 15;
        public override int MessageId => ID;
        public string OwnerId { get; set; }
        public string TargetId { get; set; }
        public int Time { get; set; }
        public WorldVector2Int EndPosition { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientUseAbilityMessage : ClientMessage
    {
        public const int ID = 16;
        public override int MessageId => ID;
        public string Ability { get; set; }
        public WorldVector2Int Position { get; set; }
        public WorldVector2Int Direction { get; set; }
        public string[] Ids { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientPingMessage : ClientMessage
    {
        public const int ID = 17;
        public override int MessageId => ID;
        public DateTime Timestamp { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientDeathMessage : ClientMessage
    {
        public const int ID = 18;
        public override int MessageId => ID;
        public string Target { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientShootProjectileMessage : ClientMessage
    {
        public const int ID = 19;
        public override int MessageId => ID;
        public string Projectile { get; set; }
        public WorldVector2Int Direction { get; set; }
        public WorldVector2Int Position { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientDestroyObjectMessage : ClientMessage
    {
        public const int ID = 20;
        public override int MessageId => ID;
        public string TargetId { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientZoneTransferRequestMessage : ClientMessage
    {
        public const int ID = 21;
        public override int MessageId => ID;
        public ClientCharacterData Data { get; set; }
        public string Zone { get; set; }
        public WorldVector2Int Position { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientZoneTransferResultMessage : ClientMessage
    {
        public const int ID = 22;
        public override int MessageId => ID;
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Zone { get; set; }
        public WorldVector2Int Position { get; set; }
        public string ObjectId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientRespawnRequestMessage : ClientMessage
    {
        public const int ID = 23;
        public override int MessageId => ID;
        public ClientCharacterData Data { get; set; }
        public string Zone { get; set; }
        public WorldVector2Int Position { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientRespawnResultMessage : ClientMessage
    {
        public const int ID = 24;
        public override int MessageId => ID;
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ObjectId { get; set; }
        public string Zone { get; set; }
        public WorldVector2Int Position { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientExperienceMessage : ClientMessage
    {
        public const int ID = 25;
        public override int MessageId => ID;
        public int Amount { get; set; }
        public int Players { get; set; }
        public string OriginId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientOpenDoorMessage : ClientMessage
    {
        public const int ID = 26;
        public override int MessageId => ID;
        public string DoorId { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientTriggerEventMessage : ClientMessage
    {
        public const int ID = 27;
        public override int MessageId => ID;
        public string[] TriggerEvents { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientHealMessage : ClientMessage
    {
        public const int ID = 28;
        public override int MessageId => ID;
        public string OwnerId { get; set; }
        public string TargetId { get; set; }
        public int Amount { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientSwitchSignalMessage : ClientMessage
    {
        public const int ID = 29;
        public override int MessageId => ID;
        public string TargetId { get; set; }
        public int Signal { get; set; }
        public bool Locked { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientAddKeyItemMessage : ClientMessage
    {
        public const int ID = 30;
        public override int MessageId => ID;
        public string Item { get; set; }
        public int Stack { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientDoorCheckMessage : ClientMessage
    {
        public const int ID = 31;
        public override int MessageId => ID;
        public string TargetId { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientOpenChestMessage : ClientMessage
    {
        public const int ID = 32;
        public override int MessageId => ID;
        public string TargetId { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientTeleportMessage : ClientMessage
    {
        public const int ID = 33;
        public override int MessageId => ID;
        public WorldVector2Int Position { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientItemRewardMessage : ClientMessage
    {
        public const int ID = 34;
        public override int MessageId => ID;
        public WorldItemStackData[] Items { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientProjectileMovementUpdateMessage : ClientMessage
    {
        public const int ID = 35;
        public override int MessageId => ID;
        public string TargetId { get; set; }
        public WorldVector2Int Position { get; set; }
        public int Speed { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientSystemEventMessage : ClientMessage
    {
        public const int ID = 36;
        public override int MessageId => ID;
        public byte[] SystemEvent { get; set; }
    }


    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientPlayerLeveledMessage : ClientMessage
    {
        public const int ID = 37;
        public override int MessageId => ID;
        public int Level { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientMovableUpdateMessage : ClientMessage
    {
        public const int ID = 38;
        public override int MessageId => ID;
        public MovableType Type { get; set; }
        public string MovableId { get; set; }
        public WorldVector2Int Position { get; set; }
        public WorldVector2Int MovablePosition { get; set; }
        public int MoveSpeed { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientRollUpdateMessage : ClientMessage
    {
        public const int ID = 39;
        public override int MessageId => ID;
        public WorldVector2Int Position { get; set; }
        public WorldVector2Int Direction { get; set; }
        public int MoveSpeed { get; set; }
        public bool Finished { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientPlayerRosterRequestMessage : ClientMessage
    {
        public const int ID = 40;
        public override int MessageId => ID;
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientPlayerRosterResponseMessage : ClientMessage
    {
        public const int ID = 41;
        public override int MessageId => ID;
        public RegisteredPlayerData[] Players { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientPlayerRosterUpdateMessage : ClientMessage
    {
        public const int ID = 42;
        public override int MessageId => ID;
        public RegisteredPlayerData Player { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientReturnToOwnerMessage : ClientMessage
    {
        public const int ID = 43;
        public override int MessageId => ID;
        public string TargetId { get; set; }
        public int DetectDistance { get; set; }
        public WorldVector2Int DetectOffset { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientProjectileReturnedMessage : ClientMessage
    {
        public const int ID = 44;
        public override int MessageId => ID;
        public string TargetId { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientUpdateCombatStatsMessage : ClientMessage
    {
        public const int ID = 45;
        public override int MessageId => ID;
        public CombatStats Stats { get; set; }
        public SecondaryStats Secondary { get; set; }
        public CombatVitals Vitals { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientQuestInteractionRequestMessage : ClientMessage
    {
        public const int ID = 46;
        public override int MessageId => ID;
        public string ObjectId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientQuestInteractionResultMessage : ClientMessage
    {
        public const int ID = 47;
        public override int MessageId => ID;
        public QuestState State { get; set; }
        public string ObjectId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientDropItemMessage : ClientMessage
    {
        public const int ID = 48;
        public override int MessageId => ID;
        public string Item { get; set; }
        public int Stack { get; set; }
        public WorldVector2Int Position { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientClaimItemRequestMessage : ClientMessage
    {
        public const int ID = 49;
        public override int MessageId => ID;
        public string ObjectId { get; set; }
        public WorldTick Tick { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientClaimItemResultMessage : ClientMessage
    {
        public const int ID = 50;
        public override int MessageId => ID;
        public bool Success { get; set; }
        public string ObjectId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientExplorePositionsMessage : ClientMessage
    {
        public const int ID = 51;
        public override int MessageId => ID;
        public WorldVector2Int[] Positions { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientPlayerRosterRemoveMessage : ClientMessage
    {
        public const int ID = 52;
        public override int MessageId => ID;
        public string PlayerId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientResetZoneRequestMessage : ClientMessage
    {
        public const int ID = 53;
        public override int MessageId => ID;
        public string ObjectId { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientResetZoneResultMessage : ClientMessage
    {
        public const int ID = 54;
        public override int MessageId => ID;
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}