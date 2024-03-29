﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using Newtonsoft.Json;
using Telepathy;

namespace CauldronOnlineCommon
{
    public static class CauldronUtils
    {
        public const int MULTIPART_RESERVE_SIZE = 256;
        public const string POSITIVE = "+";
        public const string NEGATIVE = "-";
        public const float kEpsilon = 1E-05f;
        public const int MAX_MESSAGE_SIZE = 65536;

        private static JsonSerializerSettings _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };

        public static ClientMessage GenerateMessageFromData(this byte[] data)
        {
            var json = Encoding.UTF8.GetString(data, 0, data.Length);
            var original = JsonConvert.DeserializeObject<ClientMessage>(json, _settings);
            switch (original.MessageId)
            {
                default:
                    return original;
                case ClientConnectRequestMessage.ID:
                    return JsonConvert.DeserializeObject<ClientConnectRequestMessage>(json, _settings);
                case ClientConnectResultMessage.ID:
                    return JsonConvert.DeserializeObject<ClientConnectResultMessage>(json, _settings);
                case ClientWorldEventsUpdateMessage.ID:
                    return JsonConvert.DeserializeObject<ClientWorldEventsUpdateMessage>(json, _settings);
                case ClientMovementUpdateMessage.ID:
                    return JsonConvert.DeserializeObject<ClientMovementUpdateMessage>(json, _settings);
                case ClientZoneTickMessage.ID:
                    return JsonConvert.DeserializeObject<ClientZoneTickMessage>(json, _settings);
                case ClientWorldSettingsRequestMessage.ID:
                    return JsonConvert.DeserializeObject<ClientWorldSettingsRequestMessage>(json, _settings);
                case ClientWorldSettingsResultMessage.ID:
                    return JsonConvert.DeserializeObject<ClientWorldSettingsResultMessage>(json, _settings);
                case ClientCreateCharacterRequestMessage.ID:
                    return JsonConvert.DeserializeObject<ClientCreateCharacterRequestMessage>(json, _settings);
                case ClientCreateCharacterResultMessage.ID:
                    return JsonConvert.DeserializeObject<ClientCreateCharacterResultMessage>(json, _settings);
                case ClientObjectRequestMessage.ID:
                    return JsonConvert.DeserializeObject<ClientObjectRequestMessage>(json, _settings);
                case ClientObjectResultMessage.ID:
                    return JsonConvert.DeserializeObject<ClientObjectResultMessage>(json, _settings);
                case ClientAggroMessage.ID:
                    return JsonConvert.DeserializeObject<ClientAggroMessage>(json, _settings);
                case ClientDamageMessage.ID:
                    return JsonConvert.DeserializeObject<ClientDamageMessage>(json, _settings);
                case ClientKnockbackMessage.ID:
                    return JsonConvert.DeserializeObject<ClientKnockbackMessage>(json, _settings);
                case ClientUseAbilityMessage.ID:
                    return JsonConvert.DeserializeObject<ClientUseAbilityMessage>(json, _settings);
                case ClientPingMessage.ID:
                    return JsonConvert.DeserializeObject<ClientPingMessage>(json, _settings);
                case ClientDeathMessage.ID:
                    return JsonConvert.DeserializeObject<ClientDeathMessage>(json, _settings);
                case ClientShootProjectileMessage.ID:
                    return JsonConvert.DeserializeObject<ClientShootProjectileMessage>(json, _settings);
                case ClientDestroyObjectMessage.ID:
                    return JsonConvert.DeserializeObject<ClientDestroyObjectMessage>(json, _settings);
                case ClientZoneTransferRequestMessage.ID:
                    return JsonConvert.DeserializeObject<ClientZoneTransferRequestMessage>(json, _settings);
                case ClientZoneTransferResultMessage.ID:
                    return JsonConvert.DeserializeObject<ClientZoneTransferResultMessage>(json, _settings);
                case ClientRespawnRequestMessage.ID:
                    return JsonConvert.DeserializeObject<ClientRespawnRequestMessage>(json, _settings);
                case ClientRespawnResultMessage.ID:
                    return JsonConvert.DeserializeObject<ClientRespawnResultMessage>(json, _settings);
                case ClientExperienceMessage.ID:
                    return JsonConvert.DeserializeObject<ClientExperienceMessage>(json, _settings);
                case ClientOpenDoorMessage.ID:
                    return JsonConvert.DeserializeObject<ClientOpenDoorMessage>(json, _settings);
                case ClientTriggerEventMessage.ID:
                    return JsonConvert.DeserializeObject<ClientTriggerEventMessage>(json, _settings);
                case ClientHealMessage.ID:
                    return JsonConvert.DeserializeObject<ClientHealMessage>(json, _settings);
                case ClientSwitchSignalMessage.ID:
                    return JsonConvert.DeserializeObject<ClientSwitchSignalMessage>(json, _settings);
                case ClientAddKeyItemMessage.ID:
                    return JsonConvert.DeserializeObject<ClientAddKeyItemMessage>(json, _settings);
                case ClientDoorCheckMessage.ID:
                    return JsonConvert.DeserializeObject<ClientDoorCheckMessage>(json, _settings);
                case ClientOpenChestMessage.ID:
                    return JsonConvert.DeserializeObject<ClientOpenChestMessage>(json, _settings);
                case ClientMultiPartMessage.ID:
                    return JsonConvert.DeserializeObject<ClientMultiPartMessage>(json, _settings);
                case ClientTeleportMessage.ID:
                    return JsonConvert.DeserializeObject<ClientTeleportMessage>(json, _settings);
                case ClientItemRewardMessage.ID:
                    return JsonConvert.DeserializeObject<ClientItemRewardMessage>(json, _settings);
                case ClientProjectileMovementUpdateMessage.ID:
                    return JsonConvert.DeserializeObject<ClientProjectileMovementUpdateMessage>(json, _settings);
                case ClientSystemEventMessage.ID:
                    return JsonConvert.DeserializeObject<ClientSystemEventMessage>(json, _settings);
                case ClientPlayerLeveledMessage.ID:
                    return JsonConvert.DeserializeObject<ClientPlayerLeveledMessage>(json, _settings);
                case ClientMovableUpdateMessage.ID:
                    return JsonConvert.DeserializeObject<ClientMovableUpdateMessage>(json, _settings);
                case ClientRollUpdateMessage.ID:
                    return JsonConvert.DeserializeObject<ClientRollUpdateMessage>(json, _settings);
                case ClientPlayerRosterRequestMessage.ID:
                    return JsonConvert.DeserializeObject<ClientPlayerRosterRequestMessage>(json, _settings);
                case ClientPlayerRosterResponseMessage.ID:
                    return JsonConvert.DeserializeObject<ClientPlayerRosterResponseMessage>(json, _settings);
                case ClientPlayerRosterUpdateMessage.ID:
                    return JsonConvert.DeserializeObject<ClientPlayerRosterUpdateMessage>(json, _settings);
                case ClientReturnToOwnerMessage.ID:
                    return JsonConvert.DeserializeObject<ClientReturnToOwnerMessage>(json, _settings);
                case ClientProjectileReturnedMessage.ID:
                    return JsonConvert.DeserializeObject<ClientProjectileReturnedMessage>(json, _settings);
                case ClientUpdateCombatStatsMessage.ID:
                    return JsonConvert.DeserializeObject<ClientUpdateCombatStatsMessage>(json, _settings);
                case ClientQuestInteractionRequestMessage.ID:
                    return JsonConvert.DeserializeObject<ClientQuestInteractionRequestMessage>(json, _settings);
                case ClientQuestInteractionResultMessage.ID:
                    return JsonConvert.DeserializeObject<ClientQuestInteractionResultMessage>(json, _settings);
                case ClientDropItemMessage.ID:
                    return JsonConvert.DeserializeObject<ClientDropItemMessage>(json, _settings);
                case ClientClaimItemRequestMessage.ID:
                    return JsonConvert.DeserializeObject<ClientClaimItemRequestMessage>(json, _settings);
                case ClientClaimItemResultMessage.ID:
                    return JsonConvert.DeserializeObject<ClientClaimItemResultMessage>(json, _settings);
                case ClientPlayerRosterRemoveMessage.ID:
                    return JsonConvert.DeserializeObject<ClientPlayerRosterRemoveMessage>(json, _settings);
                case ClientResetZoneRequestMessage.ID:
                    return JsonConvert.DeserializeObject<ClientResetZoneRequestMessage>(json, _settings);
                case ClientResetZoneResultMessage.ID:
                    return JsonConvert.DeserializeObject<ClientResetZoneResultMessage>(json, _settings);

            }
        }

        public static WorldEvent GenerateEventFromData(this byte[] data)
        {
            var json = Encoding.UTF8.GetString(data, 0, data.Length);
            var original = JsonConvert.DeserializeObject<WorldEvent>(json, _settings);
            switch (original.EventId)
            {
                case ObjectCreatedEvent.ID:
                    return JsonConvert.DeserializeObject<ObjectCreatedEvent>(json, _settings);
                case MovementEvent.ID:
                    return JsonConvert.DeserializeObject<MovementEvent>(json, _settings);
                case UpdateCombatStatsEvent.ID:
                    return JsonConvert.DeserializeObject<UpdateCombatStatsEvent>(json, _settings);
                case DamageEvent.ID:
                    return JsonConvert.DeserializeObject<DamageEvent>(json, _settings);
                case KnockbackEvent.ID:
                    return JsonConvert.DeserializeObject<KnockbackEvent>(json, _settings);
                case AbilityEvent.ID:
                    return JsonConvert.DeserializeObject<AbilityEvent>(json, _settings);
                case ObjectDeathEvent.ID:
                    return JsonConvert.DeserializeObject<ObjectDeathEvent>(json, _settings);
                case ShootProjectileEvent.ID:
                    return JsonConvert.DeserializeObject<ShootProjectileEvent>(json, _settings);
                case DestroyObjectEvent.ID:
                    return JsonConvert.DeserializeObject<DestroyObjectEvent>(json, _settings);
                case SpawnLootEvent.ID:
                    return JsonConvert.DeserializeObject<SpawnLootEvent>(json, _settings);
                case RespawnEvent.ID:
                    return JsonConvert.DeserializeObject<RespawnEvent>(json, _settings);
                case UpdatePathEvent.ID:
                    return JsonConvert.DeserializeObject<UpdatePathEvent>(json, _settings);
                case HealEvent.ID:
                    return JsonConvert.DeserializeObject<HealEvent>(json, _settings);
                case FullHealEvent.ID:
                    return JsonConvert.DeserializeObject<FullHealEvent>(json, _settings);
                case DoorEvent.ID:
                    return JsonConvert.DeserializeObject<DoorEvent>(json, _settings);
                case SwitchSignalEvent.ID:
                    return JsonConvert.DeserializeObject<SwitchSignalEvent>(json, _settings);
                case DoorCheckEvent.ID:
                    return JsonConvert.DeserializeObject<DoorCheckEvent>(json, _settings);
                case ChestOpenEvent.ID:
                    return JsonConvert.DeserializeObject<ChestOpenEvent>(json, _settings);
                case ChestRefillEvent.ID:
                    return JsonConvert.DeserializeObject<ChestRefillEvent>(json, _settings);
                case TeleportEvent.ID:
                    return JsonConvert.DeserializeObject<TeleportEvent>(json, _settings);
                case KeyItemLootEvent.ID:
                    return JsonConvert.DeserializeObject<KeyItemLootEvent>(json, _settings);
                case ObjectStateEvent.ID:
                    return JsonConvert.DeserializeObject<ObjectStateEvent>(json, _settings);
                case BridgeStateEvent.ID:
                    return JsonConvert.DeserializeObject<BridgeStateEvent>(json, _settings);
                case TimerEvent.ID:
                    return JsonConvert.DeserializeObject<TimerEvent>(json, _settings);
                case MovableEvent.ID:
                    return JsonConvert.DeserializeObject<MovableEvent>(json, _settings);
                case RollEvent.ID:
                    return JsonConvert.DeserializeObject<RollEvent>(json, _settings);
                case ReturnToOwnerEvent.ID:
                    return JsonConvert.DeserializeObject<ReturnToOwnerEvent>(json, _settings);
                case HasReturnedToOwnerEvent.ID:
                    return JsonConvert.DeserializeObject<HasReturnedToOwnerEvent>(json, _settings);
                case VisualFxEvent.ID:
                    return JsonConvert.DeserializeObject<VisualFxEvent>(json, _settings);
                case WorldQuestUpdateEvent.ID:
                    return JsonConvert.DeserializeObject<WorldQuestUpdateEvent>(json, _settings);
                case PlayerDroppedItemEvent.ID:
                    return JsonConvert.DeserializeObject<PlayerDroppedItemEvent>(json, _settings);
                case PlayerClaimItemEvent.ID:
                    return JsonConvert.DeserializeObject<PlayerClaimItemEvent>(json, _settings);
                default:
                    return original;
            }
        }

        public static ObjectParameter GenerateObjectParameter(this byte[] data)
        {
            var json = Encoding.UTF8.GetString(data, 0, data.Length);
            var original = JsonConvert.DeserializeObject<ObjectParameter>(json, _settings);
            switch (original.Type)
            {
                case AggroParameter.TYPE:
                    return JsonConvert.DeserializeObject<AggroParameter>(json, _settings);
                case CombatStatsParameter.TYPE:
                    return JsonConvert.DeserializeObject<CombatStatsParameter>(json, _settings);
                case HurtboxParameter.TYPE:
                    return JsonConvert.DeserializeObject<HurtboxParameter>(json, _settings);
                case HitboxParameter.TYPE:
                    return JsonConvert.DeserializeObject<HitboxParameter>(json, _settings);
                case KnockbackReceiverParameter.TYPE:
                    return JsonConvert.DeserializeObject<KnockbackReceiverParameter>(json, _settings);
                case AbilityManagerParameter.TYPE:
                    return JsonConvert.DeserializeObject<AbilityManagerParameter>(json, _settings);
                case ObjectDeathParameter.TYPE:
                    return JsonConvert.DeserializeObject<ObjectDeathParameter>(json, _settings);
                case ObjectPathParameter.TYPE:
                    return JsonConvert.DeserializeObject<ObjectPathParameter>(json, _settings);
                case ShopParameter.TYPE:
                    return JsonConvert.DeserializeObject<ShopParameter>(json, _settings);
                case TerrainParameter.TYPE:
                    return JsonConvert.DeserializeObject<TerrainParameter>(json, _settings);
                case DialogueParameter.TYPE:
                    return JsonConvert.DeserializeObject<DialogueParameter>(json, _settings);
                case DoorParameter.TYPE:
                    return JsonConvert.DeserializeObject<DoorParameter>(json, _settings);
                case TriggerEventHitboxParameter.TYPE:
                    return JsonConvert.DeserializeObject<TriggerEventHitboxParameter>(json, _settings);
                case SwitchParameter.TYPE:
                    return JsonConvert.DeserializeObject<SwitchParameter>(json, _settings);
                case GroupSpawnerTraitData.TYPE:
                    return JsonConvert.DeserializeObject<GroupSpawnParameter>(json, _settings);
                case LootChestParameter.TYPE:
                    return JsonConvert.DeserializeObject<LootChestParameter>(json, _settings);
                case KeyItemChestParameter.TYPE:
                    return JsonConvert.DeserializeObject<KeyItemChestParameter>(json, _settings);
                case ZoneTransitionParameter.TYPE:
                    return JsonConvert.DeserializeObject<ZoneTransitionParameter>(json, _settings);
                case CrafterParameter.TYPE:
                    return JsonConvert.DeserializeObject<CrafterParameter>(json, _settings);
                case BridgeParameter.TYPE:
                    return JsonConvert.DeserializeObject<BridgeParameter>(json, _settings);
                case PlayerParameter.TYPE:
                    return JsonConvert.DeserializeObject<PlayerParameter>(json, _settings);
                case MovableParameter.TYPE:
                    return JsonConvert.DeserializeObject<MovableParameter>(json, _settings);
                case WalledParameter.TYPE:
                    return JsonConvert.DeserializeObject<WalledParameter>(json, _settings);
                case MovementParameter.TYPE:
                    return JsonConvert.DeserializeObject<MovementParameter>(json, _settings);
                case ProjectileRedirectParameter.TYPE:
                    return JsonConvert.DeserializeObject<ProjectileRedirectParameter>(json, _settings);
                case BombableDoorParameter.TYPE:
                    return JsonConvert.DeserializeObject<BombableDoorParameter>(json, _settings);
                case StashParameter.TYPE:
                    return JsonConvert.DeserializeObject<StashParameter>(json, _settings);
                case TilemapParameter.TYPE:
                    return JsonConvert.DeserializeObject<TilemapParameter>(json, _settings);
                case CullableParameter.TYPE:
                    return JsonConvert.DeserializeObject<CullableParameter>(json, _settings);
                case WorldQuestParameter.TYPE:
                    return JsonConvert.DeserializeObject<WorldQuestParameter>(json, _settings);
                case BossParameter.TYPE:
                    return JsonConvert.DeserializeObject<BossParameter>(json, _settings);
                case DroppedItemParameter.TYPE:
                    return JsonConvert.DeserializeObject<DroppedItemParameter>(json, _settings);
                case ZoneResetInteractionParameter.TYPE:
                    return JsonConvert.DeserializeObject<ZoneResetInteractionParameter>(json, _settings);
                default:
                    return original;
            }

        }

        public static SystemEvent GenerateSystemEvent(this byte[] data)
        {
            var json = Encoding.UTF8.GetString(data, 0, data.Length);
            return JsonConvert.DeserializeObject<SystemEvent>(json);
        }

        public static byte[] ToData<T>(this T worldEvent) where T : WorldEvent
        {
            var json = JsonConvert.SerializeObject(worldEvent, _settings);
            return Encoding.UTF8.GetBytes(json);
        }

        public static byte[] ToParameterData<T>(this T objParameter) where T : ObjectParameter
        {
            var json = JsonConvert.SerializeObject(objParameter, _settings);
            return Encoding.UTF8.GetBytes(json);
        }

        public static byte[] ToByteArray<T>(this T msg) where T : ClientMessage
        {
            var json = JsonConvert.SerializeObject(msg, _settings);
            return Encoding.UTF8.GetBytes(json);
        }

        public static byte[] ToEventData<T>(this T systemEvent) where T : SystemEvent
        {
            var json = JsonConvert.SerializeObject(systemEvent);
            return Encoding.UTF8.GetBytes(json);
        }

        public static ClientMultiPartMessage[] ToMultiPart(this byte[] data, int maxSize, int messageId)
        {
            var partId = Guid.NewGuid().ToString();
            var max = maxSize - MULTIPART_RESERVE_SIZE;
            var partSize = max - (max / 4);
            var parts = data.Length / partSize;
            var partCheck = parts * partSize;
            if (partCheck < data.Length)
            {
                parts++;
            }

            var returnMessages = new ClientMultiPartMessage[parts];
            var returnData = data.ToList();
            for (var i = 0; i < parts; i++)
            {
                var part = new byte[0];
                var amount = partSize;
                if (amount > returnData.Count)
                {
                    part = returnData.ToArray();
                }
                else
                {
                    part = returnData.GetRange(0, amount).ToArray();
                    returnData.RemoveRange(0, amount);
                }

                returnMessages[i] = new ClientMultiPartMessage { MultiPartId = partId, Part = part, Id = messageId, TotalParts = parts, PartOrder = i };
            }

            return returnMessages;
        }

        public static ClientMessage ToClientMessage(this ClientMultiPartMessage[] multiPart)
        {
            var data = new List<byte>();
            var orderedParts = multiPart.OrderBy(p => p.PartOrder).ToArray();
            foreach (var part in orderedParts)
            {
                data.AddRange(part.Part);
            }
            return data.ToArray().GenerateMessageFromData();

        }

        public static int Square(this int integer)
        {
            return integer * integer;
        }

        public static string ToStatString(this int integer, string statName = "")
        {
            var statNameString = string.IsNullOrEmpty(statName) ? string.Empty : $" {statName}";
            if (integer > 0)
            {
                return $"{POSITIVE}{integer}{statNameString}";

            }

            if (integer < 0)
            {
                return $"{NEGATIVE}{integer}{statNameString}";
            }

            return string.Empty;
        }

        public static string ToStatString(this float value, string statName = "")
        {
            var statNameString = string.IsNullOrEmpty(statName) ? string.Empty : $" {statName}";
            if (value > 0)
            {
                return $"{POSITIVE}{value:P}{statNameString}";
            }

            if (value < 0)
            {
                return $"{NEGATIVE}{value:P}{statNameString}";
            }

            return string.Empty;
        }

        public static string ToSingleLineString(this string[] strings)
        {
            var returnString = string.Empty;
            foreach (var str in strings)
            {
                if (string.IsNullOrEmpty(returnString))
                {
                    returnString = str;
                }
                else
                {
                    returnString = $"{returnString} {str}";
                }
            }

            return returnString;
        }
    }
}
