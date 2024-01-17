using System.Linq;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class TeleportTrait : WorldTrait
    {
        private int _range = 1;
        private int _teleportTicks = 1;

        private TickTimer _teleportTimer = null;
        private string[] _applyOnTeleportEnd = new string[0];

        public TeleportTrait(WorldTraitData data) : base(data)
        {
            if (data is TeleportTraitData teleport)
            {
                _range = teleport.Range;
                _teleportTicks = teleport.TeleportTicks;
                _applyOnTeleportEnd = teleport.ApplyOnTeleportEnd;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);

            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                var tiles = zone.GetTilesInPovArea(_parent.Tile, _range);
                if (tiles.Contains(_parent.Tile))
                {
                    var tileList = tiles.ToList();
                    tileList.Remove(_parent.Tile);
                    tiles = tileList.ToArray();
                }
                if (tiles.Length > 0)
                {
                    _parent.SetObjectState(WorldObjectState.Teleporting);
                    var tile = tiles.Length > 1 ? tiles[RNGService.Range(0, tiles.Length)] : tiles[0];
                    _parent.SetPosition(tile.Position);
                    _parent.Tile = tile;
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


        private void TeleportFinished()
        {
            _teleportTimer.Destroy();
            _teleportTimer = null;
            _parent.SetObjectState(WorldObjectState.Active);
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
            _parent.RemoveTrait(this);
        }
    }
}