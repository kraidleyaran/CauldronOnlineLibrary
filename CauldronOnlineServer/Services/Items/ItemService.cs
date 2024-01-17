using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using CauldronOnlineCommon.Data.Items;
using CauldronOnlineServer.Logging;
using FileDataLib;

namespace CauldronOnlineServer.Services.Items
{
    public class ItemService : WorldService
    {
        public const string NAME = "Item";
        public const string DROPPED_ITEM = "DroppedItem";

        private static ItemService _instance = null;

        public override string Name => NAME;

        private string _lootTablePath;

        private Dictionary<string, LootTableData> _lootTables = new Dictionary<string, LootTableData>();
        private ConcurrentDictionary<string, int> _keyItems = new ConcurrentDictionary<string, int>();

        public ItemService(string lootTablePath)
        {
            _lootTablePath = lootTablePath;
        }

        public override void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                if (Directory.Exists(_lootTablePath))
                {
                    var files = Directory.GetFiles(_lootTablePath, $"*.{LootTableData.EXTENSION}");
                    foreach (var file in files)
                    {
                        var result = FileData.LoadData<LootTableData>(file);
                        if (result.Success)
                        {
                            if (!_lootTables.ContainsKey(result.Data.Name))
                            {
                                _lootTables.Add(result.Data.Name, result.Data);
                            }
                            else
                            {
                                Log($"Duplicate loot table detected - {result.Data.Name}", LogType.Warning);
                            }
                        }
                        else
                        {
                            Log($"Error while loading loot table at path {file} - {(result.HasException ? $"{result.Exception}" : "Unknown error")}");
                        }
                    }
                }

                Log($"Loaded {_lootTables.Count} Loot Tables");
                base.Start();
            }
            
        }

        public static LootTableData GetLootTable(string tableName)
        {
            if (_instance._lootTables.TryGetValue(tableName, out var table))
            {
                return table;
            }

            return null;
        }

        public static void AddKeyItem(string item, int stack)
        {
            if (_instance._keyItems.TryGetValue(item, out var currentStack))
            {
                _instance._keyItems.TryUpdate(item, stack + currentStack, currentStack);
            }
            else
            {
                _instance._keyItems.TryAdd(item, stack);
            }
        }

        public static void RemoveKeyItem(string item, int stack)
        {
            if (_instance._keyItems.TryGetValue(item, out var currentStack))
            {
                _instance._keyItems.TryUpdate(item, Math.Max(0, currentStack - stack), currentStack);
            }
        }

        public static bool HasKeyItem(string item, int stack = 1)
        {
            if (_instance._keyItems.TryGetValue(item, out var currentStack))
            {
                return currentStack >= stack;
            }

            return false;
        }

        public static string GenerateDroppedItemId(string ownerId)
        {
            return $"{ownerId}-{DROPPED_ITEM}-{Guid.NewGuid().ToString()}";
        }
    }
}