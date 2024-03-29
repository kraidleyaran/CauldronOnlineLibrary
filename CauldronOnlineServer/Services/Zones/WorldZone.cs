﻿using System;
using System.Collections.Generic;
using System.Linq;
using CauldronOnlineCommon;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Zones;
using CauldronOnlineServer.Interfaces;
using CauldronOnlineServer.Services.TriggerEvents;
using CauldronOnlineServer.Services.Zones.Managers;
using ConcurrentMessageBus;
using RogueSharp;

namespace CauldronOnlineServer.Services.Zones
{
    public class WorldZone : IDestroyable
    {
        public WorldZoneData Data;
        public string Id;

        public TickManager Tick;
        public ObjectManager ObjectManager;
        public WorldEventManager EventManager;
        public WorldVector2Int DefaultSpawn;
        public TriggerEventManager TriggerManager;
        //public MinimapManager Minimap;

        private Dictionary<WorldVector2Int, ZoneTile> _tiles = new Dictionary<WorldVector2Int, ZoneTile>();
        private WorldVector2Int _size = WorldVector2Int.Zero;
        private WorldVector2Int _offset = WorldVector2Int.Zero;

        private Map _pathingMap = null;

        public WorldZone(WorldZoneData data)
        {
            Data = data;
        }

        public void Startup(string zoneId)
        {
            Id = zoneId;
            DefaultSpawn = Data.DefaultSpawn;
            SubscribeToMessages();
            ObjectManager = new ObjectManager(Id, Data.DefaultSpawn);
            EventManager = new WorldEventManager(Id);
            TriggerManager = new TriggerEventManager(Id);
            //Minimap = new MinimapManager(Id);
            _pathingMap = new Map(Data.Size.X, Data.Size.Y);
            _offset = Data.Offset;
            _size = Data.Size;
            foreach (var tile in Data.Tiles)
            {
                if (!_tiles.ContainsKey(tile.Position))
                {
                    var cellPos = tile.Position + _offset;
                    var cell = _pathingMap.GetCell(cellPos.X, cellPos.Y);
                    _pathingMap.SetCellProperties(cell.X, cell.Y, true, true);
                    _tiles.Add(tile.Position, new ZoneTile(tile, cell));
                }
            }

            foreach (var spawnData in Data.Spawns)
            {
                var tile = GetTile(spawnData.Tile);
                if (tile != null)
                {
                    ObjectManager.RequestObject(spawnData.Spawn.DisplayName, spawnData.Spawn.Traits, spawnData.Spawn.ShowOnClient, spawnData.Spawn.Parameters, tile.WorldPosition, spawnData.Spawn.IsMonster, null, spawnData.Spawn.ShowOnClient, false, false, spawnData.Spawn.StartActive, spawnData.Spawn.MinimapIcon);
                }
            }

            //This needs to start last so that the tick doesn't go off until we're sure everything is setup
            Tick = new TickManager(WorldServer.Settings.ZoneTick, Id);
        }

        public bool IsValidPosition(WorldVector2Int worldPos)
        {
            return _tiles.ContainsKey(new WorldVector2Int((int)Math.Floor((worldPos.X + CauldronUtils.kEpsilon) / WorldServer.Settings.TileSize), (int)Math.Floor((worldPos.Y + CauldronUtils.kEpsilon) / WorldServer.Settings.TileSize)));
        }

        public ZoneTile FindClosestTile(ZoneTile previousTile, WorldVector2Int currentPos)
        {
            var distance = currentPos.Distance(previousTile.Position);
            var tile = GetTilesInPovArea(previousTile, distance).OrderBy(t => currentPos.SqrDistance(t.Position)).FirstOrDefault();
            if (tile != null)
            {
                return tile;
            }
            return previousTile;
        }

        public ZoneTile FindClosesTileByWorldPosition(WorldVector2Int currentPos, ZoneTile lastTile, int distance)
        {
            var tile = GetTilesInPovArea(lastTile, distance).OrderBy(t => currentPos.SqrDistance(t.Position)).FirstOrDefault();
            if (tile != null)
            {
                return tile;
            }

            return lastTile;
        }

        public ZoneTile GetTileByWorldPosition(WorldVector2Int worldPos)
        {
            
            var tilePos = new WorldVector2Int((int)Math.Floor((worldPos.X + CauldronUtils.kEpsilon) / WorldServer.Settings.TileSize), (int)Math.Floor((worldPos.Y + CauldronUtils.kEpsilon) / WorldServer.Settings.TileSize));
            if (_tiles.TryGetValue(tilePos, out var tile))
            {
                return tile;
            }

            return null;
        }

        public ZoneTile GetTileByCellPosition(WorldVector2Int cell)
        {
            if (_tiles.TryGetValue(cell - _offset, out var tile))
            {
                return tile;
            }

            return null;
        }

        public ZoneTile GetTile(WorldVector2Int tilePos)
        {
            if (_tiles.TryGetValue(tilePos, out var tile))
            {
                return tile;
            }

            return null;
        }

        public ZoneTile[] FindPath(ZoneTile start, ZoneTile end, int area, float diagonalCost = 0f)
        {
            var min = new WorldVector2Int(Math.Min(start.Position.X, end.Position.X), Math.Min(start.Position.Y, end.Position.Y));
            var max = new WorldVector2Int(Math.Max(start.Position.X, end.Position.X), Math.Max(start.Position.Y, end.Position.Y));
            var size = (max - min) + new WorldVector2Int(area, area);
            
            var offset = min;

            var pathingMap = new Map(size.X, size.Y);
            for (var y = 0; y < size.Y; y++)
            {
                for (var x = 0; x < size.X; x++)
                {
                    var tilePos = new WorldVector2Int(x + offset.X, y + offset.Y);
                    var tile = GetTile(tilePos);
                    if (tile != null)
                    {
                        pathingMap.SetCellProperties(x,y, true, true);
                    }
                }
            }
            var startPos = start.Position - offset;
            var endPos = end.Position - offset;
            var startCell = pathingMap.GetCell(startPos.X, startPos.Y);
            var endCell = pathingMap.GetCell(endPos.X, endPos.Y);
            var pathFinder = diagonalCost > 0f ? new PathFinder(pathingMap, diagonalCost) : new PathFinder(pathingMap);
            var path = pathFinder.TryFindShortestPath(startCell, endCell);
            var returnPath = new List<ZoneTile>();
            if (path != null && path.Length > 0)
            {
                foreach (var cell in path.Steps)
                {
                    var cellPos = new WorldVector2Int(cell.X, cell.Y);
                    var tilePos = cellPos + offset;
                    var tile = GetTile(tilePos);
                    if (tile != null)
                    {
                        returnPath.Add(tile);
                    }
                }
            }

            if (returnPath.Count > 0 && returnPath[0] == start)
            {
                returnPath.RemoveAt(0);
            }

            return returnPath.ToArray();
        }

        public ZoneTile[] GetTilesInSquareArea(ZoneTile tile, int area)
        {
            var cells = _pathingMap.GetCellsInSquare(tile.Cell.X, tile.Cell.Y, area);
            var returnTiles = new List<ZoneTile>();
            foreach (var cell in cells)
            {
                var areaTile = GetTileByCellPosition(new WorldVector2Int(cell.X, cell.Y));
                if (areaTile != null)
                {
                    returnTiles.Add(areaTile);
                }
            }
            return returnTiles.ToArray();
        }

        public ZoneTile[] GetTilesInPovArea(ZoneTile tile, int area)
        {
            var cells = _pathingMap.ComputeFov(tile.Cell.X, tile.Cell.Y, area, true);
            var tiles = new List<ZoneTile>();
            foreach (var cell in cells)
            {
                var povTile = GetTileByCellPosition(new WorldVector2Int(cell.X, cell.Y));
                if (povTile != null)
                {
                    tiles.Add(povTile);
                }
            }

            return tiles.ToArray();
        }

        public void SetBlockedTile(WorldVector2Int pos, WorldObject blocker)
        {
            if (_tiles.TryGetValue(pos, out var tile))
            {
                tile.BlockingObject = blocker;
            }
        }

        public void RemoveBlockedTile(WorldVector2Int pos, WorldObject blocker)
        {
            if (_tiles.TryGetValue(pos, out var tile) && tile.Blocked && tile.BlockingObject == blocker)
            {
                tile.BlockingObject = null;
            }
        }

        public ZoneTile[] GetBorderTilesInCircle(ZoneTile pos, int area)
        {
            var cells = _pathingMap.GetBorderCellsInCircle(pos.Cell.X, pos.Cell.Y, area);
            return cells.Select(c => GetTileByCellPosition(new WorldVector2Int(c.X, c.Y))).ToArray();
        }

        public ZoneTile[] GetBorderTilesInSquare(ZoneTile pos, int area)
        {
            var cells = _pathingMap.GetBorderCellsInSquare(pos.Cell.X, pos.Cell.Y, area);
            return cells.Select(c => GetTileByCellPosition(new WorldVector2Int(c.X, c.Y))).ToArray();
        }

        public ZoneTile[] GetTiles(WorldVector2Int[] tiles)
        {
            var returnTiles = new List<ZoneTile>();
            foreach (var tilePos in tiles)
            {
                var tile = GetTile(tilePos);
                if (tile != null)
                {
                    returnTiles.Add(tile);
                }
            }

            return returnTiles.ToArray();
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<ZoneUpdateTickMessage>(ZoneUpdateTick, Id);
        }

        private void ZoneUpdateTick(ZoneUpdateTickMessage msg)
        {
            this.SendMessage(new ZoneCheckInMessage{Zone = Id});
            this.Unsubscribe<ZoneUpdateTickMessage>();
        }

        public void Reset()
        {
            foreach (var triggerEvent in Data.ResetEvents)
            {
                TriggerEventService.ResetEvent(triggerEvent);
            }
            Destroy();

        }

        public void Destroy()
        {
            foreach (var tile in _tiles)
            {
                tile.Value.Destroy();
            }
            _tiles.Clear();

            TriggerManager.Destroy();
            ObjectManager.Destroy();
            Tick.Destroy();
            EventManager.Destroy();
            this.UnsubscribeFromAllMessages();
            Id = string.Empty;
            _pathingMap.Clear();
            _pathingMap = null;
            _offset = WorldVector2Int.Zero;
            _size = WorldVector2Int.Zero;
        }
    }
}