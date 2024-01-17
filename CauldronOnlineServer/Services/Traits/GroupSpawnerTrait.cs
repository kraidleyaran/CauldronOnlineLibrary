using System.Collections.Generic;
using System.Linq;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineCommon.Data.Zones;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class GroupSpawnerTrait : WorldTrait
    {
        private ZoneSpawnData[] _objects = new ZoneSpawnData[0];

        private int _spawnEvery = 0;
        private float _chanceToSpawn = 0f;
        private float _bonusChanceToSpawn = 0f;
        private float _bonusPerMissedChance = 0f;
        private bool _applyStateToChildren = false;
        private string[] _additionalTraits = new string[0];

        private Dictionary<string, WorldObject> _objs = new Dictionary<string, WorldObject>();
        private TickTimer _spawnTimer = null;

        public GroupSpawnerTrait(GroupSpawnParameter parameter)
        {
            Name = parameter.Type;
            _objects = parameter.Objects;
            _spawnEvery = parameter.SpawnEvery;
            _chanceToSpawn = parameter.ChanceToSpawn;
            _bonusPerMissedChance = parameter.BonusOnMissedChance;
            _applyStateToChildren = parameter.ApplyStateToChildren;
            _additionalTraits = parameter.AdditionalTraits;
        }

        public GroupSpawnerTrait(WorldTraitData data) : base(data)
        {
            if (data is GroupSpawnerTraitData groupData)
            {
                _objects = groupData.Objects;
                _spawnEvery = groupData.SpawnEvery;
                _chanceToSpawn = groupData.ChanceToSpawn;
                _bonusPerMissedChance = groupData.BonusPerMissedChance;
                _applyStateToChildren = groupData.ApplyStateToChildren;
                _additionalTraits = groupData.AdditionalTraits;
            }

        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            SubscribeToMessages();
        }

        private void SpawnGroup()
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
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    foreach (var zoneSpawn in _objects)
                    {
                        var tilePos = zoneSpawn.Tile + _parent.Tile.Position;
                        var tile = zone.GetTile(tilePos);
                        if (tile != null)
                        {
                            var objSpawn = zoneSpawn.Spawn;
                            zone.ObjectManager.RequestObject(objSpawn.DisplayName, objSpawn.Traits,
                                objSpawn.ShowOnClient, objSpawn.Parameters, tile.WorldPosition, objSpawn.IsMonster,
                                UnitSpawned, objSpawn.ShowOnClient, true, objSpawn.ShowAppearance,
                                objSpawn.StartActive, objSpawn.MinimapIcon);
                        }
                    }
                }

            }
        }

        private void UnitSpawned(WorldObject obj)
        {
            if (!_objs.ContainsKey(obj.Data.Id))
            {
                _objs.Add(obj.Data.Id, obj);
                obj.AddTrait(new SpawnedObjectTrait(_parent));
                var traits = TraitService.GetWorldTraits(_additionalTraits);
                foreach (var trait in traits)
                {
                    obj.AddTrait(trait);
                }
            }
            
        }

        private void TimerComplete()
        {
            if (_objs.Count <= 0)
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
            if (_applyStateToChildren)
            {
                _parent.SubscribeWithFilter<ObjectStateUpdatedMessage>(ObjectStateUpdated, _id);
            }
        }

        private void RemoveObjectFromSpawner(RemoveObjectFromSpawnerMessage msg)
        {
            _objs.Remove(msg.ObjectId);
        }

        private void ZoneUpdateTick(ZoneUpdateTickMessage msg)
        {
            if (_objs.Count <= 0 && _spawnTimer == null)
            {
                _spawnTimer = new TickTimer(_spawnEvery, 0 , _parent.ZoneId);
                _spawnTimer.OnLoopFinish += SpawnGroup;
                _spawnTimer.OnComplete += TimerComplete;
            }
        }

        private void ObjectStateUpdated(ObjectStateUpdatedMessage msg)
        {
            if (_applyStateToChildren)
            {
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    var objs = _objs.Values.ToArray();
                    foreach (var obj in objs)
                    {
                        obj.SetObjectState(_parent.State);
                        zone.EventManager.RegisterEvent(new ObjectStateEvent{Active = _parent.State == WorldObjectState.Active, TargetId = obj.Data.Id});
                    }
                }

            }
        }
    }
}