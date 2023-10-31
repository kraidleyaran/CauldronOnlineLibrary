using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Zones;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;

namespace CauldronOnlineServer.Services.Traits
{
    public class DelayedSpawnTrait : WorldTrait
    {
        private int _delayedTicks = 0;
        private ZoneSpawnData _spawnData = null;

        private TickTimer _spawnTimer = null;

        public DelayedSpawnTrait(DelayedSpawnParameter parameter)
        {
            Name = parameter.Type;
            _delayedTicks = parameter.DelayTicks;
            _spawnData = parameter.Spawn;
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _spawnTimer = new TickTimer(_delayedTicks, 0, _parent.ZoneId);
            _spawnTimer.OnComplete += Spawn;
        }

        private void ObjectSpawned(WorldObject obj)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.ObjectManager.RequestDestroyObject(_parent.Data.Id);
            }
        }

        private void Spawn()
        {
            _spawnTimer.Destroy();
            _spawnTimer = null;

            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.ObjectManager.RequestObject(_spawnData.Spawn.DisplayName, _spawnData.Spawn.Traits, _spawnData.Spawn.ShowNameOnClient, _spawnData.Spawn.Parameters, _parent.Data.Position, _spawnData.Spawn.IsMonster, ObjectSpawned, _spawnData.Spawn.ShowOnClient, true, _spawnData.ShowAppearance, _spawnData.StartActive);
            }
        }

        public override void Destroy()
        {
            if (_spawnTimer != null)
            {
                _spawnTimer.Destroy();
                _spawnTimer = null;
            }
            base.Destroy();
        }
    }
}