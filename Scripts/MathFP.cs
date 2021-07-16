using System;

namespace DeltaTimer.FixedPoint
{
    public static partial class MathFP
    {
        public static readonly FPFloat Pi = FPFloat.CreateByRawValue(PI);
        public static readonly FPFloat PiTimes2 = FPFloat.CreateByRawValue(PI_TIMES_2);
        public static readonly FPFloat PiOver2 = FPFloat.CreateByRawValue(PI_OVER_2);
        public static readonly FPFloat Deg2Rad = Pi / new FPFloat(180);
        public static readonly FPFloat Rad2Deg = new FPFloat(180) / Pi;

        private const long LN2 = 0xB17217F7;
        private const long LOG2MAX = 0x1F00000000;
        private const long LOG2MIN = -0x2000000000;

        private const long PI = 0x3243F6A88;
        private const long PI_TIMES_2 = 0x6487ED511;
        private const long PI_OVER_2 = 0x1921FB544;

        internal const int LUT_SIZE = (int)(PI_OVER_2 >> 15);

        private static readonly FPFloat FPLutInterval = new FPFloat(LUT_SIZE - 1) / PiOver2;

        /// <summary>
        /// Returns an integer that indicates the sign of x.
        /// 1 means positive, -1 means negative, 0 means zero
        /// </summary>
        public static int Sign(FPFloat x)
        {
            if (x.rawValue == 0L)
            {
                return 0;
            }
            else if (x.rawValue > 0L)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns the absolute value of x.
        /// </summary>
        public static FPFloat Abs(FPFloat x)
        {
            long mask = x.rawValue >> (FPFloat.FRACTIONAL_BITS - 1);
            long rawValue = (x.rawValue + mask) ^ mask;
            return FPFloat.CreateByRawValue(rawValue);
        }

        public static bool Approximately(FPFloat x, FPFloat y)
        {
            FPFloat diff = x - y;
            if (diff < FPFloat.Zero)
            {
                diff = -diff;
            }

            return diff < FPFloat.Precision;
        }

        public static bool ApproximatelyZero(FPFloat x)
        {
            if (x < FPFloat.Zero)
            {
                x = -x;
            }

            return x < FPFloat.Precision;
        }

        public static FPFloat Min(FPFloat x, FPFloat y)
        {
            if (x < y)
            {
                return x;
            }
            else
            {
                return y;
            }
        }

        public static int Min(int x, int y)
        {
            if (x < y)
            {
                return x;
            }
            else
            {
                return y;
            }
        }

        public static FPFloat Max(FPFloat x, FPFloat y)
        {
            if (x > y)
            {
                return x;
            }
            else
            {
                return y;
            }
        }

        public static int Max(int x, int y)
        {
            if (x > y)
            {
                return x;
            }
            else
            {
                return y;
            }
        }

        public static FPFloat Clamp(FPFloat value, FPFloat min, FPFloat max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        public static FPFloat Clamp01(FPFloat value)
        {
            return Clamp(value, FPFloat.Zero, FPFloat.One);
        }

        public static FPFloat Lerp(FPFloat a, FPFloat b, FPFloat t)
        {
            t = Clamp01(t);

            if (t == FPFloat.Zero)
            {
                return a;
            }
            else if (t == FPFloat.One)
            {
                return b;
            }
            else
            {
                return a + (b - a) * t;
            }
        }

        /// <summary>
        /// Returns the largest integer smaller than or equal to x.
        /// </summary>
        public static FPFloat Floor(FPFloat x)
        {
            // Just zero out the fractional part
            ulong result = (ulong)x.rawValue & FPFloat.INTEGER_MASK;
            return FPFloat.CreateByRawValue((long)result);
        }

        /// <summary>
        /// Returns the largest integer smaller than or equal to x.
        /// </summary>
        public static int FloorToInt(FPFloat x)
        {
            FPFloat floor = Floor(x);
            return floor.ToInt();
        }

        /// <summary>
        /// Returns the smallest integer greater than or equal to x.
        /// </summary>
        public static FPFloat Ceiling(FPFloat x)
        {
            bool hasFractionalPart = (x.rawValue & FPFloat.FRACTION_MASK) != 0;
            return hasFractionalPart ? Floor(x) + FPFloat.One : x;
        }

        /// <summary>
        /// Returns the smallest integer greater than or equal to x.
        /// </summary>
        public static int CeilingToInt(FPFloat x)
        {
            FPFloat ceiling = Ceiling(x);
            return ceiling.ToInt();
        }

        /// <summary>
        /// Returns x rounded to the nearest integer.
        /// </summary>
        public static FPFloat Round(FPFloat x)
        {
            int sign = Sign(x);
            if (sign == 0)
            {
                return x;
            }

            long fractionalPart = x.rawValue & FPFloat.FRACTION_MASK;
            FPFloat integralPart = Floor(x);

            if (sign > 0)
            {
                if (fractionalPart >= 0x80000000)
                {
                    return integralPart + FPFloat.One;
                }
                else
                {
                    return integralPart;
                }
            }
            else
            {
                if (fractionalPart <= 0x80000000)
                {
                    return integralPart;
                }
                else
                {
                    return integralPart + FPFloat.One;
                }
            }
        }

        public static int RoundToInt(FPFloat x)
        {
            FPFloat round = Round(x);
            return round.ToInt();
        }

        /// <summary>
        /// Returns 2 raised to the specified power.
        /// Provides at least 6 decimals of accuracy.
        /// </summary>
        internal static FPFloat Pow2(FPFloat x)
        {
            if (x.rawValue == 0)
            {
                return FPFloat.One;
            }

            // Avoid negative arguments by exploiting that exp(-x) = 1/exp(x).
            bool neg = x.rawValue < 0;
            if (neg)
            {
                x = -x;
            }

            if (x == FPFloat.One)
            {
                return neg ? FPFloat.OneOver2 : new FPFloat(2);
            }

            if (x.rawValue >= LOG2MAX)
            {
                return neg ? FPFloat.One / FPFloat.MaxValue : FPFloat.MaxValue;
            }

            if (x.rawValue <= LOG2MIN)
            {
                return neg ? FPFloat.MaxValue : FPFloat.Zero;
            }

            /* The algorithm is based on the power series for exp(x):
             * http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
             * 
             * From term n, we get term n+1 by multiplying with x/n.
             * When the sum term drops to zero, we can stop summing.
             */

            int integerPart = FloorToInt(x);
            // Take fractional part of exponent
            x = FPFloat.CreateByRawValue(x.rawValue & FPFloat.FRACTION_MASK);

            FPFloat result = FPFloat.One;
            FPFloat term = FPFloat.One;
            int i = 1;
            while (term.rawValue != 0)
            {
                term = FPFloat.FastMul(FPFloat.FastMul(x, term), FPFloat.CreateByRawValue(LN2)) / new FPFloat(i);
                result += term;
                i++;
            }

            result = FPFloat.CreateByRawValue(result.rawValue << integerPart);
            if (neg)
            {
                result = FPFloat.One / result;
            }

            return result;
        }

        /// <summary>
        /// Returns the base-2 logarithm of a specified number.
        /// Provides at least 9 decimals of accuracy.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was non-positive
        /// </exception>
        internal static FPFloat Log2(FPFloat x)
        {
            if (x.rawValue <= 0)
            {
                throw new ArgumentOutOfRangeException("Non-positive value passed to Ln", "x");
            }

            // This implementation is based on Clay. S. Turner's fast binary logarithm
            // algorithm (C. S. Turner,  "A Fast Binary Logarithm Algorithm", IEEE Signal
            //     Processing Mag., pp. 124,140, Sep. 2010.)

            long b = 1U << (FPFloat.FRACTIONAL_BITS - 1);
            long y = 0;

            long rawX = x.rawValue;
            while (rawX < FPFloat.One.rawValue)
            {
                rawX <<= 1;
                y -= FPFloat.One.rawValue;
            }

            while (rawX >= (FPFloat.One.rawValue << 1))
            {
                rawX >>= 1;
                y += FPFloat.One.rawValue;
            }

            FPFloat z = FPFloat.CreateByRawValue(rawX);
            for (int i = 0; i < FPFloat.FRACTIONAL_BITS; i++)
            {
                z = FPFloat.FastMul(z, z);
                if (z.rawValue >= (FPFloat.One.rawValue << 1))
                {
                    z = FPFloat.CreateByRawValue(z.rawValue >> 1);
                    y += b;
                }
                b >>= 1;
            }

            return FPFloat.CreateByRawValue(y);
        }

        /// <summary>
        /// Returns the natural logarithm of a specified number.
        /// Provides at least 7 decimals of accuracy.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was non-positive
        /// </exception>
        public static FPFloat Ln(FPFloat x)
        {
            return FPFloat.FastMul(Log2(x), FPFloat.CreateByRawValue(LN2));
        }

        /// <summary>
        /// Returns a specified number raised to the specified power.
        /// Provides about 5 digits of accuracy for the result.
        /// </summary>
        /// <exception cref="DivideByZeroException">
        /// The base was zero, with a negative exponent
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The base was negative, with a non-zero exponent
        /// </exception>
        public static FPFloat Pow(FPFloat x, FPFloat y)
        {
            if (x == FPFloat.One)
            {
                return FPFloat.One;
            }

            if (y.rawValue == 0)
            {
                return FPFloat.One;
            }

            if (x.rawValue == 0)
            {
                if (y.rawValue < 0)
                {
                    throw new DivideByZeroException();
                }

                return FPFloat.Zero;
            }

            FPFloat log2 = Log2(x);
            return Pow2(y * log2);
        }

        /// <summary>
        /// Returns the square root of a specified number.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was negative.
        /// </exception>
        public static FPFloat Sqrt(FPFloat x)
        {
            var xl = x.rawValue;
            if (xl < 0)
            {
                // We cannot represent infinities like Single and Double, and Sqrt is
                // mathematically undefined for x < 0. So we just throw an exception.
                throw new ArgumentOutOfRangeException("Negative value passed to Sqrt", "x");
            }
            else if (x.rawValue == 0L)
            {
                return FPFloat.Zero;
            }

            var num = (ulong)xl;
            var result = 0UL;

            // second-to-top bit
            var bit = 1UL << (FPFloat.RAW_VALUE_BITS - 2);

            while (bit > num)
            {
                bit >>= 2;
            }

            // The main part is executed twice, in order to avoid
            // using 128 bit values in computations.
            for (var i = 0; i < 2; ++i)
            {
                // First we get the top 48 bits of the answer.
                while (bit != 0)
                {
                    if (num >= result + bit)
                    {
                        num -= result + bit;
                        result = (result >> 1) + bit;
                    }
                    else
                    {
                        result >>= 1;
                    }
                    bit >>= 2;
                }

                if (i == 0)
                {
                    // Then process it again to get the lowest 16 bits.
                    if (num > (1UL << (FPFloat.RAW_VALUE_BITS / 2)) - 1)
                    {
                        // The remainder 'num' is too large to be shifted left
                        // by 32, so we have to add 1 to result manually and
                        // adjust 'num' accordingly.
                        // num = a - (result + 0.5)^2
                        //       = num + result^2 - (result + 0.5)^2
                        //       = num - result - 0.5
                        num -= result;
                        num = (num << (FPFloat.RAW_VALUE_BITS / 2)) - 0x80000000UL;
                        result = (result << (FPFloat.RAW_VALUE_BITS / 2)) + 0x80000000UL;
                    }
                    else
                    {
                        num <<= (FPFloat.RAW_VALUE_BITS / 2);
                        result <<= (FPFloat.RAW_VALUE_BITS / 2);
                    }

                    bit = 1UL << (FPFloat.RAW_VALUE_BITS / 2 - 2);
                }
            }
            // Finally, if next bit would have been 1, round the result upwards.
            if (num > result)
            {
                ++result;
            }
            return FPFloat.CreateByRawValue((long)result);
        }

        /// <summary>
        /// Returns the sine of angle x in radians.
        /// </summary>
        public static FPFloat Sin(FPFloat x)
        {
            long clampedL = ClampSinValue(x.rawValue, out bool flipHorizontal, out bool flipVertical);
            FPFloat clamped = FPFloat.CreateByRawValue(clampedL);

            // Find the two closest values in the LUT and perform linear interpolation
            // This is what kills the performance of this function on x86 - x64 is fine though
            FPFloat rawIndex = FPFloat.FastMul(clamped, FPLutInterval);
            FPFloat roundedIndex = Round(rawIndex);
            FPFloat indexError = rawIndex - roundedIndex;
            int roundIndexInt = roundedIndex.ToInt();

            long nearestValueL = SinLut[flipHorizontal ? SinLut.Length - 1 - roundIndexInt : roundIndexInt];
            FPFloat nearestValue = FPFloat.CreateByRawValue(nearestValueL);

            long secondNearestValueL = SinLut[flipHorizontal ?
                SinLut.Length - 1 - roundIndexInt - Sign(indexError) :
                roundIndexInt + Sign(indexError)];
            FPFloat secondNearestValue = FPFloat.CreateByRawValue(secondNearestValueL);

            long delta = FPFloat.FastMul(indexError, Abs(nearestValue - secondNearestValue)).rawValue;
            var interpolatedValue = nearestValue.rawValue + (flipHorizontal ? -delta : delta);
            var finalValue = flipVertical ? -interpolatedValue : interpolatedValue;
            return FPFloat.CreateByRawValue(finalValue);
        }

        /// <summary>
        /// Returns a rough approximation of the sine of angle x in radians.
        /// This is at least 3 times faster than Sin() on x86 and slightly faster than Math.Sin(),
        /// however its accuracy is limited to 4-5 decimals, for small enough values of x.
        /// </summary>
        public static FPFloat FastSin(FPFloat x)
        {
            var clampedL = ClampSinValue(x.rawValue, out bool flipHorizontal, out bool flipVertical);

            // Here we use the fact that the SinLut table has a number of entries
            // equal to (PI_OVER_2 >> 15) to use the angle to index directly into it
            uint rawIndex = (uint)(clampedL >> 15);
            if (rawIndex >= LUT_SIZE)
            {
                rawIndex = LUT_SIZE - 1;
            }

            long nearestValue = SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)rawIndex :
                (int)rawIndex];
            long result = flipVertical ? -nearestValue : nearestValue;
            return FPFloat.CreateByRawValue(result);
        }

        private static long ClampSinValue(long angle, out bool flipHorizontal, out bool flipVertical)
        {
            // Obtained from ((Fix64)1686629713.065252369824872831112M).m_rawValue
            // This is (2^29)*PI, where 29 is the largest N such that (2^N)*PI < MaxValue.
            // The idea is that this number contains way more precision than PI_TIMES_2,
            // and (((x % (2^29*PI)) % (2^28*PI)) % ... (2^1*PI) = x % (2 * PI)
            // In practice this gives us an error of about 1,25e-9 in the worst case scenario (Sin(MaxValue))
            // Whereas simply doing x % PI_TIMES_2 is the 2e-3 range.
            var largePI = 7244019458077122842;

            var clamped2Pi = angle;
            for (int i = 0; i < 29; ++i)
            {
                clamped2Pi %= (largePI >> i);
            }
            if (angle < 0)
            {
                clamped2Pi += PI_TIMES_2;
            }

            // The LUT contains values for 0 - PiOver2; every other value must be obtained by
            // vertical or horizontal mirroring
            flipVertical = clamped2Pi >= PI;
            // obtain (angle % PI) from (angle % 2PI) - much faster than doing another modulo
            var clampedPi = clamped2Pi;
            while (clampedPi >= PI)
            {
                clampedPi -= PI;
            }
            flipHorizontal = clampedPi >= PI_OVER_2;
            // obtain (angle % PI_OVER_2) from (angle % PI) - much faster than doing another modulo
            var clampedPiOver2 = clampedPi;
            if (clampedPiOver2 >= PI_OVER_2)
            {
                clampedPiOver2 -= PI_OVER_2;
            }
            return clampedPiOver2;
        }

        /// <summary>
        /// Returns the cosine of angle x in radians.
        /// </summary>
        public static FPFloat Cos(FPFloat x)
        {
            long xl = x.rawValue;
            long rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            return Sin(FPFloat.CreateByRawValue(rawAngle));
        }

        /// <summary>
        /// Returns a rough approximation of the cosine of angle x in radians.
        /// </summary>
        public static FPFloat FastCos(FPFloat x)
        {
            var xl = x.rawValue;
            var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            return FastSin(FPFloat.CreateByRawValue(rawAngle));
        }

        /// <summary>
        /// Returns the tangent of angle x in radians.
        /// </summary>
        public static FPFloat Tan(FPFloat x)
        {
            long clampedPi = x.rawValue % PI;
            bool flip = false;
            if (clampedPi < 0)
            {
                clampedPi = -clampedPi;
                flip = true;
            }
            if (clampedPi > PI_OVER_2)
            {
                flip = !flip;
                clampedPi = PI_OVER_2 - (clampedPi - PI_OVER_2);
            }

            FPFloat clamped = FPFloat.CreateByRawValue(clampedPi);

            // Find the two closest values in the LUT and perform linear interpolation
            FPFloat rawIndex = FPFloat.FastMul(clamped, FPLutInterval);
            FPFloat roundedIndex = Round(rawIndex);
            FPFloat indexError = rawIndex - roundedIndex;
            int roundIndexInt = roundedIndex.ToInt();

            long nearestValueL = TanLut[roundIndexInt];
            FPFloat nearestValue = FPFloat.CreateByRawValue(nearestValueL);

            long secondNearestValueL = TanLut[roundIndexInt + Sign(indexError)];
            FPFloat secondNearestValue = FPFloat.CreateByRawValue(secondNearestValueL);

            long delta = FPFloat.FastMul(indexError, Abs(nearestValue - secondNearestValue)).rawValue;
            long interpolatedValue = nearestValue.rawValue + delta;
            long finalValue = flip ? -interpolatedValue : interpolatedValue;
            return FPFloat.CreateByRawValue(finalValue);
        }

        /// <summary>
        /// Returns the arc-sine of x - the angle in radians which sine is x.
        /// Provides at least 7 decimals of accuracy.
        /// </summary>
        /// <param name="x">Within [-1, 1]</param>
        /// <returns>In radians within [-π/2, π/2]</returns>
        public static FPFloat Asin(FPFloat x)
        {
            if (x < -FPFloat.One || x > FPFloat.One)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (x.rawValue == 0)
            {
                return FPFloat.Zero;
            }
            else if (x == FPFloat.One)
            {
                return PiOver2;
            }
            else if (x == -FPFloat.One)
            {
                return -PiOver2;
            }

            FPFloat result = Atan(x / Sqrt(FPFloat.One - x * x));
            return result;
        }

        /// <summary>
        /// Returns the arc-cosine of x - the angle in radians which cosine is x.
        /// Provides at least 7 decimals of accuracy.
        /// </summary>
        /// <param name="x">Within [-1, 1]</param>
        /// <returns>In radians within [0, π]</returns>
        public static FPFloat Acos(FPFloat x)
        {
            if (x < -FPFloat.One || x > FPFloat.One)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (x.rawValue == 0)
            {
                return PiOver2;
            }

            FPFloat result = Atan(Sqrt(FPFloat.One - x * x) / x);
            return x.rawValue < 0 ? result + Pi : result;
        }

        /// <summary>
        /// Returns the arc-tangent of x - the angle in radians which tangent is x.
        /// Provides at least 7 decimals of accuracy.
        /// </summary>
        /// <returns>In radians within [-π/2, π/2]</returns>
        public static FPFloat Atan(FPFloat x)
        {
            if (x.rawValue == 0)
            {
                return FPFloat.Zero;
            }

            // Force positive values for argument
            // Atan(-z) = -Atan(z).
            bool neg = x.rawValue < 0;
            if (neg)
            {
                x = -x;
            }

            bool invert = x > FPFloat.One;
            if (invert)
            {
                x = FPFloat.One / x;
            }

            FPFloat result = FPFloat.One;
            FPFloat term = FPFloat.One;

            FPFloat xSq = x * x;
            FPFloat xSq2 = xSq * FPFloat.Two;
            FPFloat xSqPlusOne = xSq + FPFloat.One;
            FPFloat xSq12 = xSqPlusOne * FPFloat.Two;
            FPFloat dividend = xSq2;
            FPFloat divisor = xSqPlusOne * 3;

            for (int i = 2; i < 30; ++i)
            {
                term *= dividend / divisor;
                result += term;

                dividend += xSq2;
                divisor += xSq12;

                if (term.rawValue == 0)
                {
                    break;
                }
            }

            result = result * x / xSqPlusOne;

            if (invert)
            {
                result = PiOver2 - result;
            }

            if (neg)
            {
                result = -result;
            }

            return result;
        }

        /// <summary>
        /// Returns the angle in radians which Tan is y/x.
        /// </summary>
        /// <returns>In radians within (-π, π]</returns>
        public static FPFloat Atan2(FPFloat y, FPFloat x)
        {
            if (x.rawValue == 0)
            {
                if (y.rawValue > 0)
                {
                    return PiOver2;
                }
                else if (y.rawValue == 0)
                {
                    return FPFloat.Zero;
                }
                else  // y.rawValue < 0
                {
                    return -PiOver2;
                }
            }

            FPFloat tan = y / x;
            if (x.rawValue > 0)
            {
                return Atan(tan);
            }
            else if (y.rawValue >= 0)
            {
                return Atan(tan) + Pi;
            }
            else  // y.rawValue < 0
            {
                return Atan(tan) - Pi;
            }
        }
    }
}
