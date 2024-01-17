using CauldronOnlineCommon;
using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Items;
using CauldronOnlineServer.Services.SystemEvents;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class KeyItemChestTrait : ChestTrait
    {
        private KeyItemChestParameter _parameter = new KeyItemChestParameter();

        public KeyItemChestTrait(KeyItemChestParameter parameter)
        {
            Name = parameter.Type;
            _parameter = parameter;
        }

        public KeyItemChestTrait(WorldTraitData data) : base(data)
        {
            if (data is KeyItemChestTraitData chest)
            {
                _parameter.Item = chest.Item;
                _parameter.Open = false;
                _parameter.OpenSprite = chest.OpenSprite;
                _parameter.ClosedSprite = chest.ClosedSprite;
                _parameter.Hitbox = chest.Hitbox;
                _parameter.ApplyEventsOnOpen = chest.ApplyTriggerEventsOnOpen;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
        }

        public override bool OpenChest(string playerName)
        {
            if (!_open)
            {
                _open = true;
                ItemService.AddKeyItem(_parameter.Item.Item, _parameter.Item.Stack);
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    zone.EventManager.RegisterEvent(new ChestOpenEvent{TargetId = _parent.Data.Id, PlayerName = playerName});
                    zone.EventManager.RegisterEvent(new KeyItemLootEvent{Position = _parent.Data.Position, Item = _parameter.Item.Item, Stack = _parameter.Item.Stack, TargetId = _parent.Data.Id, PlayerName = playerName});                    
                }

                foreach (var triggerEvent in _parameter.ApplyEventsOnOpen)
                {
                    TriggerEventService.TriggerEvent(triggerEvent);
                }

                if (_parameter.RewardToPlayers.Length > 0)
                {
                    WorldServer.SendToAllClients(new ClientItemRewardMessage{Items = _parameter.RewardToPlayers});
                }
            }

            return _open;

        }
    }
}