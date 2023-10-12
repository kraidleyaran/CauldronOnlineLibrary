using System;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Zones;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class SpawnObjectTraitData : WorldTraitData
    {
        public const string TYPE = "SpawnObject";
        public override string Type => TYPE;

        public ZoneSpawnData Spawn;
    }
}