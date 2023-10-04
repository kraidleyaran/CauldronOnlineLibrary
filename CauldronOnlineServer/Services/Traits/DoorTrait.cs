using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Items;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones;
using MessageBusLib;

namespace CauldronOnlineServer.Services.Traits
{
    public class DoorTrait : WorldTrait
    {
        private DoorParameter _doorParameter = new DoorParameter();

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

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<SetDoorStateMessage>(SetDoorState, _id);
            _parent.SubscribeWithFilter<DoorCheckMessage>(DoorCheck, _id);
            if (_doorParameter.TriggerEvents.Length > 0)
            {
                foreach (var triggerEvent in _doorParameter.TriggerEvents)
                {
                    this.SubscribeWithFilter<ZoneEventTriggerMessage>(ZoneEventTrigger, TriggerEventService.GetFilter(_parent.ZoneId, triggerEvent));
                }
            }
        }

        private void SetDoorState(SetDoorStateMessage msg)
        {
            _doorParameter.Open = msg.Open;
            _parent.AddParameter(_doorParameter);
            if (!msg.IsEvent)
            {
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
                    zone.EventManager.RegisterEvent(new DoorEvent{Open = _doorParameter.Open, OwnerId = _parent.Data.Id, TargetId = _parent.Data.Id});
                }
            }
        }

        private void ZoneEventTrigger(ZoneEventTriggerMessage msg)
        {
            _doorParameter.Open = !_doorParameter.Open;
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
                zone.EventManager.RegisterEvent(new DoorEvent { Open = _doorParameter.Open, OwnerId = _parent.Data.Id, TargetId = _parent.Data.Id });
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