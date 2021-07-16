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

        internal const int RAW_VALUE_BITS = 64;
        internal const int FRACTIONAL_BITS = 32;
        internal const ulong INTEGER_MASK = 0xFFFFFFFF00000000;
        internal const long FRACTION_MASK = 0x00000000FFFFFFFF;

        private const string RAW_VALUE_FORMAT = "0x{0}";
        private const string RAW_VALUE_FORMAT_PROVIDER = "X16";

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
            return obj is FPFloat @float && @float.rawValue == rawValue;
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
                remainder %= divider;
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


    }
}
