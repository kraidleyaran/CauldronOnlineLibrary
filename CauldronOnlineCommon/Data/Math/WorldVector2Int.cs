using System;
using System.Collections.Generic;

namespace CauldronOnlineCommon.Data.Math
{
    [Serializable]
    public struct WorldVector2Int
    {
        public static WorldVector2Int Zero = new WorldVector2Int(0, 0);
        public static WorldVector2Int One = new WorldVector2Int(1, 1);
        public static WorldVector2Int Up = new WorldVector2Int(0, 1);
        public static WorldVector2Int Down = new WorldVector2Int(0, -1);
        public static WorldVector2Int Left = new WorldVector2Int(-1, 0);
        public static WorldVector2Int Right = new WorldVector2Int(1, 0);
        public static WorldVector2Int UpLeft = new WorldVector2Int(-1,1);
        public static WorldVector2Int UpRight = new WorldVector2Int(1, 1);
        public static WorldVector2Int DownLeft = new WorldVector2Int(-1, -1);
        public static WorldVector2Int DownRight = new WorldVector2Int(1, -1);

        public int X;
        public int Y;

        public WorldVector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static WorldVector2Int operator +(WorldVector2Int a) => a;
        public static WorldVector2Int operator +(WorldVector2Int a, WorldVector2Int b) => new WorldVector2Int(a.X + b.X, a.Y + b.Y);

        public static WorldVector2Int operator -(WorldVector2Int a) => new WorldVector2Int(a.X * -1, a.Y * -1);
        public static WorldVector2Int operator -(WorldVector2Int a, WorldVector2Int b) => new WorldVector2Int(a.X - b.X, a.Y - b.Y);

        public static WorldVector2Int operator *(WorldVector2Int a, int b) => new WorldVector2Int(a.X * b, a.Y * b);
        public static WorldVector2Int operator /(WorldVector2Int a, int b) => new WorldVector2Int(a.X / b, a.Y / b);

        public static bool operator ==(WorldVector2Int a, WorldVector2Int b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(WorldVector2Int a, WorldVector2Int b)
        {
            return !(a == b);
        }


        public override bool Equals(object obj)
        {
            var other = (WorldVector2Int)obj;
            return other.X == X && other.Y == Y;
        }

        public bool Equals(WorldVector2Int other)
        {
            return X == other.X && Y == other.Y;
        }

        public WorldVector2Int NakedValues()
        {
            var returnValue = this;
            if (returnValue.X < 0)
            {
                returnValue.X *= -1;
            }

            if (returnValue.Y < 0)
            {
                returnValue.Y *= -1;
            }

            return returnValue;
        }

        public WorldVector2Int Direction(WorldVector2Int other, bool split = false)
        {
            var direction = other - this;
            if (split)
            {
                var naked = direction.NakedValues();
                if (naked.X > naked.Y)
                {
                    direction.X = direction.X > 0 ? 1 : -1;
                    direction.Y = 0;
                }
                else if (naked.Y > naked.X)
                {
                    direction.X = 0;
                    direction.Y = direction.Y > 0 ? 1 : -1;
                }
                else if (naked != Zero)
                {
                    if (direction.X > 0)
                    {
                        direction.X = 1;
                    }
                    else if (direction.X < 0)
                    {
                        direction.X = -1;
                    }

                    if (direction.Y < 0)
                    {
                        direction.Y = -1;
                    }
                    else if (direction.Y > 0)
                    {
                        direction.Y = 1;
                    }
                }
                else
                {
                    return Zero;
                }
            }
            else
            {
                if (direction.Y > 0)
                {
                    direction.Y = 1;
                }
                else if (direction.Y < 0)
                {
                    direction.Y = -1;
                }

                if (direction.X > 0)
                {
                    direction.X = 1;
                }
                else if (direction.X < 0)
                {
                    direction.X = -1;
                }
            }
            

            return direction;
        }

        public WorldVector2Int FaceDirection(WorldVector2Int other)
        {
            var direction = other - this;
            var nakedValues = direction.NakedValues();
            if (nakedValues.X > nakedValues.Y)
            {
                direction.X = direction.X > 0 ? 1 : -1;
                direction.Y = 0;
            }
            else if (nakedValues.Y > nakedValues.X)
            {
                direction.Y = direction.Y > 0 ? 1 : -1;
                direction.X = 0;
            }
            else
            {
                direction = Zero;
            }

            return direction;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public override string ToString()
        {
            return $"[X:{X}, Y:{Y}]";
        }

        public int SqrDistance(WorldVector2Int other)
        {
            return (other.X - X).Square() + (other.Y - Y).Square();
        }

        public int Distance(WorldVector2Int other)
        {
            return (int)System.Math.Sqrt(SqrDistance(other));
        }

        public WorldVector2Int[] GetDiagonals(WorldVector2Int pos)
        {
            var returnPositions = new List<WorldVector2Int>();
            returnPositions.Add(pos + UpLeft);
            returnPositions.Add(pos + UpRight);
            returnPositions.Add(pos + DownLeft);
            returnPositions.Add(pos + DownRight);

            return returnPositions.ToArray();
        }
    }
}