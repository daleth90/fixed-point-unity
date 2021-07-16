using System;

namespace DeltaTimer.FixedPoint
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns a random float
        /// </summary>
        private static FPFloat NextFloat(this Random random)
        {
            int integer = random.Next();
            int fraction = random.Next();
            long rawValue = ((long)integer << FPFloat.FRACTIONAL_BITS) + fraction;
            return FPFloat.CreateByRawValue(rawValue);
        }

        /// <summary>
        /// Returns a random float within [0, max)
        /// </summary>
        public static FPFloat NextFloat(this Random random, FPFloat max)
        {
            FPFloat randomValue = NextFloat(random);
            FPFloat remain = randomValue % max;
            return remain;
        }

        /// <summary>
        /// Returns a random float within [min, max)
        /// </summary>
        public static FPFloat NextFloat(this Random random, FPFloat min, FPFloat max)
        {
            FPFloat diff = max - min;
            return min + NextFloat(random, diff);
        }

        /// <summary>
        /// Returns a random float within [0, 1)
        /// </summary>
        public static FPFloat NextFloat01(this Random random)
        {
            long rawValue = random.Next();  // Implicit cast int to long
            return FPFloat.CreateByRawValue(rawValue);
        }
    }
}
