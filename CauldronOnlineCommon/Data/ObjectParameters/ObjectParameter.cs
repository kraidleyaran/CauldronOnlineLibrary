using System;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Quests;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.Zones;
using Newtonsoft.Json;

namespace CauldronOnlineCommon.Data.ObjectParameters
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class ObjectParameter
    {
        public virtual string Type { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class AggroParameter: ObjectParameter
    {
        public const string TYPE = "Aggro";
        public override string Type => TYPE;

        public int AggroRange { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class CombatStatsParameter : ObjectParameter
    {
        public const string TYPE = "CombatStats";
        public override string Type => TYPE;

        public CombatStats Stats { get; set; }
        public SecondaryStats BonusSecondary { get; set; }
        public CombatVitals Vitals { get; set; }
        public bool Monster { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class HurtboxParameter : ObjectParameter
    {
        public const string TYPE = "Hurtbox";
        public override string Type => TYPE;

        public WorldVector2Int Size { get; set; }
        public WorldVector2Int Offset { get; set; }
        public bool Knockback { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class HitboxParameter : ObjectParameter
    {
        public const string TYPE = "Hitbox";
        public override string Type => TYPE;

        public ApplyHitboxData[] Hitboxes { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class KnockbackReceiverParameter : ObjectParameter
    {
        public const string TYPE = "KnockbackReceiver";
        public override string Type => TYPE;

        public bool ReceiveKnockback { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class AbilityManagerParameter : ObjectParameter
    {
        public const string TYPE = "AbilityManager";
        public override string Type => TYPE;
        public bool ApplyTraitsOnClient { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class ObjectDeathParameter : ObjectParameter
    {
        public const string TYPE = "ObjectDeath";
        public override string Type => TYPE;
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class ObjectPathParameter : ObjectParameter
    {
        public const string TYPE = "ObjectPath";
        public override string Type => TYPE;
        public WorldVector2Int[] Positions { get; set; }
        public int Speed { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class ShopParameter : ObjectParameter
    {
        public const string TYPE = "Shop";
        public override string Type => TYPE;
        public ShopItemData[] Items { get; set; }
        public HitboxData Hitbox { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class TerrainParameter : ObjectParameter
    {
        public const string TYPE = "Terrain";
        public override string Type => TYPE;
        public HitboxData Hitbox { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class DialogueParameter : ObjectParameter
    {
        public const string TYPE = "Dialogue";
        public override string Type => TYPE;
        public string[] Dialogue { get; set; }
        public string ActionText { get; set; }
        public HitboxData Hitbox { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class DoorParameter : ObjectParameter
    {
        public const string TYPE = "Door";
        public override string Type => TYPE;
        public bool Open { get; set; }
        public WorldItemStackData[] RequiredItems { get; set; }
        public HitboxData Hitbox { get; set; }
        public float Rotation { get; set; }
        public string[] TriggerEvents { get; set; }
        public bool AllowOpenWithNoItems { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class TriggerEventHitboxParameter : ObjectParameter
    {
        public const string TYPE = "TriggerEventHitbox";
        public override string Type => TYPE;
        public HitboxData Hitbox { get; set; }
        public string[] TriggerEvents { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class SwitchParameter : ObjectParameter
    {
        public const string TYPE = "Switch";
        public override string Type => TYPE;
        public string Name { get; set; }
        public HitboxData Hitbox { get; set; }
        public string[] Signals { get; set; }
        public int CurrentSignal { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class GroupSpawnParameter : ObjectParameter
    {
        public override string Type => GroupSpawnerTraitData.TYPE;
        public ZoneSpawnData[] Objects { get; set; }
        public int SpawnEvery { get; set; }
        public float ChanceToSpawn { get; set; }
        public float BonusOnMissedChance { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class LootChestParameter : ObjectParameter
    {
        public const string TYPE = "LootChest";
        public override string Type => TYPE;
        public bool Open { get; set; }
        public string LootTable { get; set; }
        public WorldIntRange Drops { get; set; }
        public string OpenSprite { get; set; }
        public string ClosedSprite { get; set; }
        public HitboxData Hitbox { get; set; }
        public bool RefillChest { get; set; }
        public WorldIntRange RefillTicks { get; set; }
        public bool DestroyAfterOpen { get; set; }
        public int DestroyTicks { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class KeyItemChestParameter : ObjectParameter
    {
        public const string TYPE = "KeyItemChest";
        public override string Type => TYPE;
        public bool Open { get; set; }
        public WorldItemStackData Item { get; set; }
        public string OpenSprite { get; set; }
        public string ClosedSprite { get; set; }
        public HitboxData Hitbox { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class ZoneQuestParameter : ObjectParameter
    {
        public const string TYPE = "ZoneQuest";
        public override string Type => TYPE;
        public string Name { get; set; }
        public QuestObjectiveData[] Objectives { get; set; }
        public int Range { get; set; }
        public string[] ApplyOnComplete { get; set; }
        public string[] TriggerEventOnComplete { get; set; }
        public WorldIntRange ResetTicks { get; set; }
    }



}