﻿using System.Collections.Generic;
using System.Linq;
using CauldronOnlineCommon;
using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineServer.Interfaces;
using CauldronOnlineServer.Services.Traits;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Zones
{
    public class WorldObject : IDestroyable
    {
        public string ZoneId;
        public ClientObjectData Data;
        public ZoneTile Tile;
        public bool ShowOnClient;
        public WorldObjectState State;

        private List<WorldTrait> _traits = new List<WorldTrait>();
        private Dictionary<string, ObjectParameter> _parameters = new Dictionary<string, ObjectParameter>();

        public WorldObject(string id, string displayName, WorldVector2Int pos, ZoneTile tile, string zoneId)
        {
            Data = new ClientObjectData
            {
                DisplayName = displayName,
                Id = id,
                Position = pos,
                Parameters = new byte[0][]
            };
            Tile = tile;
            ZoneId = zoneId;
        }

        public void AddTrait(WorldTrait trait, object sender = null)
        {
            var count = _traits.Count(t => t.Name == trait.Name);
            if (count < trait.MaxStack)
            {
                if (!trait.Instant)
                {
                    _traits.Add(trait);
                }
                trait.Setup(this, sender);
            }
        }

        public void RemoveTrait(WorldTrait trait)
        {
            if (_traits.Contains(trait))
            {
                _traits.Remove(trait);
                trait.Destroy();
            }
        }

        public void SetPosition(WorldVector2Int pos)
        {
            Data.Position = pos;
            this.SendMessageTo(new UpdatePositionMessage{Position = Data.Position}, this);
        }

        public void AddParameter<T>(T parameter) where T: ObjectParameter
        {
            if (_parameters.TryGetValue(parameter.Type, out var existing))
            {
                _parameters[parameter.Type] = parameter;
            }
            else
            {
                existing = parameter;
                _parameters.Add(parameter.Type, existing);
            }

            Data.Parameters = _parameters.Values.Select(p => p.ToParameterData()).ToArray();
        }

        public void RemoveParameter(string type)
        {
            _parameters.Remove(type);
            Data.Parameters = _parameters.Values.Select(p => p.ToParameterData()).ToArray();
        }

        public T GetParamter<T>(string type) where T : ObjectParameter
        {
            if (_parameters.TryGetValue(type, out var parameter) && parameter is T typeParameter)
            {
                return typeParameter;
            }

            return null;
        }

        public void SetObjectState(WorldObjectState state)
        {
            if (State != state)
            {
                State = state;
                this.SendMessageTo(ObjectStateUpdatedMessage.INSTANCE, this);
            }
        }

        public void Destroy()
        {
            foreach (var trait in _traits)
            {
                trait.Destroy();
            }
            _traits.Clear();
        }
    }
}