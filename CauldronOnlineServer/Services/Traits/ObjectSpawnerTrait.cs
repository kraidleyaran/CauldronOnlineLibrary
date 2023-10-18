using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.Zones;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class ObjectSpawnerTrait : WorldTrait
    {
        private int _spawnEvery = 1;
        private int _maxSpawns = 1;
        private int _spawnArea = 1;
        private float _chanceToSpawn = 1f;
        private float _bonusChanceToSpawn = 0f;
        private float _bonusPerMissedChance = 0f;
        private bool _initialSpawn = false;

        private TickTimer _spawnTimer = null;

        private ObjectSpawnData _spawnData = null;

        private List<string> _worldObjects = new List<string>();

        private ZoneTile[] _spawnableTiles = new ZoneTile[0];

        public ObjectSpawnerTrait(WorldTraitData data) : base(data)
        {
            if (data is ObjectSpawnerTraitData spawnerData)
            {
                _spawnArea = spawnerData.SpawnArea;
                _maxSpawns = spawnerData.MaxSpawns;
                _spawnEvery = spawnerData.SpawnEvery;
                _spawnData = spawnerData.SpawnData;
                _chanceToSpawn = spawnerData.ChanceToSpawn;
                _bonusPerMissedChance = spawnerData.BonusPerMissedChance;
                _initialSpawn = spawnerData.InitialSpawn;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                _spawnableTiles = zone.GetTilesInSquareArea(_parent.Tile, _spawnArea);
            }

            if (_initialSpawn)
            {
                _bonusChanceToSpawn = 100f;
                SpawnUnit();
            }
            SubscribeToMessages();
        }

        private void SpawnUnit()
        {
            var spawn = false;
            if (_chanceToSpawn < 1f)
            {
                spawn = RNGService.Roll(_chanceToSpawn + _bonusChanceToSpawn);
                if (spawn)
                {
                    _bonusChanceToSpawn = 0f;
                }
                else
                {
                    _bonusChanceToSpawn += _bonusPerMissedChance;
                }
            }
            else
            {
                spawn = true;
            }

            if (spawn)
            {
                
                if (_spawnableTiles.Length > 0)
                {
                    var zone = ZoneService.GetZoneById(_parent.ZoneId);
                    if (zone != null)
                    {
                        var tile = _spawnableTiles.Length > 1 ? _spawnableTiles[RNGService.Range(0, _spawnableTiles.Length)] : _spawnableTiles[0];
                        zone.ObjectManager.RequestObject(_spawnData.DisplayName, _spawnData.Traits, _spawnData.ShowNameOnClient, _spawnData.Parameters, tile.WorldPosition, _spawnData.IsMonster, OnUnitSpawned, _spawnData.ShowOnClient, true, _spawnData.ShowAppearance, _spawnData.StartActive);
                    }
                }
            }
            

        }

        private void OnUnitSpawned(WorldObject obj)
        {
            _worldObjects.Add(obj.Data.Id);
            obj.AddTrait(new SpawnedObjectTrait(_parent));
        }

        private void TimerComplete()
        {
            if (_worldObjects.Count < _maxSpawns)
            {
                _spawnTimer.Restart();
            }
            else
            {
                _spawnTimer.Destroy();
                _spawnTimer = null;
            }
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<ZoneUpdateTickMessage>(ZoneUpdateTick, _parent.ZoneId);
            _parent.SubscribeWithFilter<RemoveObjectFromSpawnerMessage>(RemoveObjectFromSpawner, _id);

        }

        private void ZoneUpdateTick(ZoneUpdateTickMessage msg)
        {
            if (_worldObjects.Count < _maxSpawns && _spawnTimer == null)
            {
                _spawnTimer = new TickTimer(_spawnEvery, 0, _parent.ZoneId);
                _spawnTimer.OnLoopFinish += SpawnUnit;
                _spawnTimer.OnComplete += TimerComplete;
            }
        }

        private void RemoveObjectFromSpawner(RemoveObjectFromSpawnerMessage msg)
        {
            _worldObjects.Remove(msg.ObjectId);
        }
        
    }
}