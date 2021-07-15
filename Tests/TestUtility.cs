using NUnit.Framework;

namespace DeltaTimer.FixedPoint.Tests
{
    internal static class TestUtility
    {
        public static readonly float PRECISION = 0.00001f;

        public static void AreEqualFloat2(float expectedX, float expectedY, FPFloat2 fPFloat2)
        {
            if (expectedX != fPFloat2.x.ToValue() || expectedY != fPFloat2.y.ToValue())
            {
                string message = string.Format("Expected: ({0}, {1})\n But was: {2}", expectedX, expectedY, fPFloat2);
                Assert.Fail(message);
            }
        }

        public static void AssertApproximatelyEqual(float expected, float actual)
        {
            AssertApproximatelyEqual(expected, actual, PRECISION);
        }

        public static void AssertApproximatelyEqual(float expected, float actual, float precision)
        {
            if (!Approximately(expected, actual, precision))
            {
                string message = string.Format("Expected: {0}\n But was: {1}", expected, actual);
                Assert.Fail(message);
            }
        }

        public static void AssertApproximatelyEqual(float expectedX, float expectedY, FPFloat2 fPFloat2)
        {
            if (!Approximately(expectedX, fPFloat2.x.ToValue()) || !Approximately(expectedY, fPFloat2.y.ToValue()))
            {
                string message = string.Format("Expected: ({0}, {1})\n But was: {2}", expectedX, expectedY, fPFloat2);
                Assert.Fail(message);
            }
        }

        private static bool Approximately(float x, float y)
        {
            return Approximately(x, y, PRECISION);
        }

        private static bool Approximately(float x, float y, float precision)
        {
            float diff = x - y;
            if (diff < 0f)
            {
                diff = -diff;
            }

            return diff < precision;
        }
    }
}
