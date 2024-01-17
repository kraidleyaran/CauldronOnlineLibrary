using System.Collections.Concurrent;
using System.Collections.Generic;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Requests;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Zones.Managers
{
    public class MinimapManager : WorldManager
    {
        private List<WorldVector2Int> _exploredTiles = new List<WorldVector2Int>();

        private ConcurrentQueue<ExploreTilesRequest> _exploreRequests = new ConcurrentQueue<ExploreTilesRequest>();

        private string _zoneId = string.Empty;

        public MinimapManager(string zoneId)
        {
            _zoneId = zoneId;
            SubscribeToMessages();
        }

        public void AddPositionRequest(WorldVector2Int[] positions)
        {
            _exploreRequests.Enqueue(new ExploreTilesRequest(positions));
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<ZoneUpdateTickMessage>(ZoneUpdateTick, _zoneId);
        }

        private void ZoneUpdateTick(ZoneUpdateTickMessage msg)
        {
            if (_exploreRequests.Count > 0)
            {
                var addedPositions = new List<WorldVector2Int>();
                while (_exploreRequests.TryDequeue(out var request))
                {
                    foreach (var tile in request.Tiles)
                    {
                        if (!_exploredTiles.Contains(tile))
                        {
                            _exploredTiles.Add(tile);
                            addedPositions.Add(tile);
                        }
                    }
                }

                if (addedPositions.Count > 0)
                {
                    var zone = ZoneService.GetZoneById(_zoneId);
                    if (zone != null)
                    {
                        zone.EventManager.RegisterEvent(new MinimapExplorationUpdateEvent{Positions = addedPositions.ToArray()});
                    }
                }
            }
        }

        public override void Destroy()
        {
            _exploredTiles.Clear();
            _exploreRequests = null;
            base.Destroy();
        }
    }
}