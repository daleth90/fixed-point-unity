using NUnit.Framework;

namespace DeltaTimer.FixedPoint.Tests
{
    public class FPFloatTests
    {
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(20)]
        [TestCase(10000)]
        [TestCase(-1)]
        [TestCase(-999)]
        public void IntConstructorTests(int originalInt)
        {
            FPFloat fpFloat = new FPFloat(originalInt);
            float actual = fpFloat.ToValue();
            Assert.AreEqual((float)originalInt, actual);
        }

        [TestCase(1f)]
        [TestCase(0.5f)]
        [TestCase(0.0013f)]
        [TestCase(66.66f)]
        [TestCase(-99f)]
        [TestCase(-1234.56789f)]
        public void FloatConstructorTests(float originalFloat)
        {
            FPFloat fpFloat = new FPFloat(originalFloat);
            float actual = fpFloat.ToValue();
            TestUtility.AssertApproximatelyEqual(originalFloat, actual);
        }

        #region Constant Tests
        [Test]
        public void PrecisionTest()
        {
            TestUtility.AssertApproximatelyEqual(0.00001f, FPFloat.Precision.ToValue());
        }

        [Test]
        public void ZeroTest()
        {
            Assert.AreEqual(0f, FPFloat.Zero.ToValue());
        }

        [Test]
        public void OneTest()
        {
            Assert.AreEqual(1f, FPFloat.One.ToValue());
        }

        [Test]
        public void OneOver2Test()
        {
            Assert.AreEqual(0.5f, FPFloat.OneOver2.ToValue());
        }
        #endregion

        [TestCase(-0.56789f, -0.56789f, true)]
        [TestCase(-0.56789f, -0.56788f, false)]
        public void EqualsTests(float x, float y, bool expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            Assert.AreEqual(expected, fpX.Equals(fpY));
        }

        [TestCase(-0.56789f, -0.56789f, true)]
        [TestCase(-0.56789f, -0.56788f, false)]
        public void GetHashCodeTests(float x, float y, bool expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            bool result = fpX.GetHashCode() == fpY.GetHashCode();
            Assert.AreEqual(expected, result);
        }

        [TestCase(1.5f, 1.5f, 0)]
        [TestCase(2f, -2f, 1)]
        [TestCase(-2f, -1f, -1)]
        public void CompareToTests(float x, float y, int expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            int result = fpX.CompareTo(fpY);
            Assert.AreEqual(expected, result);
        }

        // Test the format, not the correction of the value
        [TestCase(0.5f, "0.5")]
        [TestCase(1, "1")]
        public void ToStringTests(float x, string expected)
        {
            FPFloat fpX = new FPFloat(x);
            Assert.AreEqual(expected, fpX.ToString());
        }

        [TestCase(0.5f, "0x0000000080000000")]
        [TestCase(1, "0x0000000100000000")]
        public void RawValueToStringTests(float x, string expected)
        {
            FPFloat fpX = new FPFloat(x);
            Assert.AreEqual(expected, fpX.RawValueToString());
        }

        #region Operator Tests
        [TestCase(1f, 1f, 2f)]
        [TestCase(0.333f, 0.666f, 0.999f)]
        [TestCase(0.1f, 0.35f, 0.45f)]
        [TestCase(0.987f, -0.9f, 0.087f)]
        public void AddTests(float x, float y, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            FPFloat fpSum = fpX + fpY;
            float actual = fpSum.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase(1f, 1f, 0f)]
        [TestCase(0.333f, 0.666f, -0.333f)]
        [TestCase(-0.9765f, -1f, 0.0235f)]
        [TestCase(10000f, 0.1f, 9999.9f)]
        public void SubstractTests(float x, float y, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            FPFloat fpDifference = fpX - fpY;
            float actual = fpDifference.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase(1234, 5678, 7006652)]
        [TestCase(100, 0.66f, 66f)]
        [TestCase(-0.375f, -1 / 3f, 0.125f)]
        [TestCase(99999f, 0f, 0f)]
        public void MultiplyTests(float x, float y, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            FPFloat fpDifference = fpX * fpY;
            float actual = fpDifference.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase(1234, 5678, 7006652)]
        [TestCase(100, 0.66f, 66f)]
        [TestCase(-0.375f, -1 / 3f, 0.125f)]
        [TestCase(99999f, 0f, 0f)]
        public void FastMultiplyTests(float x, float y, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            FPFloat result = FPFloat.FastMul(fpX, fpY);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase(100, 3, 100f / 3f)]
        [TestCase(0.3333f, 0.3333f, 1f)]
        [TestCase(-0.375f, -3f, 0.125f)]
        public void DivideTests(float x, float y, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            FPFloat result = fpX / fpY;
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase(100, 3, 1f)]
        [TestCase(0.3333f, 0.3333f, 0f)]
        [TestCase(-0.375f, -3f, -0.375f)]
        public void ModTests(float x, float y, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            FPFloat result = fpX % fpY;
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase(-9.7891f, 9.7891f)]
        [TestCase(0f, 0f)]
        public void NegativeTests(float x, float expected)
        {
            FPFloat fp = new FPFloat(x);
            FPFloat result = -fp;
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase(1234f, 1234f, true)]
        [TestCase(0.0001f, 0.0001001f, false)]
        [TestCase(-9.7891f, 9.7891f, false)]
        [TestCase(0f, 0f, true)]
        public void EqualTests(float x, float y, bool expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            bool actual = fpX == fpY;
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1234f, 1234f, false)]
        [TestCase(0.0001f, 0.0001001f, true)]
        [TestCase(-9.7891f, 9.7891f, true)]
        [TestCase(0f, 0f, false)]
        public void NotEqualTests(float x, float y, bool expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            bool actual = fpX != fpY;
            Assert.AreEqual(expected, actual);
        }

        [TestCase(0f, 0f, false)]
        [TestCase(1234f, 1234f, false)]
        [TestCase(0.0001f, 0.0001001f, false)]
        [TestCase(9.7892f, 9.7891f, true)]
        [TestCase(-1234f, 0.001f, false)]
        public void GreaterTests(float x, float y, bool expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            bool actual = fpX > fpY;
            Assert.AreEqual(expected, actual);
        }

        [TestCase(0f, 0f, false)]
        [TestCase(1234f, 1234f, false)]
        [TestCase(0.0001f, 0.0001001f, true)]
        [TestCase(9.7892f, 9.7891f, false)]
        [TestCase(-1234f, 0.001f, true)]
        public void LessTests(float x, float y, bool expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            bool actual = fpX < fpY;
            Assert.AreEqual(expected, actual);
        }

        [TestCase(0f, 0f, true)]
        [TestCase(1234f, 1234f, true)]
        [TestCase(0.0001f, 0.0001001f, false)]
        [TestCase(9.7892f, 9.7891f, true)]
        [TestCase(-1234f, 0.001f, false)]
        public void GreaterOrEqualTests(float x, float y, bool expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            bool actual = fpX >= fpY;
            Assert.AreEqual(expected, actual);
        }

        [TestCase(0f, 0f, true)]
        [TestCase(1234f, 1234f, true)]
        [TestCase(0.0001f, 0.0001001f, true)]
        [TestCase(9.7892f, 9.7891f, false)]
        [TestCase(-1234f, 0.001f, true)]
        public void LessOrEqualTests(float x, float y, bool expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            bool actual = fpX <= fpY;
            Assert.AreEqual(expected, actual);
        }
        #endregion
    }
}
