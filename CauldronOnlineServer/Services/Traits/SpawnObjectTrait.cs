using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.Zones;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class SpawnObjectTrait : WorldTrait
    {
        public override bool Instant => true;

        private ZoneSpawnData _zoneSpawn = null;
        

        public SpawnObjectTrait(WorldTraitData data) : base(data)
        {
            if (data is SpawnObjectTraitData spawnData)
            {
                _zoneSpawn = spawnData.Spawn;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                var tile = zone.GetTile(_parent.Tile.Position + _zoneSpawn.Tile);
                if (tile != null)
                {
                    zone.ObjectManager.RequestObject(_zoneSpawn.Spawn.DisplayName, _zoneSpawn.Spawn.Traits,
                        _zoneSpawn.Spawn.ShowNameOnClient, _zoneSpawn.Spawn.Parameters, tile.WorldPosition,
                        _zoneSpawn.Spawn.IsMonster, null, _zoneSpawn.Spawn.ShowOnClient, true,
                        _zoneSpawn.Spawn.ShowAppearance, _zoneSpawn.StartActive);
                }
            }
        }
    }
}