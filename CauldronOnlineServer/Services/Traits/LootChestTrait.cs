using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Items;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;

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
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
        }

        public override bool OpenChest()
        {
            if (!_open)
            {
                _parameter.Open = true;
                var lootTable = ItemService.GetLootTable(_parameter.LootTable);
                if (lootTable != null)
                {
                    var zone = ZoneService.GetZoneById(_parent.ZoneId);
                    if (zone != null)
                    {
                        zone.EventManager.RegisterEvent(new SpawnLootEvent { Drops = _parameter.Drops, LootTable = lootTable, Position = _parent.Data.Position, OwnerId = _parent.Data.Id});
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
                base.OpenChest();
            }

            return _open;
        }

        private void RefillChest()
        {
            _refillTimer.Destroy();
            _refillTimer = null;
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
            _destroyTimer.Destroy();
            _destroyTimer = null;
            if (!_open)
            {
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    zone.ObjectManager.RequestDestroyObject(_parent.Data.Id);
                }
            }
        }

        public override void Destroy()
        {
            if (_refillTimer != null)
            {
                _refillTimer.Destroy();
            }

            if (_destroyTimer != null)
            {
                _destroyTimer.Destroy();
            }
            base.Destroy();
        }
    }
}