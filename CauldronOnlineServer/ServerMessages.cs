﻿using System;
using CauldronOnlineCommon;
using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineServer.Requests;
using CauldronOnlineServer.Services.Traits;
using CauldronOnlineServer.Services.Zones;
using MessageBusLib;
using Newtonsoft.Json.Serialization;

namespace CauldronOnlineServer
{
    public class ZoneUpdateTickMessage : EventMessage
    {
        public static ZoneUpdateTickMessage INSTANCE = new ZoneUpdateTickMessage();
    }

    public class ZoneResolveTickMessage : EventMessage
    {
        public static ZoneResolveTickMessage INSTANCE = new ZoneResolveTickMessage();
    }

    public class ZonePlayerUpdateTickMessage : EventMessage
    {
        public static ZonePlayerUpdateTickMessage INSTANCE = new ZonePlayerUpdateTickMessage();
    }

    public class ZoneEventProcessTickMessage : EventMessage
    {
        public static ZoneEventProcessTickMessage INSTANCE = new ZoneEventProcessTickMessage();
    }

    public class UpdateCurrentZoneTickMessage : EventMessage
    {
        public WorldTick Tick;
    }


    public class PlayerWorldEventsUpdateMessage : EventMessage
    {
        public byte[] Message;
    }

    public class UpdatePositionMessage : EventMessage
    {
        public WorldVector2Int Position;
    }

    public class PlayerDisconnectedMessage : EventMessage
    {
        public static PlayerDisconnectedMessage INSTANCE = new PlayerDisconnectedMessage();
    }

    public class SetCurrentPathMessage : EventMessage
    {
        public ZoneTile[] Path;
    }

    public class ZoneTileUpdatedMessage : EventMessage
    {
        public static ZoneTileUpdatedMessage INSTANCE = new ZoneTileUpdatedMessage();
    }

    public class RemoveObjectFromSpawnerMessage : EventMessage
    {
        public string ObjectId;
    }

    public class SetAiStateMessage : EventMessage
    {
        public AiState State;
    }

    public class UpdateAiStateMessage : EventMessage
    {
        public AiState State;
    }

    public class QueryAiStateMessage : EventMessage
    {
        public Action<AiState> DoAfter;
    }

    public class AggroRequestMessage : EventMessage
    {
        public AggroRequest Request;
    }

    public class ClearCurrentPathMessage : EventMessage
    {
        public static ClearCurrentPathMessage INSTANCE = new ClearCurrentPathMessage();
    }

    public class TakeDamageMessage : EventMessage
    {
        public int Amount;
        public string OwnerId;
    }

    public class ObjectStateUpdatedMessage : EventMessage
    {
        public static ObjectStateUpdatedMessage INSTANCE = new ObjectStateUpdatedMessage();
    }

    public class ObjectDeathMessage : EventMessage
    {
        public static ObjectDeathMessage INSTANCE = new ObjectDeathMessage{OwnerId = string.Empty};
        public string OwnerId;
        
    }

    public class AbilityCheckMessage : EventMessage
    {
        public int Distance;
        public WorldVector2Int Direction;
        public WorldObject Target;
        public ZoneTile[] Pov;
        public Action<bool> DoAfter;
        
    }

    public class ApplyKnockbackMessage : EventMessage
    {
        public WorldVector2Int Position;
        public int Time;
    }

    public class KnockbackFinishedMessage : EventMessage
    {
        public static KnockbackFinishedMessage INSTANCE = new KnockbackFinishedMessage();
    }

    public class UpdatePovMessage : EventMessage
    {
        public ZoneTile[] Pov;
    }

    public class RemoveFromAggroMessage : EventMessage
    {
        public string OwnerId;
    }

    public class ApplyExperienceMessage : EventMessage
    {
        public byte[] Message;
    }

    public class QueryCombatVitalsMessage : EventMessage
    {
        public Action<CombatVitals> DoAfter;
    }

    public class ApplyMovementSpeedMessage : EventMessage
    {
        public int Speed;
        public bool Bonus;
    }

    public class HealMessage : EventMessage
    {
        public string OwnerId;
        public int Amount;
        public bool IsEvent;
    }

    public class SetDoorStateMessage : EventMessage
    {
        public bool Open;
        public bool IsEvent;
    }

    public class EventTriggeredMessage : EventMessage
    {
        public string Event;
    }

    public class ZoneEventTriggerMessage : EventMessage
    {
        public static ZoneEventTriggerMessage INSTANCE = new ZoneEventTriggerMessage();
    }

    public class ZoneCheckInMessage : EventMessage
    {
        public string Zone;
    }

    public class ZonesFinishedMessage : EventMessage
    {
        public static ZonesFinishedMessage INSTANCE = new ZonesFinishedMessage();
    }

    public class SetSwitchSignalMessage : EventMessage
    {
        public int Signal;
        public bool IsEvent;
    }

    public class UpdateSignalMessage : EventMessage
    {
        public int Signal;
    }

    public class DoorCheckMessage : EventMessage
    {
        public static DoorCheckMessage INSTANCE = new DoorCheckMessage();
    }

    public class OpenChestMessage : EventMessage
    {
        public static OpenChestMessage INSTANCE = new OpenChestMessage();
    }

}