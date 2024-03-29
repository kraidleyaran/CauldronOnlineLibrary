﻿using System.Collections.Generic;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Switches;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Items;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class DoorTrait : WorldTrait
    {
        private DoorParameter _doorParameter = new DoorParameter();

        private List<string> _triggeredEvents = new List<string>();

        public DoorTrait(DoorParameter doorParameter)
        {
            Name = doorParameter.Type;
            _doorParameter = doorParameter;
        }

        public DoorTrait(WorldTraitData data) : base(data)
        {
            if (data is DoorTraitData door)
            {
                _doorParameter.Open = door.Open;
                _doorParameter.RequiredItems = door.RequiredItems;
                _doorParameter.Hitbox = door.Hitbox;
                _doorParameter.Rotation = door.Rotation;
                _doorParameter.TriggerEvents = door.TriggerEvents;
                _doorParameter.AllowOpenWithNoItems = door.AllowOpenWithNoItems;
                _doorParameter.ApplyTrapSpawn = door.ApplyTrapSpawn;
                _doorParameter.TrappedSpawnPosition = door.TrappedSpawnPosition;
                _doorParameter.Signals = door.Signals;
                _doorParameter.RequireAllEvents = door.RequireAllTriggerEvents;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_doorParameter);
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null && !_doorParameter.Open)
            {
                zone.SetBlockedTile(_parent.Tile.Position, _parent);
            }
            SubscribeToMessages();
        }

        private void ToggleDoor(bool registerEvent)
        {
            _doorParameter.Open = !_doorParameter.Open;
            _parent.RefreshParameters();
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                if (_doorParameter.Open)
                {
                    zone.RemoveBlockedTile(_parent.Tile.Position, _parent);
                }
                else
                {
                    zone.SetBlockedTile(_parent.Tile.Position, _parent);
                }

                if (registerEvent)
                {
                    zone.EventManager.RegisterEvent(new DoorEvent { Open = _doorParameter.Open, OwnerId = _parent.Data.Id, TargetId = _parent.Data.Id });
                }
            }
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<SetDoorStateMessage>(SetDoorState, _id);
            _parent.SubscribeWithFilter<DoorCheckMessage>(DoorCheck, _id);
            if (_doorParameter.TriggerEvents.Length > 0)
            {
                foreach (var triggerEvent in _doorParameter.TriggerEvents)
                {
                    this.SubscribeWithFilter<ZoneEventTriggerMessage>(msg => ZoneEventTrigger(msg, triggerEvent), TriggerEventService.GetFilter(_parent.ZoneId, triggerEvent));
                }
            }

            if (_doorParameter.Signals.Length > 0)
            {
                foreach (var signal in _doorParameter.Signals)
                {
                    this.SubscribeWithFilter<UpdateSignalMessage>(msg => UpdateSignal(msg, signal), SwitchTrait.GenerateFilter(signal.Switch, _parent.ZoneId));
                }
            }
        }

        private void SetDoorState(SetDoorStateMessage msg)
        {
            _doorParameter.Open = msg.Open;
            _parent.RefreshParameters();
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                if (_doorParameter.Open)
                {
                    zone.RemoveBlockedTile(_parent.Tile.Position, _parent);
                }
                else
                {
                    zone.SetBlockedTile(_parent.Tile.Position, _parent);
                }

                if (!msg.IsEvent)
                {
                    zone.EventManager.RegisterEvent(new DoorEvent { Open = _doorParameter.Open, OwnerId = _parent.Data.Id, TargetId = _parent.Data.Id });
                }
            }
        }

        private void ZoneEventTrigger(ZoneEventTriggerMessage msg, string triggerEvent)
        {
            if (!_doorParameter.RequireAllEvents && !_triggeredEvents.Contains(triggerEvent))
            {
                _triggeredEvents.Add(triggerEvent);
            }

            if (!_doorParameter.RequireAllEvents || _triggeredEvents.Count == _doorParameter.TriggerEvents.Length)
            {
                ToggleDoor(true);
            }

        }

        private void UpdateSignal(UpdateSignalMessage msg, RequiredSwitchSignalData required)
        {
            if (required.Signal == msg.Signal)
            {
                ToggleDoor(true);
            }
        }

        private void DoorCheck(DoorCheckMessage msg)
        {
            if (!_doorParameter.Open)
            {
                var open = false;
                if (_doorParameter.RequiredItems.Length > 0)
                {
                    var allItemsFound = true;
                    foreach (var item in _doorParameter.RequiredItems)
                    {
                        if (!ItemService.HasKeyItem(item.Item, item.Stack))
                        {
                            allItemsFound = false;
                            break;
                        }
                    }

                    open = allItemsFound;
                }
                else if (_doorParameter.AllowOpenWithNoItems)
                {
                    open = true;
                }

                if (open)
                {
                    if (_doorParameter.RequiredItems.Length > 0)
                    {
                        foreach (var item in _doorParameter.RequiredItems)
                        {
                            ItemService.RemoveKeyItem(item.Item, item.Stack);
                        }
                    }
                    _doorParameter.Open = true;
                    _parent.RefreshParameters();
                    var zone = ZoneService.GetZoneById(_parent.ZoneId);
                    if (zone != null)
                    {
                        zone.EventManager.RegisterEvent(new DoorEvent { Open = _doorParameter.Open, OwnerId = _parent.Data.Id, TargetId = _parent.Data.Id });
                    }

                }
            }
            
        }
    }
}