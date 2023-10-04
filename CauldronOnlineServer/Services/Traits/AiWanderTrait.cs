using System;
using System.Collections.Generic;
using System.Linq;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;
using MessageBusLib;

namespace CauldronOnlineServer.Services.Traits
{
    public class AiWanderTrait : WorldTrait
    {
        private int _wanderRange = 1;
        private bool _anchor = true;
        private ZoneTile _anchorTile = null;
        private WorldIntRange _idleTicks = new WorldIntRange(0,0);
        private float _chanceToIdle = 0f;
        private float _diagonalCost = 0f;

        private AiState _aiState = AiState.Idle;
        private List<ZoneTile> _currentPath = new List<ZoneTile>();
        private TickTimer _idleTimer = null;

        private bool _knockbackActive = false;

        public AiWanderTrait(WorldTraitData data) : base(data)
        {
            if (data is AiWanderTraitData wanderData)
            {
                _wanderRange = wanderData.WanderRange;
                _anchor = wanderData.Anchor;
                _idleTicks = wanderData.IdleTicks;
                _chanceToIdle = wanderData.ChanceToIdle;
                _diagonalCost = wanderData.DiagonalCost;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            if (_anchor)
            {
                _anchorTile = _parent.Tile;
                if (_anchorTile == null)
                {
                    _anchor = false;
                }
            }
            SubscribeToMessages();
        }

        private void IdleTimerFinished()
        {
            _idleTimer.Destroy();
            _idleTimer = null;
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<ZoneUpdateTickMessage>(ZoneUpdateTick, _parent.ZoneId);

            _parent.SubscribeWithFilter<UpdateAiStateMessage>(UpdateAiState, _id);
            _parent.SubscribeWithFilter<ZoneTileUpdatedMessage>(ZoneTileUpdated, _id);
            _parent.SubscribeWithFilter<ApplyKnockbackMessage>(ApplyKnockback, _id);
            _parent.SubscribeWithFilter<KnockbackFinishedMessage>(KnockbackFinished, _id);
            
        }

        private void UpdateAiState(UpdateAiStateMessage msg)
        {
            _aiState = msg.State;
            if (_aiState != AiState.Wander)
            {
                if (_currentPath.Count > 0)
                {
                    _currentPath.Clear();
                }

                if (_idleTimer != null)
                {
                    _idleTimer.Destroy();
                    _idleTimer = null;
                }
                
            }
        }

        private void ZoneTileUpdated(ZoneTileUpdatedMessage msg)
        {
            if (_aiState == AiState.Wander && _currentPath.Count > 0)
            {
                var index = _currentPath.IndexOf(_parent.Tile);
                if (index >= 0)
                {
                    _currentPath.RemoveRange(0, index + 1);
                }

                if (_currentPath.Count <= 0)
                {
                    this.SendMessageTo(new SetAiStateMessage{State = AiState.Idle}, _parent);
                }
            }
        }

        private void ZoneUpdateTick(ZoneUpdateTickMessage msg)
        {
            if ((_aiState == AiState.Idle || _aiState == AiState.Wander) && _currentPath.Count <= 0 && !_knockbackActive && _idleTimer == null)
            {
                var idle = RNGService.Roll(_chanceToIdle);
                if (idle)
                {
                    var ticks = _idleTicks.Roll(true);
                    if (ticks > 0)
                    {
                        _idleTimer = new TickTimer(_idleTicks.Roll(true), 0, _parent.ZoneId);
                        _idleTimer.OnComplete += IdleTimerFinished;
                        if (_aiState != AiState.Wander)
                        {
                            this.SendMessageTo(new SetAiStateMessage { State = AiState.Wander }, _parent);
                        }
                    }
                }
                else
                {
                    if (_aiState != AiState.Wander)
                    {
                        this.SendMessageTo(new SetAiStateMessage { State = AiState.Wander }, _parent);
                    }

                    var zone = ZoneService.GetZoneById(_parent.ZoneId);
                    if (zone != null)
                    {
                        var wanderTiles = _anchor ? zone.GetTilesInSquareArea(_anchorTile, _wanderRange) : zone.GetTilesInSquareArea(_parent.Tile, _wanderRange);
                        if (Array.IndexOf(wanderTiles, _parent.Tile) >= 0)
                        {
                            var tiles = wanderTiles.ToList();
                            tiles.Remove(_parent.Tile);
                            wanderTiles = tiles.ToArray();
                        }
                        if (wanderTiles.Length > 0)
                        {
                            var tile = wanderTiles.Length > 1 ? wanderTiles[RNGService.Range(0, wanderTiles.Length)] : wanderTiles[0];
                            var path = zone.FindPath(_parent.Tile, tile, _wanderRange, _diagonalCost);
                            if (path.Length > 0)
                            {
                                _currentPath = path.ToList();
                                this.SendMessageTo(new SetCurrentPathMessage { Path = path }, _parent);
                            }
                            else
                            {
                                this.SendMessageTo(new SetAiStateMessage { State = AiState.Idle }, _parent);
                            }
                        }
                    }
                }
                
                
            }
        }

        private void ApplyKnockback(ApplyKnockbackMessage msg)
        {
            _knockbackActive = true;
            _currentPath.Clear();
        }

        private void KnockbackFinished(KnockbackFinishedMessage msg)
        {
            _knockbackActive = false;
        }
    }
}