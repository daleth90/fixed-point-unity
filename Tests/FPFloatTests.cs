using NUnit.Framework;
using System;

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

        [Test]
        public void PiTest()
        {
            TestUtility.AssertApproximatelyEqual(3.14159274f, FPFloat.Pi.ToValue());
        }

        [Test]
        public void PiTimes2Test()
        {
            TestUtility.AssertApproximatelyEqual(6.28318548f, FPFloat.PiTimes2.ToValue());
        }

        [Test]
        public void PiOver2Test()
        {
            TestUtility.AssertApproximatelyEqual(1.57079637f, FPFloat.PiOver2.ToValue());
        }

        [Test]
        public void Deg2RadTest()
        {
            TestUtility.AssertApproximatelyEqual(0.0174532924f, FPFloat.Deg2Rad.ToValue());
        }

        [Test]
        public void Rad2DegTest()
        {
            TestUtility.AssertApproximatelyEqual(57.29578f, FPFloat.Rad2Deg.ToValue());
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

        // Run sufficient times, to make sure that the values keep in range
        [Test]
        public void RandomMaxTests()
        {
            Random random = new Random();
            for (int i = 0; i < 1000000; i++)
            {
                FPFloat value = FPFloat.Random(random, 13);
                if (value < FPFloat.Zero || value >= 13)
                {
                    Assert.Fail();
                }
            }
        }

        // Run sufficient times, to make sure that the values keep in range
        [Test]
        public void RandomMinMaxTests()
        {
            Random random = new Random();
            for (int i = 0; i < 1000000; i++)
            {
                FPFloat value = FPFloat.Random(random, 9, 13);
                if (value < 9 || value >= 13)
                {
                    Assert.Fail();
                }
            }
        }

        // Run sufficient times, to make sure that the values keep in range
        [Test]
        public void Random01Tests()
        {
            Random random = new Random();
            for (int i = 0; i < 1000000; i++)
            {
                FPFloat value = FPFloat.Random01(random);
                if (value < FPFloat.Zero || value >= FPFloat.One)
                {
                    Assert.Fail();
                }
            }
        }

        [TestCase(-5.1f, -1)]
        [TestCase(-1f, -1)]
        [TestCase(0f, 0)]
        [TestCase(1f, 1)]
        [TestCase(5.1f, 1)]
        public void SignTests(float x, int expected)
        {
            FPFloat fpX = new FPFloat(x);
            int actual = FPFloat.Sign(fpX);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(-5.1f, 5.1f)]
        [TestCase(-1f, 1f)]
        [TestCase(0f, 0f)]
        [TestCase(1f, 1f)]
        [TestCase(5.1f, 5.1f)]
        public void AbsTests(float x, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat result = FPFloat.Abs(fpX);
            float actual = result.ToValue();
            Assert.AreEqual(expected, actual);
        }

        [TestCase(5.6f, 5.6008f, false)]
        [TestCase(5.6f, 5.599999f, true)]
        public void ApproximatelyTests(float x, float y, bool expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            bool actual = FPFloat.Approximately(fpX, fpY);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(-0.0001f, false)]
        [TestCase(-0.00001f, false)]
        [TestCase(-0.000008f, true)]
        [TestCase(0f, true)]
        [TestCase(0.000008f, true)]
        [TestCase(0.00001f, false)]
        [TestCase(0.0001f, false)]
        public void ApproximatelyZeroTests(float x, bool expected)
        {
            FPFloat fpX = new FPFloat(x);
            bool actual = FPFloat.ApproximatelyZero(fpX);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(-5.1f, -6f, -6f)]
        [TestCase(2f, 8f, 2f)]
        [TestCase(-4f, -4f, -4f)]
        public void MinTests(float x, float y, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            FPFloat result = FPFloat.Min(fpX, fpY);
            float actual = result.ToValue();
            Assert.AreEqual(expected, actual);
        }

        [TestCase(-5.1f, -6f, -5.1f)]
        [TestCase(2f, 8f, 8f)]
        [TestCase(-4f, -4f, -4f)]
        public void MaxTests(float x, float y, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            FPFloat result = FPFloat.Max(fpX, fpY);
            float actual = result.ToValue();
            Assert.AreEqual(expected, actual);
        }

        [TestCase(-5.1f, -6f, 0f, -5.1f)]
        [TestCase(-1f, 0f, 1f, 0f)]
        [TestCase(8f, -3f, 6f, 6f)]
        public void ClampTests(float value, float min, float max, float expected)
        {
            FPFloat fpValue = new FPFloat(value);
            FPFloat fpMin = new FPFloat(min);
            FPFloat fpMax = new FPFloat(max);
            FPFloat result = FPFloat.Clamp(fpValue, fpMin, fpMax);
            float actual = result.ToValue();
            Assert.AreEqual(expected, actual);
        }

        [TestCase(-0.00001f, 0f)]
        [TestCase(0f, 0f)]
        [TestCase(0.00001f, 0.00001f)]
        [TestCase(0.5491f, 0.5491f)]
        [TestCase(0.99999f, 0.99999f)]
        [TestCase(1f, 1f)]
        [TestCase(1.00001f, 1f)]
        public void Clamp01Tests(float value, float expected)
        {
            FPFloat fpValue = new FPFloat(value);
            FPFloat result = FPFloat.Clamp01(fpValue);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase(0f, 1f, 0f, 0f)]
        [TestCase(0f, 1f, 1f, 1f)]
        [TestCase(0f, 1f, 0.3f, 0.3f)]
        [TestCase(0f, 1f, -1f, 0f)]
        [TestCase(0f, 1f, 1.2f, 1f)]
        [TestCase(-4f, 6f, 0.5f, 1f)]
        [TestCase(6f, -4f, 0.9f, -3f)]
        public void LerpTests(float a, float b, float t, float expected)
        {
            FPFloat fpA = new FPFloat(a);
            FPFloat fpB = new FPFloat(b);
            FPFloat fpT = new FPFloat(t);
            FPFloat result = FPFloat.Lerp(fpA, fpB, fpT);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase(-5.1f, -6f)]
        [TestCase(-1f, -1f)]
        [TestCase(0f, 0f)]
        [TestCase(1f, 1f)]
        [TestCase(5.1f, 5f)]
        public void FloorTests(float x, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat result = FPFloat.Floor(fpX);
            float actual = result.ToValue();
            Assert.AreEqual(expected, actual);
        }

        [TestCase(-5.1f, -6)]
        [TestCase(-1f, -1)]
        [TestCase(0f, 0)]
        [TestCase(1f, 1)]
        [TestCase(5.1f, 5)]
        public void FloorToIntTests(float x, int expected)
        {
            FPFloat fpX = new FPFloat(x);
            int actual = FPFloat.FloorToInt(fpX);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(-5.1f, -5f)]
        [TestCase(-1f, -1f)]
        [TestCase(0f, 0f)]
        [TestCase(1f, 1f)]
        [TestCase(5.1f, 6f)]
        public void CeilingTests(float x, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat result = FPFloat.Ceiling(fpX);
            float actual = result.ToValue();
            Assert.AreEqual(expected, actual);
        }

        [TestCase(-5.1f, -5)]
        [TestCase(-1f, -1)]
        [TestCase(0f, 0)]
        [TestCase(1f, 1)]
        [TestCase(5.1f, 6)]
        public void CeilingToIntTests(float x, int expected)
        {
            FPFloat fpX = new FPFloat(x);
            int actual = FPFloat.CeilingToInt(fpX);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(-5.5f, -6f)]
        [TestCase(-5.1f, -5f)]
        [TestCase(-4.5f, -5f)]
        [TestCase(-4.4f, -4f)]
        [TestCase(-1f, -1f)]
        [TestCase(0f, 0f)]
        [TestCase(1f, 1f)]
        [TestCase(4.5f, 5f)]
        [TestCase(4.6f, 5f)]
        [TestCase(5.4f, 5f)]
        [TestCase(5.5f, 6f)]
        public void RoundTests(float x, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat result = FPFloat.Round(fpX);
            float actual = result.ToValue();
            Assert.AreEqual(expected, actual);
        }

        [TestCase(-5.5f, -6)]
        [TestCase(-5.1f, -5)]
        [TestCase(-4.5f, -5)]
        [TestCase(-4.4f, -4)]
        [TestCase(-1f, -1)]
        [TestCase(0f, 0)]
        [TestCase(1f, 1)]
        [TestCase(4.5f, 5)]
        [TestCase(4.6f, 5)]
        [TestCase(5.4f, 5)]
        [TestCase(5.5f, 6)]
        public void RoundToIntTests(float x, int expected)
        {
            FPFloat fpX = new FPFloat(x);
            int actual = FPFloat.RoundToInt(fpX);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(100f, 0f, 1f)]
        [TestCase(4f, 0.5f, 2f)]
        [TestCase(0.1f, 2f, 0.01f)]
        [TestCase(0.1f, 3f, 0.001f)]
        [TestCase(0.3f, 2f, 0.09f)]
        [TestCase(0.1f, -3f, 1000f)]
        public void PowTests(float x, float y, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            FPFloat result = FPFloat.Pow(fpX, fpY);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual, 0.0001f);  // Pow() has bigger floating error
        }

        [TestCase(100f, 10f)]
        [TestCase(4f, 2f)]
        [TestCase(16f, 4f)]
        [TestCase(0.01f, 0.1f)]
        public void SqrtTests(float x, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat result = FPFloat.Sqrt(fpX);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase((float)Math.PI * -2f, 0f)]
        [TestCase((float)Math.PI * -1.5f, 1f)]
        [TestCase((float)Math.PI * -1f, 0f)]
        [TestCase((float)Math.PI * -0.5f, -1f)]
        [TestCase(0f, 0f)]
        [TestCase((float)Math.PI * 0.5f, 1f)]
        [TestCase((float)Math.PI, 0f)]
        [TestCase((float)Math.PI * 1.5f, -1f)]
        [TestCase((float)Math.PI * 2f, 0f)]
        [TestCase((float)Math.PI * 2.5f, 1f)]
        [TestCase((float)Math.PI * 3f, 0f)]
        [TestCase((float)Math.PI * 3.5f, -1f)]
        [TestCase((float)Math.PI * 4f, 0f)]
        [TestCase((float)Math.PI * 1f / 6f, 0.5f)]
        [TestCase((float)Math.PI * 2f / 6f, 0.86602540378f)]
        [TestCase((float)Math.PI * 4f / 6f, 0.86602540378f)]
        [TestCase((float)Math.PI * 5f / 6f, 0.5f)]
        [TestCase((float)Math.PI * 7f / 6f, -0.5f)]
        [TestCase((float)Math.PI * 8f / 6f, -0.86602540378f)]
        [TestCase((float)Math.PI * 10f / 6f, -0.86602540378f)]
        [TestCase((float)Math.PI * 11f / 6f, -0.5f)]
        [TestCase((float)Math.PI * 0.25f, 0.707106781f)]
        [TestCase((float)Math.PI * 0.75f, 0.707106781f)]
        [TestCase((float)Math.PI * 1.25f, -0.707106781f)]
        [TestCase((float)Math.PI * 1.75f, -0.707106781f)]
        public void SinTests(float radian, float expected)
        {
            FPFloat fpRadian = new FPFloat(radian);
            FPFloat result = FPFloat.Sin(fpRadian);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase((float)Math.PI * -2f, 0f)]
        [TestCase((float)Math.PI * -1.5f, 1f)]
        [TestCase((float)Math.PI * -1f, 0f)]
        [TestCase((float)Math.PI * -0.5f, -1f)]
        [TestCase(0f, 0f)]
        [TestCase((float)Math.PI * 0.5f, 1f)]
        [TestCase((float)Math.PI, 0f)]
        [TestCase((float)Math.PI * 1.5f, -1f)]
        [TestCase((float)Math.PI * 2f, 0f)]
        [TestCase((float)Math.PI * 2.5f, 1f)]
        [TestCase((float)Math.PI * 3f, 0f)]
        [TestCase((float)Math.PI * 3.5f, -1f)]
        [TestCase((float)Math.PI * 4f, 0f)]
        [TestCase((float)Math.PI * 1f / 6f, 0.5f)]
        [TestCase((float)Math.PI * 2f / 6f, 0.86602540378f)]
        [TestCase((float)Math.PI * 4f / 6f, 0.86602540378f)]
        [TestCase((float)Math.PI * 5f / 6f, 0.5f)]
        [TestCase((float)Math.PI * 7f / 6f, -0.5f)]
        [TestCase((float)Math.PI * 8f / 6f, -0.86602540378f)]
        [TestCase((float)Math.PI * 10f / 6f, -0.86602540378f)]
        [TestCase((float)Math.PI * 11f / 6f, -0.5f)]
        [TestCase((float)Math.PI * 0.25f, 0.707106781f)]
        [TestCase((float)Math.PI * 0.75f, 0.707106781f)]
        [TestCase((float)Math.PI * 1.25f, -0.707106781f)]
        [TestCase((float)Math.PI * 1.75f, -0.707106781f)]
        public void FastSinTests(float radian, float expected)
        {
            FPFloat fpRadian = new FPFloat(radian);
            FPFloat result = FPFloat.FastSin(fpRadian);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase((float)Math.PI * -2f, 1f)]
        [TestCase((float)Math.PI * -1.5f, 0f)]
        [TestCase((float)Math.PI * -1f, -1f)]
        [TestCase((float)Math.PI * -0.5f, 0f)]
        [TestCase(0f, 1f)]
        [TestCase((float)Math.PI * 0.5f, 0f)]
        [TestCase((float)Math.PI * 1f, -1f)]
        [TestCase((float)Math.PI * 1.5f, 0f)]
        [TestCase((float)Math.PI * 2f, 1f)]
        [TestCase((float)Math.PI * 2.5f, 0f)]
        [TestCase((float)Math.PI * 3f, -1f)]
        [TestCase((float)Math.PI * 3.5f, 0f)]
        [TestCase((float)Math.PI * 4f, 1f)]
        [TestCase((float)Math.PI * 1f / 6f, 0.86602540378f)]
        [TestCase((float)Math.PI * 2f / 6f, 0.5f)]
        [TestCase((float)Math.PI * 4f / 6f, -0.5f)]
        [TestCase((float)Math.PI * 5f / 6f, -0.86602540378f)]
        [TestCase((float)Math.PI * 7f / 6f, -0.86602540378f)]
        [TestCase((float)Math.PI * 8f / 6f, -0.5f)]
        [TestCase((float)Math.PI * 10f / 6f, 0.5f)]
        [TestCase((float)Math.PI * 11f / 6f, 0.86602540378f)]
        [TestCase((float)Math.PI * 0.25f, 0.707106781f)]
        [TestCase((float)Math.PI * 0.75f, -0.707106781f)]
        [TestCase((float)Math.PI * 1.25f, -0.707106781f)]
        [TestCase((float)Math.PI * 1.75f, 0.707106781f)]
        public void CosTests(float radian, float expected)
        {
            FPFloat fpRadian = new FPFloat(radian);
            FPFloat result = FPFloat.Cos(fpRadian);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase((float)Math.PI * -2f, 1f)]
        [TestCase((float)Math.PI * -1.5f, 0f)]
        [TestCase((float)Math.PI * -1f, -1f)]
        [TestCase((float)Math.PI * -0.5f, 0f)]
        [TestCase(0f, 1f)]
        [TestCase((float)Math.PI * 0.5f, 0f)]
        [TestCase((float)Math.PI * 1f, -1f)]
        [TestCase((float)Math.PI * 1.5f, 0f)]
        [TestCase((float)Math.PI * 2f, 1f)]
        [TestCase((float)Math.PI * 2.5f, 0f)]
        [TestCase((float)Math.PI * 3f, -1f)]
        [TestCase((float)Math.PI * 3.5f, 0f)]
        [TestCase((float)Math.PI * 4f, 1f)]
        [TestCase((float)Math.PI * 1f / 6f, 0.86602540378f)]
        [TestCase((float)Math.PI * 2f / 6f, 0.5f)]
        [TestCase((float)Math.PI * 4f / 6f, -0.5f)]
        [TestCase((float)Math.PI * 5f / 6f, -0.86602540378f)]
        [TestCase((float)Math.PI * 7f / 6f, -0.86602540378f)]
        [TestCase((float)Math.PI * 8f / 6f, -0.5f)]
        [TestCase((float)Math.PI * 10f / 6f, 0.5f)]
        [TestCase((float)Math.PI * 11f / 6f, 0.86602540378f)]
        [TestCase((float)Math.PI * 0.25f, 0.707106781f)]
        [TestCase((float)Math.PI * 0.75f, -0.707106781f)]
        [TestCase((float)Math.PI * 1.25f, -0.707106781f)]
        [TestCase((float)Math.PI * 1.75f, 0.707106781f)]
        public void FastCosTests(float radian, float expected)
        {
            FPFloat fpRadian = new FPFloat(radian);
            FPFloat result = FPFloat.FastCos(fpRadian);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase(0f, 0f)]
        [TestCase((float)Math.PI * 0.25f, 1f)]
        [TestCase((float)Math.PI * 0.75f, -1f)]
        [TestCase((float)Math.PI * 1.25f, 1f)]
        [TestCase((float)Math.PI * 1.75f, -1f)]
        public void TanTests(float radian, float expected)
        {
            FPFloat fpRadian = new FPFloat(radian);
            FPFloat result = FPFloat.Tan(fpRadian);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase(0f, 0f)]
        [TestCase(1f, (float)Math.PI * 0.5f)]
        [TestCase(-1f, (float)Math.PI * -0.5f)]
        [TestCase(0.5f, (float)Math.PI * 1f / 6f)]
        [TestCase(-0.5f, (float)Math.PI * -1f / 6f)]
        [TestCase(0.86602540378f, (float)Math.PI * 2f / 6f)]
        [TestCase(-0.86602540378f, (float)Math.PI * -2f / 6f)]
        [TestCase(0.707106781f, (float)Math.PI * 0.25f)]
        [TestCase(-0.707106781f, (float)Math.PI * -0.25f)]
        public void AsinTests(float x, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat result = FPFloat.Asin(fpX);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase(0f, (float)Math.PI * 0.5f)]
        [TestCase(1f, 0f)]
        [TestCase(-1f, (float)Math.PI * 1f)]
        [TestCase(0.86602540378f, (float)Math.PI * 1f / 6f)]
        [TestCase(0.5f, (float)Math.PI * 2f / 6f)]
        [TestCase(-0.5f, (float)Math.PI * 4f / 6f)]
        [TestCase(-0.86602540378f, (float)Math.PI * 5f / 6f)]
        [TestCase(0.707106781f, (float)Math.PI * 0.25f)]
        [TestCase(-0.707106781f, (float)Math.PI * 0.75f)]
        public void AcosTests(float x, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat result = FPFloat.Acos(fpX);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase(0f, 0f)]
        [TestCase(1f, (float)Math.PI * 0.25f)]
        [TestCase(-1f, (float)Math.PI * -0.25f)]
        [TestCase(1.73205081f, (float)Math.PI * 1f / 3f)]
        [TestCase(-1.73205081f, (float)Math.PI * -1f / 3f)]
        public void AtanTests(float x, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat result = FPFloat.Atan(fpX);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }

        [TestCase(0f, 0f, 0f)]
        [TestCase(1f, 0f, (float)Math.PI * 0.5f)]
        [TestCase(-1f, 0f, (float)Math.PI * -0.5f)]
        [TestCase(0f, 1f, 0f)]
        [TestCase(0f, -1f, (float)Math.PI)]
        [TestCase(1f, 1f, (float)Math.PI * 0.25f)]
        [TestCase(1f, -1f, (float)Math.PI * 0.75f)]
        [TestCase(-1f, 1f, (float)Math.PI * -0.25f)]
        [TestCase(-1f, -1f, (float)Math.PI * -0.75f)]
        public void Atan2Tests(float y, float x, float expected)
        {
            FPFloat fpY = new FPFloat(y);
            FPFloat fpX = new FPFloat(x);
            FPFloat result = FPFloat.Atan2(fpY, fpX);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }
    }
}
