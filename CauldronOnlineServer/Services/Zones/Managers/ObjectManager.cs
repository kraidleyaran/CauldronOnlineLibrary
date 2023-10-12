using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Requests;
using CauldronOnlineServer.Services.Traits;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Zones.Managers
{
    public class ObjectManager : WorldManager
    {
        private const string OBJECT = "Obj";

        private Dictionary<string, WorldObject> _objects = new Dictionary<string, WorldObject>();
        private Dictionary<string, string> _playerIdLookup = new Dictionary<string, string>();
        private Dictionary<string, string> _reversePlayerLookup = new Dictionary<string, string>();

        private string _zoneId = string.Empty;
        private WorldVector2Int _defaultSpawn = WorldVector2Int.Zero;

        private ConcurrentQueue<CreatePlayerObjectRequest> _createPlayerRequests = new ConcurrentQueue<CreatePlayerObjectRequest>();
        private ConcurrentQueue<DestroyObjectRequest> _destroyObjectRequests = new ConcurrentQueue<DestroyObjectRequest>();
        private ConcurrentQueue<CreateObjectRequest> _createObjectRequests = new ConcurrentQueue<CreateObjectRequest>();

        public ObjectManager(string zoneId, WorldVector2Int defaultSpawn)
        {
            _zoneId = zoneId;
            _defaultSpawn = defaultSpawn;
            SubscribeToMessages();
        }

        public string GetPlayerObjectIdByWorldId(string worldId)
        {
            if (_playerIdLookup.TryGetValue(worldId, out var objId))
            {
                return objId;
            }

            return string.Empty;
        }

        public bool IsPlayer(string objectId)
        {
            return _reversePlayerLookup.ContainsKey(objectId);
        }

        public WorldObject[] GetPlayersInZone()
        {
            var playerIds = _reversePlayerLookup.Keys.ToArray();
            var returnObjects = new List<WorldObject>();
            foreach (var id in playerIds)
            {
                if (_objects.TryGetValue(id, out var obj))
                {
                    returnObjects.Add(obj);
                }
            }

            return returnObjects.ToArray();
        }

        public void RequestPlayerObject(ClientCharacterData data, WorldVector2Int position, int connectionId, string worldId, Action<string, string, WorldVector2Int> doAfter)
        {
            _createPlayerRequests.Enqueue(new CreatePlayerObjectRequest(data, position, connectionId, worldId, doAfter));
        }

        public void RequestDestroyObject(string id, string playerId = "", Action action = null)
        {
            _destroyObjectRequests.Enqueue(new DestroyObjectRequest(id, playerId, action));
        }

        public void RequestObject(string displayName, string[] traits, bool showName, ObjectParameter[] parameters, WorldVector2Int position, bool isMonster, Action<WorldObject> doAfter, bool showOnClient, bool instant = false, bool showAppearance = false)
        {
            if (instant)
            {
                GenerateObject(displayName, traits, showName, parameters, position, isMonster, doAfter, showOnClient, showAppearance);
            }
            else
            {
                _createObjectRequests.Enqueue(new CreateObjectRequest(displayName, traits, showName, parameters, position, isMonster, doAfter, showOnClient, showAppearance));
            }
            
        }

        public bool TryGetObjectById(string id, out WorldObject obj)
        {
            return _objects.TryGetValue(id, out obj);
        }

        public ClientObjectData[] GetObjectData()
        {
            var data = new List<ClientObjectData>();
            foreach (var obj in _objects.Values)
            {
                if (obj.ShowOnClient)
                {
                    data.Add(obj.Data);
                }
            }

            return data.ToArray();
        }

        private string GenerateId()
        {
            var id = $"{_zoneId}-{OBJECT}-{Guid.NewGuid().ToString()}";
            while (_objects.ContainsKey(id))
            {
                id = $"{_zoneId}-{OBJECT}-{Guid.NewGuid().ToString()}";
            }

            return id;
        }

        private void GeneratePlayerObject(CreatePlayerObjectRequest request)
        {
            var id = GenerateId();
            var pos = request.Position;
            var zone = ZoneService.GetZoneById(_zoneId);
            if (zone != null)
            {
                if (!zone.IsValidPosition(pos))
                {
                    pos = _defaultSpawn;
                }

                var tile = zone.GetTileByWorldPosition(pos);
                var obj = new WorldObject(id, request.Data.DisplayName, pos, tile, _zoneId)
                { ShowOnClient = true };
                obj.Data.ShowName = true;
                obj.AddTrait(new SpriteTrait(request.Data.Sprite));
                obj.AddTrait(new PlayerTrait(request.ConnectionId, request.WorldId));
                obj.AddTrait(new CombatStatsTrait(request.Data.Stats, request.Data.Vitals));
                obj.AddTrait(new PlayerMovementTrait(request.WorldId));
                _objects.Add(obj.Data.Id, obj);
                _playerIdLookup.Add(request.WorldId, id);
                _reversePlayerLookup.Add(id, request.WorldId);
                zone.EventManager.RegisterEvent(new ObjectCreatedEvent{Data = obj.Data, ShowAppearance = true});
                request.DoAfter?.Invoke(id, zone.Name, pos);
            }

        }

        private void GenerateObject(string displayName, string[] traits, bool showName, ObjectParameter[] parameters, WorldVector2Int position, bool isMonster, Action<WorldObject> doAfter, bool showOnClient, bool showAppearance)
        {
            var id = GenerateId();
            var zone = ZoneService.GetZoneById(_zoneId);
            if (zone != null)
            {
                var tile = zone.GetTileByWorldPosition(position);
                if (tile == null)
                {
                    WorldServer.Log($"[ObjectManager] - Invalid tile position {position}");
                    tile = zone.GetTile(zone.DefaultSpawn);
                }
                var obj = new WorldObject(id, displayName, position, tile, _zoneId);

                foreach (var traitName in traits)
                {
                    var trait = TraitService.GetTraitByName(traitName);
                    if (trait != null)
                    {
                        obj.AddTrait(trait, obj);
                    }
                }

                if (parameters.Length > 0)
                {
                    ApplyParameters(parameters, obj);
                }
                _objects.Add(id, obj);
                obj.ShowOnClient = showOnClient;
                obj.Data.IsMonster = isMonster;
                obj.Data.ShowName = showName;
                if (obj.ShowOnClient)
                {
                    zone.EventManager.RegisterEvent(new ObjectCreatedEvent { Data = obj.Data, ShowAppearance = showAppearance});
                }
                doAfter?.Invoke(obj);
            }
        }

        private void ApplyParameters(ObjectParameter[] parameters, WorldObject obj)
        {
            foreach (var parameter in parameters)
            {
                switch (parameter.Type)
                {
                    case DoorParameter.TYPE:
                        if (parameter is DoorParameter door)
                        {
                            obj.AddTrait(new DoorTrait(door));
                        }
                        break;
                    case DialogueParameter.TYPE:
                        if (parameter is DialogueParameter dialogue)
                        {
                            obj.AddTrait(new DialogueTrait(dialogue));
                        }
                        break;
                    case TriggerEventHitboxParameter.TYPE:
                        if (parameter is TriggerEventHitboxParameter triggerEventHitbox)
                        {
                            obj.AddParameter(triggerEventHitbox);
                        }
                        break;
                    case SwitchParameter.TYPE:
                        if (parameter is SwitchParameter switchParam)
                        {
                            obj.AddTrait(new SwitchTrait(switchParam));
                        }
                        break;
                    case LootChestParameter.TYPE:
                        if (parameter is LootChestParameter lootChest)
                        {
                            obj.AddTrait(new LootChestTrait(lootChest));
                        }
                        break;
                    case KeyItemChestParameter.TYPE:
                        if (parameter is KeyItemChestParameter keyItemChest)
                        {
                            obj.AddTrait(new KeyItemChestTrait(keyItemChest));
                        }
                        break;
                    case ZoneQuestParameter.TYPE:
                        if (parameter is ZoneQuestParameter zoneQuest)
                        {
                            obj.AddTrait(new ZoneQuestTrait(zoneQuest));
                        }
                        break;
                        
                }
            }
        }

        private void DestroyObject(DestroyObjectRequest request)
        {
            if (_objects.TryGetValue(request.ObjectId, out var obj))
            {
                obj.SetObjectState(WorldObjectState.Destroying);
                _objects.Remove(request.ObjectId);
                if (!string.IsNullOrEmpty(request.PlayerId))
                {
                    _playerIdLookup.Remove(request.PlayerId);
                    _reversePlayerLookup.Remove(request.ObjectId);
                }
                obj.SetObjectState(WorldObjectState.Destroyed);
                obj.Destroy();
            }
            request.DoAfter?.Invoke();
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<ZoneUpdateTickMessage>(ZoneUpdateTick, _zoneId);
            this.SubscribeWithFilter<ZoneResolveTickMessage>(ZoneResolveTick, _zoneId);
        }

        private void ZoneUpdateTick(ZoneUpdateTickMessage msg)
        {
            if (_createPlayerRequests.Count > 0)
            {
                while (_createPlayerRequests.TryDequeue(out var request))
                {
                    GeneratePlayerObject(request);
                }
            }

            if (_createObjectRequests.Count > 0)
            {
                while (_createObjectRequests.TryDequeue(out var request))
                {
                    GenerateObject(request.DisplayName, request.Traits, request.ShowName, request.Parameters, request.Position, request.IsMonster, request.DoAfter, request.ShowOnClient, request.ShowAppearance);
                }
            }
        }

        private void ZoneResolveTick(ZoneResolveTickMessage msg)
        {
            if (_destroyObjectRequests.Count > 0)
            {
                while (_destroyObjectRequests.TryDequeue(out var request))
                {
                    DestroyObject(request);
                }
            }
        }
        
    }
}