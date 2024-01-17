using System.Linq;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class StaticTeleportTrait : WorldTrait
    {
        private WorldVector2Int[] _tilePositions = new WorldVector2Int[0];
        private bool _isLocalPosition = false;
        private int _teleportTicks = 1;
        private string[] _applyOnTeleportEnd = new string[0];
        private int _ticksAfterTeleport = 0;

        private TickTimer _teleportTimer = null;
        

        public StaticTeleportTrait(WorldTraitData data) : base(data)
        {
            if (data is StaticTeleportTraitData staticTeleport)
            {
                _tilePositions = staticTeleport.Tiles;
                _teleportTicks = staticTeleport.TeleportTicks;
                _isLocalPosition = staticTeleport.IsLocalPosition;
                _applyOnTeleportEnd = staticTeleport.ApplyOnTeleportEnd;
                _ticksAfterTeleport = staticTeleport.AfterTicks;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            var positions = _tilePositions.ToList();
            if (_isLocalPosition)
            {
                for (var i = 0; i < _tilePositions.Length; i++)
                {
                    var pos = _tilePositions[i] + _parent.Tile.Position;
                    if (pos == _parent.Tile.Position)
                    {
                        positions.Remove(_tilePositions[i]);
                    }
                    else
                    {
                        positions[i] = pos;
                    }
                }
            }
            else
            {
                for (var i = 0; i < _tilePositions.Length; i++)
                {
                    if (_tilePositions[i] == _parent.Tile.Position)
                    {
                        positions.Remove(_tilePositions[i]);
                    }
                }
            }
            _tilePositions = positions.ToArray();
            if (_tilePositions.Length > 0)
            {
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    var tilePosition = _tilePositions.Length > 1 ? _tilePositions[RNGService.Range(0, _tilePositions.Length)] : _tilePositions[0];
                    var tile = zone.GetTile(tilePosition);
                    if (tile != null)
                    {
                        _parent.SetObjectState(WorldObjectState.Teleporting);
                        _parent.Tile = tile;
                        _parent.SetPosition(tile.WorldPosition);
                        this.SendMessageTo(ZoneTileUpdatedMessage.INSTANCE, _parent);
                        zone.EventManager.RegisterEvent(new TeleportEvent { Position = tile.WorldPosition, ObjectId = _parent.Data.Id });
                        zone.EventManager.RegisterEvent(new ObjectStateEvent { Active = false, TargetId = _parent.Data.Id });
                        _teleportTimer = new TickTimer(_teleportTicks, 0, _parent.ZoneId);
                        _teleportTimer.OnComplete += TeleportFinished;
                    }
                    else
                    {
                        _parent.RemoveTrait(this);
                    }
                }
                else
                {
                    _parent.RemoveTrait(this);
                }
            }
            else
            {
                _parent.RemoveTrait(this);
            }
            
        }

        private void TeleportFinished()
        {
            _teleportTimer.Destroy();
            _teleportTimer = null;
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.EventManager.RegisterEvent(new ObjectStateEvent { Active = true, TargetId = _parent.Data.Id });
            }

            var traits = TraitService.GetWorldTraits(_applyOnTeleportEnd);
            foreach (var trait in traits)
            {
                _parent.AddTrait(trait);
            }

            if (_ticksAfterTeleport > 0)
            {
                _teleportTimer = new TickTimer(_ticksAfterTeleport, 0, _parent.ZoneId);
                _teleportTimer.OnComplete += AfterTimerFinished;
            }
            else
            {
                if (_parent.State == WorldObjectState.Teleporting)
                {
                    _parent.SetObjectState(WorldObjectState.Active);
                }
                _parent.RemoveTrait(this);
            }
            
        }

        private void AfterTimerFinished()
        {
            _teleportTimer.Destroy();
            _teleportTimer = null;
            if (_parent.State == WorldObjectState.Teleporting)
            {
                _parent.SetObjectState(WorldObjectState.Active);
            }
            _parent.RemoveTrait(this);
        }

        public override void Destroy()
        {
            if (_teleportTimer != null)
            {
                _teleportTimer.Destroy();
                _teleportTimer = null;
                if (_parent.State == WorldObjectState.Teleporting)
                {
                    _parent.State = WorldObjectState.Active;
                }
            }


            base.Destroy();
        }
    }
}