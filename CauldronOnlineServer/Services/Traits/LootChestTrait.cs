using CauldronOnlineCommon.Data.Items;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Items;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class LootChestTrait : ChestTrait
    {
        private LootChestParameter _parameter = new LootChestParameter();

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
                base.OpenChest();
            }

            return _open;
        }
    }
}