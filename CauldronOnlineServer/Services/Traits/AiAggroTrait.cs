using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Requests;
using CauldronOnlineServer.Services.Combat;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class AiAggroTrait : WorldTrait
    {
        private int _aggroRange = 1;
        private int _defaultAggro = 0;

        private Dictionary<string, int> _aggrod = new Dictionary<string, int>();
        private List<string> _aware = new List<string>();
        private string _currentTarget = string.Empty;
        private List<ZoneTile> _currentPath = new List<ZoneTile>();
        private ZoneTile _lastTargetTile = null;
        private List<ZoneTile> _pov = new List<ZoneTile>();
        private string[] _applyOnAggro = new string[0];
        private float _diagonalCost = 0f;

        private ConcurrentQueue<AggroRequest> _aggroRequests = new ConcurrentQueue<AggroRequest>();

        private AiState _aiState = AiState.Idle;

        private bool _knockbackActive = false;

        private WorldTrait[] _appliedTraits = new WorldTrait[0];

        public AiAggroTrait(WorldTraitData data) : base(data)
        {
            if (data is AiAggroTraitData aggroData)
            {
                _aggroRange = aggroData.AggroRange;
                _defaultAggro = aggroData.DefaultAggro;
                _applyOnAggro = aggroData.ApplyOnAggro;
                _diagonalCost = aggroData.DiagonalCost;
            }
        }

        private bool IsTargetAggrod(WorldObject target)
        {
            return _pov.Contains(target.Tile);
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(new AggroParameter{AggroRange = _aggroRange});
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<ZoneUpdateTickMessage>(ZoneUpdateTick, _parent.ZoneId);
            _parent.SubscribeWithFilter<UpdateAiStateMessage>(UpdateAiState, _id);
            _parent.SubscribeWithFilter<ZoneTileUpdatedMessage>(ZoneTileUpdated, _id);
            _parent.SubscribeWithFilter<AggroRequestMessage>(AggroRequest, _id);
            _parent.SubscribeWithFilter<ObjectStateUpdatedMessage>(ObjectStateUpdated, _id);
            _parent.SubscribeWithFilter<ApplyKnockbackMessage>(Applyknockback, _id);
            _parent.SubscribeWithFilter<KnockbackFinishedMessage>(KnockbackFinished, _id);
            _parent.SubscribeWithFilter<TakeDamageMessage>(TakeDamage, _id);
            _parent.SubscribeWithFilter<RemoveFromAggroMessage>(RemoveFromAggro, _id);
        }

        private void ZoneUpdateTick(ZoneUpdateTickMessage msg)
        {
            if (_parent.State == WorldObjectState.Active && !_knockbackActive)
            {
                var removedFromAggroMsg = new RemoveFromAggroMessage{OwnerId = _parent.Data.Id};
                var aggrod = _aggrod.ToArray();
                foreach (var obj in aggrod)
                {
                    if (obj.Value <= 0)
                    {
                        if (!_aware.Contains(obj.Key))
                        {
                            _aggrod.Remove(obj.Key);
                            if (_currentTarget == obj.Key)
                            {
                                if (_currentPath.Count > 0)
                                {
                                    _currentPath.Clear();
                                    this.SendMessageTo(ClearCurrentPathMessage.INSTANCE, _parent);
                                }
                                WorldServer.Log($"[AGGRO] - Aggro dropped - {obj.Value}");
                                _currentTarget = string.Empty;
                                _lastTargetTile = null;
                            }
                            this.SendMessageWithFilter(removedFromAggroMsg, obj.Key);
                        }
                        
                        
                    }
                }

                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    if (_aggroRequests.Count > 0)
                    {
                        while (_aggroRequests.TryDequeue(out var request))
                        {
                            if (request.Remove)
                            {
                                if (_aware.Contains(request.Target))
                                {
                                    _aware.Remove(request.Target);
                                    request.DoAfer.Invoke(true, _parent.Data.Id);
                                }
                            }
                            else if (zone.ObjectManager.TryGetObjectById(request.Target, out var target))
                            {
                                if (!_aware.Contains(target.Data.Id))
                                {
                                    _aware.Add(target.Data.Id);
                                }
                                if (_pov.Contains(target.Tile))
                                {
                                    if (!_aggrod.ContainsKey(request.Target))
                                    {
                                        _aggrod.Add(request.Target, _defaultAggro);
                                        request.DoAfer.Invoke(false, _parent.Data.Id);
                                    }

                                }
                            }

                        }
                    }

                    var aware = _aware.ToArray();
                    foreach (var obj in aware)
                    {
                        if (!_aggrod.ContainsKey(obj) && zone.ObjectManager.TryGetObjectById(obj, out var target))
                        {
                            if (_pov.Contains(target.Tile))
                            {
                                if (!_aggrod.ContainsKey(target.Data.Id))
                                {
                                    _aggrod.Add(target.Data.Id, _defaultAggro);
                                }
                            }
                            else
                            {
                                var distance = _parent.Tile.Position.Distance(target.Tile.Position);
                                if (distance > _aggroRange)
                                {
                                    _aware.Remove(obj);
                                }
                            }
                        }
                    }


                    if (string.IsNullOrEmpty(_currentTarget) || !_aggrod.ContainsKey(_currentTarget))
                    {
                        if (_aggrod.Count > 0)
                        {
                            var ordered = _aggrod.OrderByDescending(a => a.Value).FirstOrDefault();
                            _currentTarget = ordered.Key;
                            _currentPath.Clear();
                            if (_aiState != AiState.Aggro)
                            {
                                this.SendMessageTo(new SetAiStateMessage { State = AiState.Aggro }, _parent);
                            }
                        }
                        else if (!string.IsNullOrEmpty(_currentTarget))
                        {
                            _currentTarget = string.Empty;
                            _currentPath.Clear();
                            _lastTargetTile = null;
                            if (_aiState == AiState.Aggro)
                            {
                                this.SendMessageTo(new SetAiStateMessage { State = AiState.Idle }, _parent);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(_currentTarget))
                    {
                        if (_aiState == AiState.Aggro)
                        {
                            this.SendMessageTo(new SetAiStateMessage { State = AiState.Idle }, _parent);
                        }
                    }
                    else
                    {
                        var usedAbility = false;
                        if (zone.ObjectManager.TryGetObjectById(_currentTarget, out var target))
                        {
                            var abilityCheckMsg = new AbilityCheckMessage
                            {
                                Target = target,
                                Distance = _parent.Tile.Position.Distance(target.Tile.Position),
                                Direction = _parent.Data.Position.FaceDirection(target.Data.Position),
                                DoAfter = (abilityUsed) => usedAbility = abilityUsed
                            };
                            this.SendMessageTo(abilityCheckMsg, _parent);
                        }

                        if (!usedAbility)
                        {
                            if (_currentPath.Count <= 0)
                            {
                                if (_aiState != AiState.Aggro)
                                {
                                    this.SendMessageTo(new SetAiStateMessage { State = AiState.Aggro }, _parent);
                                }

                                if (zone.ObjectManager.TryGetObjectById(_currentTarget, out var targetObj))
                                {
                                    _lastTargetTile = targetObj.Tile;
                                    var path = zone.FindPath(_parent.Tile, _lastTargetTile, _aggroRange, _diagonalCost);
                                    if (path.Length > 0)
                                    {
                                        _currentPath = path.ToList();
                                        this.SendMessageTo(new SetCurrentPathMessage { Path = path }, _parent);
                                    }
                                }
                                else
                                {
                                    _aggrod.Remove(_currentTarget);
                                    _currentTarget = string.Empty;
                                    _lastTargetTile = null;
                                }
                            }
                            else
                            {
                                if (zone.ObjectManager.TryGetObjectById(_currentTarget, out var targetObj))
                                {
                                    if (!_pov.Contains(targetObj.Tile))
                                    {
                                        _aggrod[_currentTarget] += CombatService.Settings.AggroPerPovCheck();
                                        WorldServer.Log($"[AGGRO] - Aggro reduced - Not in Pov - {_aggrod[_currentTarget]}");
                                    }
                                    if (_lastTargetTile == null || _lastTargetTile != targetObj.Tile)
                                    {
                                        _lastTargetTile = targetObj.Tile;
                                    }

                                    if (_currentPath[_currentPath.Count - 1] != _lastTargetTile)
                                    {
                                        var tileIndex = _currentPath.IndexOf(_lastTargetTile);
                                        if (tileIndex < 0)
                                        {
                                            var targetDirection = _parent.Tile.Position.Direction(_lastTargetTile.Position);
                                            var currentDirection = _parent.Tile.Position.Direction(_currentPath[_currentPath.Count - 1].Position);
                                            if (targetDirection != currentDirection)
                                            {
                                                _aggrod[_currentTarget] += CombatService.Settings.AggroPerPathChangeCheck(2);
                                                _currentPath.Clear();
                                                WorldServer.Log($"[AGGRO] - Path Cleared - Different directions - {_aggrod[_currentTarget]}");
                                                this.SendMessageTo(ClearCurrentPathMessage.INSTANCE, _parent);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    _lastTargetTile = null;
                                    _aggrod.Remove(_currentTarget);
                                    _aware.Remove(_currentTarget);
                                    _currentTarget = string.Empty;
                                    _currentPath.Clear();
                                    this.SendMessageTo(ClearCurrentPathMessage.INSTANCE, _parent);
                                }

                            }

                        }

                    }
                }
                
                
            }
            
        }

        private void UpdateAiState(UpdateAiStateMessage msg)
        {
            var prevState = _aiState;
            _aiState = msg.State;
            if (_aiState != AiState.Aggro && _currentPath.Count > 0)
            {
                _currentPath.Clear();
                if (_appliedTraits.Length > 0)
                {
                    foreach (var trait in _appliedTraits)
                    {
                        _parent.RemoveTrait(trait);
                    }
                    _appliedTraits = new WorldTrait[0];
                }
            }
            else if (prevState != AiState.Aggro && _aiState == AiState.Aggro && _appliedTraits.Length <= 0)
            {
                if (_applyOnAggro.Length > 0)
                {
                    var traits = TraitService.GetWorldTraits(_applyOnAggro);
                    var applied = new List<WorldTrait>();
                    foreach (var trait in traits)
                    {
                        _parent.AddTrait(trait);
                        if (!trait.Instant)
                        {
                            applied.Add(trait);
                        }
                    }

                    _appliedTraits = applied.ToArray();
                }
            }
        }

        private void ZoneTileUpdated(ZoneTileUpdatedMessage msg)
        {
            if (_aiState == AiState.Aggro)
            {
                if (_currentPath.Count > 0 && _currentPath[0] == _parent.Tile)
                {
                    _currentPath.RemoveAt(0);
                }
            }
            else if (_currentPath.Count > 0)
            {
                _currentPath.Clear();
            }

            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                _pov = zone.GetTilesInPovArea(_parent.Tile, _aggroRange).ToList();
                this.SendMessageTo(new UpdatePovMessage{Pov = _pov.ToArray()}, _parent);
            }
        }

        private void AggroRequest(AggroRequestMessage msg)
        {
            _aggroRequests.Enqueue(msg.Request);
        }

        private void ObjectStateUpdated(ObjectStateUpdatedMessage msg)
        {
            if (_parent.State != WorldObjectState.Active && _parent.State != WorldObjectState.Attacking)
            {
                _aggrod.Clear();
                _currentTarget = string.Empty;
                _currentPath.Clear();
                _lastTargetTile = null;
            }
        }

        private void Applyknockback(ApplyKnockbackMessage msg)
        {
            _knockbackActive = true;
            if (_aiState == AiState.Aggro)
            {
                _currentPath.Clear();
                _lastTargetTile = null;
            }
        }

        private void KnockbackFinished(KnockbackFinishedMessage msg)
        {
            _knockbackActive = false;
        }

        private void TakeDamage(TakeDamageMessage msg)
        {
            var aggro = CombatService.Settings.AggroPerDamage(msg.Amount);
            if (!_aggrod.TryGetValue(msg.OwnerId, out var amount))
            {
                _aggrod.Add(msg.OwnerId, _defaultAggro);
            }

            _aggrod[msg.OwnerId] += aggro;

            if (_aware.Contains(msg.OwnerId))
            {
                _aware.Remove(msg.OwnerId);
            }
        }

        private void RemoveFromAggro(RemoveFromAggroMessage msg)
        {
            _aggrod.Remove(msg.OwnerId);
            _aware.Remove(msg.OwnerId);
            if (_currentTarget == msg.OwnerId)
            {
                _currentPath.Clear();
                this.SendMessageTo(ClearCurrentPathMessage.INSTANCE, _parent);
                _currentTarget = string.Empty;
            }
        }

        public override void Destroy()
        {
            _parent?.RemoveParameter(AggroParameter.TYPE);
            base.Destroy();
        }
    }
}