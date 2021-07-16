using NUnit.Framework;
using System;

namespace DeltaTimer.FixedPoint.Tests
{
    public class MathFPTests
    {
        #region Constant Tests
        [Test]
        public void PiTest()
        {
            TestUtility.AssertApproximatelyEqual(3.14159274f, MathFP.Pi.ToValue());
        }

        [Test]
        public void PiTimes2Test()
        {
            TestUtility.AssertApproximatelyEqual(6.28318548f, MathFP.PiTimes2.ToValue());
        }

        [Test]
        public void PiOver2Test()
        {
            TestUtility.AssertApproximatelyEqual(1.57079637f, MathFP.PiOver2.ToValue());
        }

        [Test]
        public void Deg2RadTest()
        {
            TestUtility.AssertApproximatelyEqual(0.0174532924f, MathFP.Deg2Rad.ToValue());
        }

        [Test]
        public void Rad2DegTest()
        {
            TestUtility.AssertApproximatelyEqual(57.29578f, MathFP.Rad2Deg.ToValue());
        }
        #endregion

        [TestCase(-5.1f, -1)]
        [TestCase(-1f, -1)]
        [TestCase(0f, 0)]
        [TestCase(1f, 1)]
        [TestCase(5.1f, 1)]
        public void SignTests(float x, int expected)
        {
            FPFloat fpX = new FPFloat(x);
            int actual = MathFP.Sign(fpX);
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
            FPFloat result = MathFP.Abs(fpX);
            float actual = result.ToValue();
            Assert.AreEqual(expected, actual);
        }

        [TestCase(5.6f, 5.6008f, false)]
        [TestCase(5.6f, 5.599999f, true)]
        public void ApproximatelyTests(float x, float y, bool expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            bool actual = MathFP.Approximately(fpX, fpY);
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
            bool actual = MathFP.ApproximatelyZero(fpX);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(-5.1f, -6f, -6f)]
        [TestCase(2f, 8f, 2f)]
        [TestCase(-4f, -4f, -4f)]
        public void MinTests(float x, float y, float expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            FPFloat result = MathFP.Min(fpX, fpY);
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
            FPFloat result = MathFP.Max(fpX, fpY);
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
            FPFloat result = MathFP.Clamp(fpValue, fpMin, fpMax);
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
            FPFloat result = MathFP.Clamp01(fpValue);
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
            FPFloat result = MathFP.Lerp(fpA, fpB, fpT);
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
            FPFloat result = MathFP.Floor(fpX);
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
            int actual = MathFP.FloorToInt(fpX);
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
            FPFloat result = MathFP.Ceiling(fpX);
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
            int actual = MathFP.CeilingToInt(fpX);
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
            FPFloat result = MathFP.Round(fpX);
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
            int actual = MathFP.RoundToInt(fpX);
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
            FPFloat result = MathFP.Pow(fpX, fpY);
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
            FPFloat result = MathFP.Sqrt(fpX);
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
            FPFloat result = MathFP.Sin(fpRadian);
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
            FPFloat result = MathFP.FastSin(fpRadian);
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
            FPFloat result = MathFP.Cos(fpRadian);
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
            FPFloat result = MathFP.FastCos(fpRadian);
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
            FPFloat result = MathFP.Tan(fpRadian);
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
            FPFloat result = MathFP.Asin(fpX);
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
            FPFloat result = MathFP.Acos(fpX);
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
            FPFloat result = MathFP.Atan(fpX);
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
            FPFloat result = MathFP.Atan2(fpY, fpX);
            float actual = result.ToValue();
            TestUtility.AssertApproximatelyEqual(expected, actual);
        }
    }
}
