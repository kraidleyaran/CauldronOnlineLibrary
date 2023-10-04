using System;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class MonsterExperienceTraitData : WorldTraitData
    {
        public const string TYPE = "MonsterExperience";
        public override string Type => TYPE;

        public WorldIntRange Experience;
    }
}