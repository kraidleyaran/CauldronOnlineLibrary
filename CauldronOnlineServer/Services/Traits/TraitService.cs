using System.Collections.Generic;
using System.IO;
using CauldronOnlineCommon.Data.Traits;
using FileDataLib;

namespace CauldronOnlineServer.Services.Traits
{
    public class TraitService : WorldService
    {
        private static TraitService _instance = null;

        public const string NAME = "Trait";

        public override string Name => NAME;

        private string _traitFolderPath = string.Empty;

        private Dictionary<string, WorldTraitData> _traits = new Dictionary<string, WorldTraitData>();
        private Dictionary<string, WorldTrait> _instants = new Dictionary<string, WorldTrait>();

        public TraitService(string traitFolderPath)
        {
            _traitFolderPath = traitFolderPath;
        }

        public override void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                if (Directory.Exists(_traitFolderPath))
                {
                    var files = Directory.GetFiles(_traitFolderPath, $"*.{WorldTraitData.EXTENSION}");
                    foreach (var file in files)
                    {
                        var result = FileData.LoadData<WorldTraitData>(file);
                        if (result.Success)
                        {
                            var trait = GenerateFromData(result.Data);
                            if (trait.Instant)
                            {
                                _instants.Add(trait.Name, trait);
                            }
                            else
                            {
                                trait.Destroy();
                                _traits.Add(result.Data.Name, result.Data);
                            }
                        }
                    }
                }
                base.Start();
            }

        }

        public WorldTrait GenerateFromData(WorldTraitData data)
        {
            switch (data.Type)
            {
                case AiMovementTraitData.TYPE:
                    return new AiMovementTrait(data);
                case SpriteTraitData.TYPE:
                    return new SpriteTrait(data);
                case AiStateTraitData.TYPE:
                    return new AiStateTrait();
                case ObjectSpawnerTraitData.TYPE:
                    return new ObjectSpawnerTrait(data);
                case AiWanderTraitData.TYPE:
                    return new AiWanderTrait(data);
                case AiAggroTraitData.TYPE:
                    return new AiAggroTrait(data);
                case CombatStatsTraitData.TYPE:
                    return new CombatStatsTrait(data);
                case HurtboxTraitData.TYPE:
                    return new HurtboxTrait(data);
                case HitboxTraitData.TYPE:
                    return new HitboxTrait(data);
                case AiAbilityManagerTraitData.TYPE:
                    return new AiAbilityManagerTrait(data);
                case LootTraitData.TYPE:
                    return new LootTrait(data);
                case MonsterExperienceTraitData.TYPE:
                    return new MonsterExperienceTrait(data);
                case ShopTraitData.TYPE:
                    return new ShopTrait(data);
                case TerrainTraitData.TYPE:
                    return new TerrainTrait(data);
                case ApplyMovementSpeedTraitData.TYPE:
                    return new ApplyMovementSpeedTrait(data);
                case DoorTraitData.TYPE:
                    return new DoorTrait(data);
                case DialogueTraitData.TYPE:
                    return new DialogueTrait(data);
                case TriggerEventReceiverTraitData.TYPE:
                    return new TriggerEventReceiverTrait(data);
                case SwitchSignalReceiverTraitData.TYPE:
                    return new SwitchSignalReceiverTrait(data);
                case SetDoorStateTraitData.TYPE:
                    return new SetDoorStateTrait(data);
                case GroupSpawnerTraitData.TYPE:
                    return new GroupSpawnerTrait(data);
                case LootChestTraitData.TYPE:
                    return new LootChestTrait(data);
                case TimerTraitData.TYPE:
                    return new TimerTrait(data);
                case ChargeTraitData.TYPE:
                    return new ChargeTrait(data);
                case KeyItemChestTraitData.TYPE:
                    return new KeyItemChestTrait(data);
                case ZoneQuestTraitData.TYPE:
                    return new ZoneQuestTrait(data);
                case SpawnObjectTraitData.TYPE:
                    return new SpawnObjectTrait(data);
                case ZoneTransitionTraitData.TYPE:
                    return new ZoneTransitionTrait(data);
                case CrafterTraitData.TYPE:
                    return new CrafterTrait(data);
                case SetObjectActiveStateTraitData.TYPE:
                    return new SetObjectActiveStateTrait(data);
                case ToggleObjectStateTraitData.TYPE:
                    return new ToggleObjectStateTrait(data);
                case MultiSwitchSignalReceiverTraitData.TYPE:
                    return new MultiSwitchSignalReceiverTrait(data);
                case SetSwitchLockStateTraitData.TYPE:
                    return new SetSwitchLockStateTrait(data);
                case SetSwitchSignalTraitData.TYPE:
                    return new SetSwitchSignalTrait(data);
                case AdvanceSwitchSignalTraitData.TYPE:
                    return new AdvanceSwitchSignalTrait(data);
                case MovableTraitData.TYPE:
                    return new MovableTrait(data);
                case WalledTraitData.TYPE:
                    return new WalledTrait(data);
                case ProjectileRedirectTraitData.TYPE:
                    return new ProjectileRedirectTrait(data);
                case MonsterScalingTraitData.TYPE:
                    return new MonsterScalingTrait(data);
                case ApplyCombatStatsTraitData.TYPE:
                    return new ApplyCombatStatsTrait(data);
                case TilemapTraitData.TYPE:
                    return new TilemapTrait(data);
                case CullableTraitData.TYPE:
                    return new CullableTrait(data);
                case TeleportTraitData.TYPE:
                    return new TeleportTrait(data);
                case TeleportAroundTargetTraitData.TYPE:
                    return new TeleportAroundTargetTrait(data);
                case VisualFxTraitData.TYPE:
                    return new VisualFxTrait(data);
                case ActivateTriggerEventsTraitData.TYPE:
                    return new ActivateTriggerEventsTrait(data);
                case WorldQuestTraitData.TYPE:
                    return new WorldQuestTrait(data);
                case StaticTeleportTraitData.TYPE:
                    return new StaticTeleportTrait(data);
                case RestoreManaTraitData.TYPE:
                    return new RestoreManaTrait(data);
                case OnDamageTakenTraitData.TYPE:
                    return new OnDamageTakenTrait(data);
                case BossTraitData.TYPE:
                    return new BossTrait(data);
                default:
                    return new WorldTrait(data);
            }
        }

        public static WorldTrait GetTraitByName(string name)
        {
            if (_instance._instants.TryGetValue(name, out var worldTrait))
            {
                return worldTrait;
            }

            if (_instance._traits.TryGetValue(name, out var traitData))
            {
                return _instance.GenerateFromData(traitData);
            }

            return null;
        }

        public static WorldTrait[] GetWorldTraits(string[] names)
        {
            var traits = new List<WorldTrait>();
            foreach (var name in names)
            {
                var trait = GetTraitByName(name);
                if (trait != null)
                {
                    traits.Add(trait);
                }
            }

            return traits.ToArray();
        }
    }
}