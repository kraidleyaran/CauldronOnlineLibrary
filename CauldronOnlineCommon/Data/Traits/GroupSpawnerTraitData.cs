using System;
using CauldronOnlineCommon.Data.Zones;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class GroupSpawnerTraitData : WorldTraitData
    {
        public const string TYPE = "GroupSpawner";
        public override string Type => TYPE;

        public int SpawnEvery;
        public float ChanceToSpawn;
        public float BonusPerMissedChance;
        public ZoneSpawnData[] Objects;
        public bool ApplyStateToChildren;
        public string[] AdditionalTraits = new string[0];
    }
}