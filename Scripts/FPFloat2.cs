using System;

namespace DeltaTimer.FixedPoint
{
    public struct FPFloat2 : IEquatable<FPFloat2>
    {
        public static readonly FPFloat2 Zero = new FPFloat2();
        public static readonly FPFloat2 Right = new FPFloat2(FPFloat.One, FPFloat.Zero);
        public static readonly FPFloat2 Left = new FPFloat2(new FPFloat(-1), FPFloat.Zero);
        public static readonly FPFloat2 Up = new FPFloat2(FPFloat.Zero, FPFloat.One);
        public static readonly FPFloat2 Down = new FPFloat2(FPFloat.Zero, new FPFloat(-1));

        private static readonly string FORMAT = "({0}, {1})";

        public FPFloat x;
        public FPFloat y;

        public FPFloat2 Normalized
        {
            get
            {
                FPFloat magnitude = Magnitude;
                if (magnitude == FPFloat.Zero)
                {
                    return Zero;
                }

                // TODO: Could be optimized by Fast inverse square root: https://en.wikipedia.org/wiki/Fast_inverse_square_root
                return new FPFloat2(x / magnitude, y / magnitude);
            }
        }

        public FPFloat SqrMagnitude { get { return x * x + y * y; } }

        public FPFloat Magnitude { get { return MathFP.Sqrt(x * x + y * y); } }

        public FPFloat2(FPFloat x, FPFloat y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is FPFloat2 other && Equals(other);
        }

        public bool Equals(FPFloat2 other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }

        public override int GetHashCode()
        {
            int hashCode = -1851806893;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return string.Format(FORMAT, x, y);
        }

        public static FPFloat2 operator +(FPFloat2 a, FPFloat2 b)
        {
            return new FPFloat2(a.x + b.x, a.y + b.y);
        }

        public static FPFloat2 operator -(FPFloat2 a, FPFloat2 b)
        {
            return new FPFloat2(a.x - b.x, a.y - b.y);
        }

        public static FPFloat2 operator *(FPFloat2 a, FPFloat d)
        {
            return new FPFloat2(a.x * d, a.y * d);
        }

        public static FPFloat2 operator *(FPFloat d, FPFloat2 a)
        {
            return new FPFloat2(a.x * d, a.y * d);
        }

        public static FPFloat2 operator /(FPFloat2 a, FPFloat d)
        {
            return new FPFloat2(a.x / d, a.y / d);
        }

        public static FPFloat2 operator -(FPFloat2 a)
        {
            return new FPFloat2(-a.x, -a.y);
        }

        public static bool operator ==(FPFloat2 a, FPFloat2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(FPFloat2 a, FPFloat2 b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static explicit operator FPFloat2(Int2 value)
        {
            return new FPFloat2(new FPFloat(value.x), new FPFloat(value.y));
        }

        /// <summary>
        /// Returns the distance between a and b.
        /// </summary>
        public static FPFloat Distance(FPFloat2 a, FPFloat2 b)
        {
            FPFloat2 diff = a - b;
            if (diff == Zero)
            {
                return FPFloat.Zero;
            }
            return (a - b).Magnitude;
        }

        /// <summary>
        /// Dot Product of two vectors.
        /// </summary>
        public static FPFloat Dot(FPFloat2 a, FPFloat2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        /// <summary>
        /// Returns the unsigned angle in degrees between a and b.
        /// The smaller of the two possible angles between the two vectors is used.
        /// </summary>
        public static FPFloat Angle(FPFloat2 a, FPFloat2 b)
        {
            FPFloat sin = a.x * b.y - b.x * a.y;
            FPFloat cos = a.x * b.x + a.y * b.y;
            FPFloat rad = MathFP.Atan2(sin, cos);
            if (rad < FPFloat.Zero)
            {
                rad = -rad;
            }
            return rad * MathFP.Rad2Deg;
        }

        /// <summary>
        /// Rotate a vector by an angle in degrees
        /// </summary>
        public FPFloat2 Rotate(FPFloat degrees)
        {
            FPFloat sin = MathFP.Sin(degrees * MathFP.Deg2Rad);
            FPFloat cos = MathFP.Cos(degrees * MathFP.Deg2Rad);

            FPFloat2 vector;
            vector.x = (cos * x) - (sin * y);
            vector.y = (sin * x) + (cos * y);

            return vector;
        }

        /// <summary>
        /// Reflects a vector off a normal. (The normal MUST be normalized)
        /// </summary>
        public static FPFloat2 Reflect(FPFloat2 direction, FPFloat2 normal)
        {
            FPFloat dot = Dot(direction, normal);
            return direction - normal * dot * FPFloat.Two;
        }
    }
}
