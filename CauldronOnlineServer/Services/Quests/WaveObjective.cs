using System.Collections.Generic;
using CauldronOnlineCommon.Data.Quests;
using CauldronOnlineServer.Services.Traits;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Quests
{
    public class WaveObjective : QuestObjective
    {
        public const string FILTER = "WaveObjective";

        private WaveData[] _waves = new WaveData[0];
        private int _ticksBetweenWaves = 1;

        private int _currentWave = 0;
        private ZoneTile[] _availableTiles = new ZoneTile[0];
        private WorldZone _zone = null;
        private WorldObject _parent = null;

        private TickTimer _betweenWavesTimer = null;

        public WaveObjective(QuestObjectiveData data) : base(data)
        {
            if (data is WaveQuestObjectiveData waveData)
            {
                _waves = waveData.Waves;
                _ticksBetweenWaves = waveData.TicksBetweenWaves;
            }
        }

        private List<string> _created = new List<string>();

        public override WorldObject[] SpawnRequiredObjects(WorldZone zone, ZoneTile[] availableTiles, WorldObject questParent, string questName)
        {
            _parent = questParent;
            _availableTiles = availableTiles;
            _zone = zone;
            _parent.SubscribeWithFilter<RemoveObjectFromSpawnerMessage>(RemoveObjectFromSpawner, FILTER);
            SpawnWave();
            return new WorldObject[0];

        }

        private void SpawnWave()
        {
            foreach (var objective in _waves[_currentWave].Spawns)
            {
                for (var i = 0; i < objective.RequiredAmount; i++)
                {
                    var tile = _availableTiles.Length > 1 ? _availableTiles[RNGService.Range(0, _availableTiles.Length)] : _availableTiles[0];
                    var template = objective.Template;
                    _zone.ObjectManager.RequestObject(template.Spawn.DisplayName, template.Spawn.Traits, template.Spawn.ShowNameOnClient, template.Spawn.Parameters, tile.WorldPosition, template.Spawn.IsMonster, ObjectSpawned, template.Spawn.ShowOnClient, true, template.Spawn.ShowAppearance);
                }
            }
        }

        private void TimerFinished()
        {
            _betweenWavesTimer.Destroy();
            _betweenWavesTimer = null;
            SpawnWave();
        }

        private void ObjectSpawned(WorldObject obj)
        {
            obj.AddTrait(new SpawnedObjectTrait(_parent));
            _created.Add(obj.Data.Id);
        }

        private void RemoveObjectFromSpawner(RemoveObjectFromSpawnerMessage msg)
        {
            _created.Remove(msg.ObjectId);
            if (_created.Count <= 0)
            {
                if (_currentWave + 1 < _waves.Length)
                {
                    _currentWave++;
                    if (_ticksBetweenWaves > 0)
                    {
                        _betweenWavesTimer = new TickTimer(_ticksBetweenWaves, 0, _zone.Id);
                        _betweenWavesTimer.OnComplete += TimerFinished;
                    }
                    else
                    {
                        SpawnWave();
                    }
                }
                else
                {
                    _currentWave = 0;
                    this.SendMessageTo(new ApplyObjectiveItemCompletetionMessage{Objective = this, Count = 1}, _parent);
                }
            }
        }
    }
}