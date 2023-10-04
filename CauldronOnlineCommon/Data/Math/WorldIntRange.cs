using System;

namespace CauldronOnlineCommon.Data.Math
{
    [Serializable]
    public struct WorldIntRange
    {
        public int Min;
        public int Max;

        public WorldIntRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public static WorldIntRange operator +(WorldIntRange range) => range;

        public static WorldIntRange operator +(WorldIntRange a, WorldIntRange b)
        {
            return new WorldIntRange(a.Min + b.Min, a.Max + b.Max);
        }

        public static WorldIntRange operator -(WorldIntRange range) => new WorldIntRange(range.Min * -1, range.Max * -1);

        public static WorldIntRange operator -(WorldIntRange a, WorldIntRange b)
        {
            return new WorldIntRange(a.Min - b.Min, a.Max - b.Max);
        }
    }
}