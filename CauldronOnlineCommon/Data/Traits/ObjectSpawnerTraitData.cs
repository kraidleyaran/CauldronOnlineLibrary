using System;
using CauldronOnlineCommon.Data.Zones;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class ObjectSpawnerTraitData : WorldTraitData
    {
        public const string TYPE = "ObjectSpawner";
        public override string Type => TYPE;

        public ObjectSpawnData SpawnData;
        public int SpawnArea = 1;
        public int SpawnEvery = 1;
        public int MaxSpawns = 1;
        public float ChanceToSpawn = 1f;
        public float BonusPerMissedChance = 0f;
        public bool InitialSpawn = false;
    }
}