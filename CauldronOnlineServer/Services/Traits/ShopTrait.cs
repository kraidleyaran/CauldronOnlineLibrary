using System.Collections.Generic;
using System.Linq;
using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class ShopTrait : WorldTrait
    {
        private ShopParameter _shopParameter = null;

        private Dictionary<string, List<ShopItemData>> _restricted = new Dictionary<string, List<ShopItemData>>();

        public ShopTrait(WorldTraitData data) : base(data)
        {
            _shopParameter = new ShopParameter {Items = new ShopItemData[0]};
            if (data is ShopTraitData shopData)
            {
                _shopParameter.Items = shopData.Items.ToArray();
                _shopParameter.Hitbox = shopData.Hitbox;
                _shopParameter.Restricted = shopData.Restricted;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            foreach (var restricted in _shopParameter.Restricted)
            {
                if (!_restricted.TryGetValue(restricted.TriggerEvent, out var items))
                {
                    items = new List<ShopItemData>();
                    _restricted.Add(restricted.TriggerEvent, items);
                }
                items.Add(restricted);
            }
            _parent.AddParameter(_shopParameter);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            var triggerEvents = _restricted.Keys.ToArray();
            foreach (var trigger in triggerEvents)
            {
                this.SubscribeWithFilter<ZoneEventTriggerMessage>(msg => ZoneEventTrigger(trigger), TriggerEventService.GetFilter(_parent.ZoneId, trigger));
            }
        }

        private void ZoneEventTrigger(string trigger)
        {
            var currentItems = _shopParameter.Items.ToList();
            if (_restricted.TryGetValue(trigger, out var items))
            {
                currentItems.AddRange(items);
            }
            _shopParameter.Items = currentItems.ToArray();
            _parent.RefreshParameters();
        }
        
    }
}