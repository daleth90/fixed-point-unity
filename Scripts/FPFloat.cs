using System;
using System.Runtime.CompilerServices;

namespace DeltaTimer.FixedPoint
{
    /// <summary>
    /// Represents a fixed-point number in Q31.32 format
    /// </summary>
    public partial struct FPFloat : IEquatable<FPFloat>, IComparable<FPFloat>
    {
        public static readonly FPFloat Precision = CreateByRawValue(0x0000_0000_0000_A7C5);  // Generate by "new FPFloat(0.00001f)" and print its rawValue
        public static readonly FPFloat Zero = new FPFloat();
        public static readonly FPFloat One = CreateByRawValue(1L << FRACTIONAL_BITS);
        public static readonly FPFloat OneOver2 = CreateByRawValue(0x0000000080000000);
        public static readonly FPFloat Two = new FPFloat(2);
        public static readonly FPFloat MinValue = CreateByRawValue(long.MinValue);
        public static readonly FPFloat MaxValue = CreateByRawValue(long.MaxValue);
        public static readonly FPFloat Pi = CreateByRawValue(PI);
        public static readonly FPFloat PiTimes2 = CreateByRawValue(PI_TIMES_2);
        public static readonly FPFloat PiOver2 = CreateByRawValue(PI_OVER_2);
        public static readonly FPFloat Deg2Rad = Pi / new FPFloat(180);
        public static readonly FPFloat Rad2Deg = new FPFloat(180) / Pi;

        private const int RAW_VALUE_BITS = 64;
        private const int FRACTIONAL_BITS = 32;
        private const ulong INTEGER_MASK = 0xFFFFFFFF00000000;
        private const long FRACTION_MASK = 0x00000000FFFFFFFF;

        private const long LN2 = 0xB17217F7;
        private const long LOG2MAX = 0x1F00000000;
        private const long LOG2MIN = -0x2000000000;
        private const long PI = 0x3243F6A88;
        private const long PI_TIMES_2 = 0x6487ED511;
        private const long PI_OVER_2 = 0x1921FB544;

        private const string RAW_VALUE_FORMAT = "0x{0}";
        private const string RAW_VALUE_FORMAT_PROVIDER = "X16";

        internal const int LUT_SIZE = (int)(PI_OVER_2 >> 15);

        private static readonly FPFloat FPLutInterval = new FPFloat(LUT_SIZE - 1) / PiOver2;

        internal long rawValue;

        public FPFloat(long value)
        {
            throw new InvalidOperationException();
        }

        public FPFloat(int value)
        {
            rawValue = (long)value << FRACTIONAL_BITS;
        }

        public FPFloat(float value)
        {
            rawValue = (long)(value * One.rawValue);
        }

        internal static FPFloat CreateByRawValue(long rawValue)
        {
            return new FPFloat { rawValue = rawValue };
        }

        public int ToInt()
        {
            return (int)(rawValue >> FRACTIONAL_BITS);
        }

        public float ToValue()
        {
            return (float)rawValue / One.rawValue;
        }

        public override bool Equals(object obj)
        {
            return obj is FPFloat && ((FPFloat)obj).rawValue == rawValue;
        }

        public override int GetHashCode()
        {
            return rawValue.GetHashCode();
        }

        public bool Equals(FPFloat other)
        {
            return rawValue == other.rawValue;
        }

        public int CompareTo(FPFloat other)
        {
            return rawValue.CompareTo(other.rawValue);
        }

        public override string ToString()
        {
            return ToValue().ToString();
        }

        public string RawValueToString()
        {
            return string.Format(RAW_VALUE_FORMAT, rawValue.ToString(RAW_VALUE_FORMAT_PROVIDER));
        }

        public static FPFloat operator +(FPFloat x, FPFloat y)
        {
            FPFloat fpFloat = CreateByRawValue(x.rawValue + y.rawValue);
            return fpFloat;
        }

        public static FPFloat operator -(FPFloat x, FPFloat y)
        {
            FPFloat fpFloat = CreateByRawValue(x.rawValue - y.rawValue);
            return fpFloat;
        }

        public static FPFloat operator *(FPFloat x, FPFloat y)
        {
            var xLow = (ulong)(x.rawValue & FRACTION_MASK);
            var xHigh = x.rawValue >> FRACTIONAL_BITS;
            var yLow = (ulong)(y.rawValue & FRACTION_MASK);
            var yHigh = y.rawValue >> FRACTIONAL_BITS;

            var mulLowLow = xLow * yLow;
            var mulLowHigh = (long)xLow * yHigh;
            var mulHighLow = xHigh * (long)yLow;
            var mulHighHigh = xHigh * yHigh;

            // The reason why using ulong, is to solve the bit setting issues caused by right shift logic.
            // Using ulong can promise the high-order empty bit positions are set to 0.
            // See: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-and-shift-operators#right-shift-operator-
            var lowResult = mulLowLow >> FRACTIONAL_BITS;
            var midResult1 = mulLowHigh;
            var midResult2 = mulHighLow;
            var highResult = mulHighHigh << FRACTIONAL_BITS;

            bool anyOverflow = false;
            (long sum, bool overflow) = AddOverflowHelper((long)lowResult, midResult1);
            anyOverflow |= overflow;
            (sum, overflow) = AddOverflowHelper(sum, midResult2);
            anyOverflow |= overflow;
            (sum, overflow) = AddOverflowHelper(sum, highResult);
            anyOverflow |= overflow;

            bool opSignsEqual = ((x.rawValue ^ y.rawValue) & MinValue.rawValue) == 0;

            // if signs of operands are equal and sign of result is negative,
            // then multiplication overflowed positively
            // the reverse is also true
            if (opSignsEqual)
            {
                if (sum < 0 || (anyOverflow && x.rawValue > 0))
                {
                    return MaxValue;
                }
            }
            else
            {
                if (sum > 0)
                {
                    return MinValue;
                }
            }

            // if the top 32 bits of hihi (unused in the result) are neither all 0s or 1s,
            // then this means the result overflowed.
            var topCarry = mulHighHigh >> FRACTIONAL_BITS;
            if (topCarry != 0 && topCarry != -1 /*&& xl != -17 && yl != -17*/)
            {
                return opSignsEqual ? MaxValue : MinValue;
            }

            // If signs differ, both operands' magnitudes are greater than 1,
            // and the result is greater than the negative operand, then there was negative overflow.
            if (!opSignsEqual)
            {
                long posOp, negOp;
                if (x.rawValue > y.rawValue)
                {
                    posOp = x.rawValue;
                    negOp = y.rawValue;
                }
                else
                {
                    posOp = y.rawValue;
                    negOp = x.rawValue;
                }
                if (sum > negOp && negOp < -One.rawValue && posOp > One.rawValue)
                {
                    return MinValue;
                }
            }

            return CreateByRawValue(sum);
        }

        private static (long sum, bool overflow) AddOverflowHelper(long x, long y)
        {
            long sum = x + y;
            // x + y overflows if sign(x) ^ sign(y) != sign(sum)
            bool overflow = ((x ^ y ^ sum) & MinValue.rawValue) != 0;
            return (sum, overflow);
        }

        /// <summary>
        /// Performs multiplication without checking for overflow.
        /// Useful for performance-critical code where the values are guaranteed not to cause overflow
        /// </summary>
        public static FPFloat FastMul(FPFloat x, FPFloat y)
        {
            var xLow = (ulong)(x.rawValue & FRACTION_MASK);
            var xHigh = x.rawValue >> FRACTIONAL_BITS;
            var yLow = (ulong)(y.rawValue & FRACTION_MASK);
            var yHigh = y.rawValue >> FRACTIONAL_BITS;

            var mulLowLow = xLow * yLow;
            var mulLowHigh = (long)xLow * yHigh;
            var mulHighLow = xHigh * (long)yLow;
            var mulHighHigh = xHigh * yHigh;

            // The reason why using ulong, is to solve the bit setting issues caused by right shift logic.
            // Using ulong can promise the high-order empty bit positions are set to 0.
            // See: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-and-shift-operators#right-shift-operator-
            var lowResult = mulLowLow >> FRACTIONAL_BITS;
            var midResult1 = mulLowHigh;
            var midResult2 = mulHighLow;
            var highResult = mulHighHigh << FRACTIONAL_BITS;

            var sum = (long)lowResult + midResult1 + midResult2 + highResult;
            return CreateByRawValue(sum);
        }

        public static FPFloat operator /(FPFloat x, FPFloat y)
        {
            long xl = x.rawValue;
            long yl = y.rawValue;

            if (yl == 0)
            {
                throw new DivideByZeroException();
            }

            ulong remainder = (ulong)(xl >= 0 ? xl : -xl);
            ulong divider = (ulong)(yl >= 0 ? yl : -yl);
            ulong quotient = 0UL;
            int bitPos = RAW_VALUE_BITS / 2 + 1;

            // If the divider is divisible by 2^n, take advantage of it.
            while ((divider & 0xF) == 0 && bitPos >= 4)
            {
                divider >>= 4;
                bitPos -= 4;
            }

            while (remainder != 0 && bitPos >= 0)
            {
                int shift = CountLeadingZeroes(remainder);
                if (shift > bitPos)
                {
                    shift = bitPos;
                }
                remainder <<= shift;
                bitPos -= shift;

                ulong div = remainder / divider;
                remainder = remainder % divider;
                quotient += div << bitPos;

                // Detect overflow
                if ((div & ~(0xFFFFFFFFFFFFFFFF >> bitPos)) != 0)
                {
                    return ((xl ^ yl) & MinValue.rawValue) == 0 ? MaxValue : MinValue;
                }

                remainder <<= 1;
                --bitPos;
            }

            // rounding
            quotient++;
            long result = (long)(quotient >> 1);
            if (((xl ^ yl) & MinValue.rawValue) != 0)
            {
                result = -result;
            }

            return CreateByRawValue(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CountLeadingZeroes(ulong x)
        {
            int result = 0;
            while ((x & 0xF000000000000000) == 0) { result += 4; x <<= 4; }
            while ((x & 0x8000000000000000) == 0) { result += 1; x <<= 1; }
            return result;
        }

        public static FPFloat operator %(FPFloat x, FPFloat y)
        {
            if (x.rawValue == MinValue.rawValue && y.rawValue == -1)
            {
                return Zero;
            }

            return CreateByRawValue(x.rawValue % y.rawValue);
        }

        public static FPFloat operator -(FPFloat x)
        {
            return x.rawValue == MinValue.rawValue ? MaxValue : CreateByRawValue(-x.rawValue);
        }

        public static bool operator ==(FPFloat x, FPFloat y)
        {
            return x.rawValue == y.rawValue;
        }

        public static bool operator !=(FPFloat x, FPFloat y)
        {
            return x.rawValue != y.rawValue;
        }

        public static bool operator >(FPFloat x, FPFloat y)
        {
            return x.rawValue > y.rawValue;
        }

        public static bool operator <(FPFloat x, FPFloat y)
        {
            return x.rawValue < y.rawValue;
        }

        public static bool operator >=(FPFloat x, FPFloat y)
        {
            return x.rawValue >= y.rawValue;
        }

        public static bool operator <=(FPFloat x, FPFloat y)
        {
            return x.rawValue <= y.rawValue;
        }

        public static implicit operator FPFloat(int x)
        {
            return new FPFloat(x);
        }

        public static explicit operator int(FPFloat x)
        {
            return (int)(x.rawValue >> FRACTIONAL_BITS);
        }

        /// <summary>
        /// Returns a random float
        /// </summary>
        private static FPFloat Random(Random random)
        {
            int integer = random.Next();
            int fraction = random.Next();
            long rawValue = ((long)integer << FRACTIONAL_BITS) + fraction;
            return CreateByRawValue(rawValue);
        }

        /// <summary>
        /// Returns a random float within [0, max)
        /// </summary>
        public static FPFloat Random(Random random, FPFloat max)
        {
            FPFloat randomValue = Random(random);
            FPFloat remain = randomValue % max;
            return remain;
        }

        /// <summary>
        /// Returns a random float within [min, max)
        /// </summary>
        public static FPFloat Random(Random random, FPFloat min, FPFloat max)
        {
            FPFloat diff = max - min;
            return min + Random(random, diff);
        }

        /// <summary>
        /// Returns a random float within [0, 1)
        /// </summary>
        public static FPFloat Random01(Random random)
        {
            long rawValue = random.Next();
            return CreateByRawValue(rawValue);
        }

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
            long mask = x.rawValue >> (FRACTIONAL_BITS - 1);
            long rawValue = (x.rawValue + mask) ^ mask;
            return CreateByRawValue(rawValue);
        }

        public static bool Approximately(FPFloat x, FPFloat y)
        {
            FPFloat diff = x - y;
            if (diff < Zero)
            {
                diff = -diff;
            }

            return diff < Precision;
        }

        public static bool ApproximatelyZero(FPFloat x)
        {
            if (x < Zero)
            {
                x = -x;
            }

            return x < Precision;
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

        public static FPFloat Clamp01(FPFloat value)
        {
            return Clamp(value, Zero, One);
        }

        public static FPFloat Lerp(FPFloat a, FPFloat b, FPFloat t)
        {
            t = Clamp01(t);

            if (t == Zero)
            {
                return a;
            }
            else if (t == One)
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
            ulong result = (ulong)x.rawValue & INTEGER_MASK;
            return CreateByRawValue((long)result);
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
            bool hasFractionalPart = (x.rawValue & FRACTION_MASK) != 0;
            return hasFractionalPart ? Floor(x) + One : x;
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

            long fractionalPart = x.rawValue & FRACTION_MASK;
            FPFloat integralPart = Floor(x);

            if (sign > 0)
            {
                if (fractionalPart >= 0x80000000)
                {
                    return integralPart + One;
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
                    return integralPart + One;
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
                return One;
            }

            // Avoid negative arguments by exploiting that exp(-x) = 1/exp(x).
            bool neg = x.rawValue < 0;
            if (neg)
            {
                x = -x;
            }

            if (x == One)
            {
                return neg ? One / new FPFloat(2) : new FPFloat(2);
            }

            if (x.rawValue >= LOG2MAX)
            {
                return neg ? One / MaxValue : MaxValue;
            }

            if (x.rawValue <= LOG2MIN)
            {
                return neg ? MaxValue : Zero;
            }

            /* The algorithm is based on the power series for exp(x):
             * http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
             * 
             * From term n, we get term n+1 by multiplying with x/n.
             * When the sum term drops to zero, we can stop summing.
             */

            int integerPart = FloorToInt(x);
            // Take fractional part of exponent
            x = CreateByRawValue(x.rawValue & FRACTION_MASK);

            FPFloat result = One;
            FPFloat term = One;
            int i = 1;
            while (term.rawValue != 0)
            {
                term = FastMul(FastMul(x, term), CreateByRawValue(LN2)) / new FPFloat(i);
                result += term;
                i++;
            }

            result = CreateByRawValue(result.rawValue << integerPart);
            if (neg)
            {
                result = One / result;
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

            long b = 1U << (FRACTIONAL_BITS - 1);
            long y = 0;

            long rawX = x.rawValue;
            while (rawX < One.rawValue)
            {
                rawX <<= 1;
                y -= One.rawValue;
            }

            while (rawX >= (One.rawValue << 1))
            {
                rawX >>= 1;
                y += One.rawValue;
            }

            FPFloat z = CreateByRawValue(rawX);
            for (int i = 0; i < FRACTIONAL_BITS; i++)
            {
                z = FastMul(z, z);
                if (z.rawValue >= (One.rawValue << 1))
                {
                    z = CreateByRawValue(z.rawValue >> 1);
                    y += b;
                }
                b >>= 1;
            }

            return CreateByRawValue(y);
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
            return FastMul(Log2(x), CreateByRawValue(LN2));
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
            if (x == One)
            {
                return One;
            }

            if (y.rawValue == 0)
            {
                return One;
            }

            if (x.rawValue == 0)
            {
                if (y.rawValue < 0)
                {
                    throw new DivideByZeroException();
                }

                return Zero;
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
                return Zero;
            }

            var num = (ulong)xl;
            var result = 0UL;

            // second-to-top bit
            var bit = 1UL << (RAW_VALUE_BITS - 2);

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
                        result = result >> 1;
                    }
                    bit >>= 2;
                }

                if (i == 0)
                {
                    // Then process it again to get the lowest 16 bits.
                    if (num > (1UL << (RAW_VALUE_BITS / 2)) - 1)
                    {
                        // The remainder 'num' is too large to be shifted left
                        // by 32, so we have to add 1 to result manually and
                        // adjust 'num' accordingly.
                        // num = a - (result + 0.5)^2
                        //       = num + result^2 - (result + 0.5)^2
                        //       = num - result - 0.5
                        num -= result;
                        num = (num << (RAW_VALUE_BITS / 2)) - 0x80000000UL;
                        result = (result << (RAW_VALUE_BITS / 2)) + 0x80000000UL;
                    }
                    else
                    {
                        num <<= (RAW_VALUE_BITS / 2);
                        result <<= (RAW_VALUE_BITS / 2);
                    }

                    bit = 1UL << (RAW_VALUE_BITS / 2 - 2);
                }
            }
            // Finally, if next bit would have been 1, round the result upwards.
            if (num > result)
            {
                ++result;
            }
            return CreateByRawValue((long)result);
        }

        /// <summary>
        /// Returns the sine of angle x in radians.
        /// </summary>
        public static FPFloat Sin(FPFloat x)
        {
            long clampedL = ClampSinValue(x.rawValue, out bool flipHorizontal, out bool flipVertical);
            FPFloat clamped = CreateByRawValue(clampedL);

            // Find the two closest values in the LUT and perform linear interpolation
            // This is what kills the performance of this function on x86 - x64 is fine though
            FPFloat rawIndex = FastMul(clamped, FPLutInterval);
            FPFloat roundedIndex = Round(rawIndex);
            FPFloat indexError = rawIndex - roundedIndex;
            int roundIndexInt = roundedIndex.ToInt();

            long nearestValueL = SinLut[flipHorizontal ? SinLut.Length - 1 - roundIndexInt : roundIndexInt];
            FPFloat nearestValue = CreateByRawValue(nearestValueL);

            long secondNearestValueL = SinLut[flipHorizontal ?
                SinLut.Length - 1 - roundIndexInt - Sign(indexError) :
                roundIndexInt + Sign(indexError)];
            FPFloat secondNearestValue = CreateByRawValue(secondNearestValueL);

            long delta = FastMul(indexError, Abs(nearestValue - secondNearestValue)).rawValue;
            var interpolatedValue = nearestValue.rawValue + (flipHorizontal ? -delta : delta);
            var finalValue = flipVertical ? -interpolatedValue : interpolatedValue;
            return CreateByRawValue(finalValue);
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
            return CreateByRawValue(result);
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
            return Sin(CreateByRawValue(rawAngle));
        }

        /// <summary>
        /// Returns a rough approximation of the cosine of angle x in radians.
        /// </summary>
        public static FPFloat FastCos(FPFloat x)
        {
            var xl = x.rawValue;
            var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            return FastSin(CreateByRawValue(rawAngle));
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

            FPFloat clamped = CreateByRawValue(clampedPi);

            // Find the two closest values in the LUT and perform linear interpolation
            FPFloat rawIndex = FastMul(clamped, FPLutInterval);
            FPFloat roundedIndex = Round(rawIndex);
            FPFloat indexError = rawIndex - roundedIndex;
            int roundIndexInt = roundedIndex.ToInt();

            long nearestValueL = TanLut[roundIndexInt];
            FPFloat nearestValue = CreateByRawValue(nearestValueL);

            long secondNearestValueL = TanLut[roundIndexInt + Sign(indexError)];
            FPFloat secondNearestValue = CreateByRawValue(secondNearestValueL);

            long delta = FastMul(indexError, Abs(nearestValue - secondNearestValue)).rawValue;
            long interpolatedValue = nearestValue.rawValue + delta;
            long finalValue = flip ? -interpolatedValue : interpolatedValue;
            return CreateByRawValue(finalValue);
        }

        /// <summary>
        /// Returns the arc-sine of x - the angle in radians which sine is x.
        /// Provides at least 7 decimals of accuracy.
        /// </summary>
        /// <param name="x">Within [-1, 1]</param>
        /// <returns>In radians within [-π/2, π/2]</returns>
        public static FPFloat Asin(FPFloat x)
        {
            if (x < -One || x > One)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (x.rawValue == 0)
            {
                return Zero;
            }
            else if (x == One)
            {
                return PiOver2;
            }
            else if (x == -One)
            {
                return -PiOver2;
            }

            FPFloat result = Atan(x / Sqrt(One - x * x));
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
            if (x < -One || x > One)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (x.rawValue == 0)
            {
                return PiOver2;
            }

            FPFloat result = Atan(Sqrt(One - x * x) / x);
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
                return Zero;
            }

            // Force positive values for argument
            // Atan(-z) = -Atan(z).
            bool neg = x.rawValue < 0;
            if (neg)
            {
                x = -x;
            }

            bool invert = x > One;
            if (invert)
            {
                x = One / x;
            }

            FPFloat result = One;
            FPFloat term = One;

            FPFloat xSq = x * x;
            FPFloat xSq2 = xSq * Two;
            FPFloat xSqPlusOne = xSq + One;
            FPFloat xSq12 = xSqPlusOne * Two;
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
                    return Zero;
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
