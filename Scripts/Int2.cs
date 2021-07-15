using System;

namespace DeltaTimer.FixedPoint
{
    [Serializable]
    public struct Int2 : IEquatable<Int2>
    {
        public int x;
        public int y;

        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Int2 @int && Equals(@int);
        }

        public bool Equals(Int2 other)
        {
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            int hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Int2 a, Int2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Int2 a, Int2 b)
        {
            return a.x != b.x || a.y != b.y;
        }
    }
}
