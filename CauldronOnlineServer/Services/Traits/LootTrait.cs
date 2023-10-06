using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Items;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class LootTrait : WorldTrait
    {
        private string _lootTable = string.Empty;
        private WorldIntRange _drops;

        public LootTrait(WorldTraitData data) : base(data)
        {
            if (data is LootTraitData lootData)
            {
                _lootTable = lootData.LootTable;
                _drops = lootData.Drops;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<ObjectDeathMessage>(ObjectDeath, _id);
        }

        private void ObjectDeath(ObjectDeathMessage msg)
        {
            var lootTable = ItemService.GetLootTable(_lootTable);
            if (lootTable != null)
            {
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    zone.EventManager.RegisterEvent(new SpawnLootEvent { LootTable = lootTable, OwnerId = _parent.Data.Id, Position = _parent.Data.Position, Drops = _drops });
                }
            }
            
        }
    }
}