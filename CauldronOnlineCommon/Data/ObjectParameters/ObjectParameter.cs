using System;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Quests;
using CauldronOnlineCommon.Data.Switches;
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
        public bool AlwaysAggrod { get; set; }
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
        public bool IsBoss { get; set; }
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
        public RestrictedShopItemData[] Restricted { get; set; }
        public HitboxData Hitbox { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class TerrainParameter : ObjectParameter
    {
        public const string TYPE = "Terrain";
        public override string Type => TYPE;
        public HitboxData Hitbox { get; set; }
        public bool IsGround { get; set; }
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
        public RequiredSwitchSignalData[] Signals { get; set; }
        public bool RequireAllEvents { get; set; }
        public bool AllowOpenWithNoItems { get; set; }
        public bool ApplyTrapSpawn { get; set; }
        public WorldVector2Int TrappedSpawnPosition { get; set; }
        
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
        public bool Locked { get; set; }
        public bool LockOnInteract { get; set; }
        public bool CombatInteractable { get; set; }
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
        public bool ApplyStateToChildren { get; set; }
        public string[] AdditionalTraits { get; set; }
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
        public string[] ResetOnEvents { get; set; }
        public bool DestroyOnReset { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class KeyItemChestParameter : ObjectParameter
    {
        public const string TYPE = "KeyItemChest";
        public override string Type => TYPE;
        public bool Open { get; set; }
        public WorldItemStackData Item { get; set; }
        public WorldItemStackData[] RewardToPlayers { get; set; }
        public string OpenSprite { get; set; }
        public string ClosedSprite { get; set; }
        public HitboxData Hitbox { get; set; }
        public string[] ApplyEventsOnOpen { get; set; }
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
        public bool UsePov { get; set; }
        public string[] ApplyOnComplete { get; set; }
        public string[] TriggerEventOnComplete { get; set; }
        public int CompletionDelay { get; set; }
        public bool ResetQuest { get; set; }
        public string[] TriggerEventOnReset { get; set; }
        public WorldIntRange ResetTicks { get; set; }
        public string SpawnEvent { get; set; }
        public WorldVector2Int[] IgnoreTiles { get; set; }
        
        
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class ZoneTransitionParameter : ObjectParameter
    {
        public const string TYPE = "ZoneTransition";
        public override string Type => TYPE;
        public string Zone { get; set; }
        public WorldVector2Int Position { get; set; }
        public float Rotation { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class CrafterParameter : ObjectParameter
    {
        public const string TYPE = "Crafter";
        public override string Type => TYPE;
        public ItemRecipeData[] Recipes { get; set; }
        public HitboxData Hitbox { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class BridgeParameter : ObjectParameter
    {
        public const string TYPE = "Bridge";
        public override string Type => TYPE;
        public string TilemapSprite { get; set; }
        public WorldVector2Int Size { get; set; }
        public bool Active { get; set; }
        public string[] ToggleOnTriggerEvents { get; set; }
        public RequiredSwitchSignalData[] ToggleOnSwitchSignals { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class PlayerParameter : ObjectParameter
    {
        public const string TYPE = "Player";
        public override string Type => TYPE;
        public SpriteColorData Colors { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class DelayedSpawnParameter : ObjectParameter
    {
        public const string TYPE = "DelayedSpawn";
        public override string Type => TYPE;
        public int DelayTicks { get; set; }
        public ZoneSpawnData Spawn { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class MovableParameter : ObjectParameter
    {
        public const string TYPE = "Movable";
        public override string Type => TYPE;
        public int MoveSpeed { get; set; }
        public string OwnerId { get; set; }
        public HitboxData Hitbox { get; set; }
        public HitboxData HorizontalHitbox { get; set; }
        public WorldOffset Offset { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class WalledParameter : ObjectParameter
    {
        public const string TYPE = "Walled";
        public override string Type => TYPE;
        public HitboxData Hitbox { get; set; }
        public bool IgnoreGround { get; set; }
        public bool CheckForPlayer { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class MovementParameter : ObjectParameter
    {
        public const string TYPE = "Movement";
        public override string Type => TYPE;
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class MovableHelperParameter : ObjectParameter
    {
        public const string TYPE = "MovableHelper";
        public override string Type => TYPE;
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class ProjectileRedirectParameter : ObjectParameter
    {
        public const string TYPE = "ProjectileRedirect";
        public WorldVector2Int Direction { get; set; }
        public HitboxData Hitbox { get; set; }
        public string[] Tags { get; set; }
        public string ProjectileDirectionIcon { get; set; }
        public override string Type => TYPE;

    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class BombableDoorParameter : ObjectParameter
    {
        public const string TYPE = "BombableDoor";
        public HitboxData Hitbox { get; set; }
        public bool Open { get; set; }
        public int BombingExperience { get; set; }
        public override string Type => TYPE;
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class StashParameter : ObjectParameter
    {
        public const string TYPE = "Stash";
        public HitboxData Hitbox { get; set; }
        public override string Type => TYPE;
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class TilemapParameter : ObjectParameter
    {
        public const string TYPE = "Tilemap";
        public string Tilemap { get; set; }
        public override string Type => TYPE;
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class CullableParameter : ObjectParameter
    {
        public const string TYPE = "Cullable";
        public override string Type => TYPE;
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class WorldQuestParameter : ObjectParameter
    {
        public const string TYPE = "WorldQuest";
        public override string Type => TYPE;
        public string QuestName { get; set; }
        public string[] StartingDialogue { get; set; }
        public string[] InProgressDialogue { get; set; }
        public string[] CompletedDialogue { get; set; }
        public QuestState State { get; set; }
        public HitboxData Hitbox { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class BossParameter : ObjectParameter
    {
        public const string TYPE = "Boss";
        public override string Type => TYPE;
        public string DisplayName { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class DroppedItemParameter : ObjectParameter
    {
        public const string TYPE = "DroppedItem";
        public override string Type => TYPE;
        public string Item { get; set; }
        public int Stack { get; set; }
        public string ItemId { get; set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class ZoneResetInteractionParameter : ObjectParameter
    {
        public const string TYPE = "ZoneResetInteraction";
        public override string Type => TYPE;
        public WorldItemStackData[] RequiredItems { get; set; }
        public HitboxData Hitbox { get; set; }
        public string Zone { get; set; }
    }

}