using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Items;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class LootChestTrait : ChestTrait
    {
        private LootChestParameter _parameter = new LootChestParameter();

        private TickTimer _refillTimer = null;
        private TickTimer _destroyTimer = null;

        public LootChestTrait(LootChestParameter parameter)
        {
            _parameter = parameter;
        }

        public LootChestTrait(WorldTraitData data) : base(data)
        {
            if (data is LootChestTraitData lootData)
            {
                _parameter.Drops = lootData.Drops;
                _parameter.LootTable = lootData.LootTable;
                _parameter.Open = false;
                _parameter.OpenSprite = lootData.OpenSprite;
                _parameter.ClosedSprite = lootData.ClosedSprite;
                _parameter.Hitbox = lootData.Hitbox;
                _parameter.RefillChest = lootData.RefillChest;
                _parameter.RefillTicks = lootData.RefillTicks;
                _parameter.DestroyAfterOpen = lootData.DestroyAfterOpen;
                _parameter.DestroyTicks = lootData.DestroyTicks;
                _parameter.ResetOnEvents = lootData.ResetOnEvents;
                _parameter.DestroyOnReset = lootData.DestroyOnReset;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
        }

        public override bool OpenChest(string player)
        {
            if (!_open)
            {
                _parameter.Open = true;
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    zone.EventManager.RegisterEvent(new ChestOpenEvent { TargetId = _parent.Data.Id, PlayerName = player});
                    var lootTable = ItemService.GetLootTable(_parameter.LootTable);
                    if (lootTable != null)
                    {
                        zone.EventManager.RegisterEvent(new SpawnLootEvent { Drops = _parameter.Drops, LootTable = lootTable, Position = _parent.Data.Position, OwnerId = _parent.Data.Id });
                    }
                }

                if (_parameter.RefillChest)
                {
                    _refillTimer = new TickTimer(_parameter.RefillTicks.Roll(true), 0, _parent.ZoneId);
                    _refillTimer.OnComplete += RefillChest;
                }
                if (_parameter.DestroyAfterOpen)
                {
                    _destroyTimer = new TickTimer(_parameter.DestroyTicks, 0, _parent.ZoneId);
                    _destroyTimer.OnComplete += DestroyChest;
                }
                _parent.RefreshParameters();
                base.OpenChest(player);
            }

            return _open;
        }

        private void RefillChest()
        {
            if (_refillTimer != null)
            {
                _refillTimer.Destroy();
                _refillTimer = null;
            }

            if (_open)
            {
                _open = false;
                _parameter.Open = false;
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    zone.EventManager.RegisterEvent(new ChestRefillEvent{TargetId = _parent.Data.Id});
                }
            }
        }

        private void DestroyChest()
        {
            if (_destroyTimer != null)
            {
                _destroyTimer.Destroy();
                _destroyTimer = null;
            }

            if (!_open)
            {
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    zone.ObjectManager.RequestDestroyObject(_parent.Data.Id);
                }
            }
        }

        protected internal override void SubscribeToMessages()
        {
            base.SubscribeToMessages();
            if (_parameter.ResetOnEvents.Length > 0)
            {
                foreach (var trigger in _parameter.ResetOnEvents)
                {
                    this.SubscribeWithFilter<ZoneEventTriggerMessage>(ZoneEventTrigger, TriggerEventService.GetFilter(_parent.ZoneId, trigger));
                }
            }
        }

        private void ZoneEventTrigger(ZoneEventTriggerMessage msg)
        {
            if (_parameter.DestroyOnReset)
            {
                DestroyChest();
            }
            else
            {
                RefillChest();
            }
        }

        public override void Destroy()
        {
            if (_refillTimer != null)
            {
                _refillTimer.Destroy();
                _refillTimer = null;
            }

            if (_destroyTimer != null)
            {
                _destroyTimer.Destroy();
                _destroyTimer = null;
            }
            base.Destroy();
        }
    }
}