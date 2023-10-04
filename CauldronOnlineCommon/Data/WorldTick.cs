using System;

namespace CauldronOnlineCommon.Data
{
    [Serializable]
    public struct WorldTick : IComparable
    {
        public const int TICK_MAX = 1000000000;

        public int Tick;
        public int Age;

        public void AddTick()
        {
            Tick++;
            if (Tick >= TICK_MAX)
            {
                Tick = 0;
                Age++;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is WorldTick other && Equals(other);
        }

        public bool Equals(WorldTick other)
        {
            return Tick == other.Tick && Age == other.Age;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Tick * 397) ^ Age;
            }
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case WorldTick tick when tick < this:
                    return 1;
                case WorldTick tick when tick > this:
                    return -1;
                case WorldTick tick when tick == this:
                    return 0;
                default:
                    return 1;
            }
        }

        public static bool operator ==(WorldTick a, WorldTick b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(WorldTick a, WorldTick b)
        {
            return !(a == b);
        }

        public static bool operator >(WorldTick a, WorldTick b)
        {
            if (a.Age == b.Age)
            {
                return a.Tick > b.Tick;
            }
            return a.Age > b.Age;
        }

        public static bool operator <(WorldTick a, WorldTick b)
        {
            if (a.Age == b.Age)
            {
                return a.Tick < b.Tick;
            }

            return a.Age < b.Age;
        }

        public static bool operator <=(WorldTick a, WorldTick b)
        {
            if (a.Age == b.Age)
            {
                return a.Tick <= b.Tick;
            }
            return a.Age < b.Age;
        }

        public static bool operator >=(WorldTick a, WorldTick b)
        {
            if (a.Age == b.Age)
            {
                return a.Tick >= b.Tick;
            }
            return a.Age > b.Age;
        }
    }
}