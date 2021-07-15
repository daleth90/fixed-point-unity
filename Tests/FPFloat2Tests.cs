using NUnit.Framework;

namespace DeltaTimer.FixedPoint.Tests
{
    public class FPFloat2Tests
    {
        [TestCase(1f, 1f)]
        [TestCase(911.1613f, -4156.87941f)]
        public void ConstructorTests(float x, float y)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            FPFloat2 fPFloat2 = new FPFloat2(fpX, fpY);
            TestUtility.AreEqualFloat2(x, y, fPFloat2);
        }

        #region Constant Tests
        [Test]
        public void ZeroTest()
        {
            Assert.AreEqual(0f, FPFloat2.Zero.x.ToValue());
            Assert.AreEqual(0f, FPFloat2.Zero.y.ToValue());
        }

        [Test]
        public void RightTest()
        {
            Assert.AreEqual(1f, FPFloat2.Right.x.ToValue());
            Assert.AreEqual(0f, FPFloat2.Right.y.ToValue());
        }

        [Test]
        public void LeftTest()
        {
            Assert.AreEqual(-1f, FPFloat2.Left.x.ToValue());
            Assert.AreEqual(0f, FPFloat2.Left.y.ToValue());
        }

        [Test]
        public void UpTest()
        {
            Assert.AreEqual(0f, FPFloat2.Up.x.ToValue());
            Assert.AreEqual(1f, FPFloat2.Up.y.ToValue());
        }

        [Test]
        public void DownTest()
        {
            Assert.AreEqual(0f, FPFloat2.Down.x.ToValue());
            Assert.AreEqual(-1f, FPFloat2.Down.y.ToValue());
        }
        #endregion

        [TestCase(0f, 0f, 0f, 0f, true)]
        [TestCase(911.1613f, -1.87941f, 911.1613f, -1.87941f, true)]
        [TestCase(911.1613f, -1.87941f, 911.1613f, -1.87942f, false)]
        public void EqualsTests(float ax, float ay, float bx, float by, bool expected)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat fpBX = new FPFloat(bx);
            FPFloat fpBY = new FPFloat(by);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);
            FPFloat2 b = new FPFloat2(fpBX, fpBY);

            bool result = a.Equals(b);
            Assert.AreEqual(expected, result);
        }

        [TestCase(0f, 0f, 0f, 0f, true)]
        [TestCase(911.1613f, -1.87941f, 911.1613f, -1.87941f, true)]
        [TestCase(911.1613f, -1.87941f, 911.1613f, -1.87942f, false)]
        public void GetHashCodeTests(float ax, float ay, float bx, float by, bool expected)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat fpBX = new FPFloat(bx);
            FPFloat fpBY = new FPFloat(by);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);
            FPFloat2 b = new FPFloat2(fpBX, fpBY);

            bool result = a.GetHashCode() == b.GetHashCode();
            Assert.AreEqual(expected, result);
        }

        // Test the format, not the correction of the value
        [TestCase(0.5f, -2f, "(0.5, -2)")]
        public void ToStringTests(float x, float y, string expected)
        {
            FPFloat fpX = new FPFloat(x);
            FPFloat fpY = new FPFloat(y);
            FPFloat2 fPFloat2 = new FPFloat2(fpX, fpY);
            Assert.AreEqual(expected, fPFloat2.ToString());
        }

        #region Operator Tests
        [TestCase(1.23f, 0.4566f, 17.411f, -0.216f)]
        public void AddTests(float ax, float ay, float bx, float by)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat fpBX = new FPFloat(bx);
            FPFloat fpBY = new FPFloat(by);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);
            FPFloat2 b = new FPFloat2(fpBX, fpBY);

            FPFloat2 result = a + b;
            TestUtility.AssertApproximatelyEqual(ax + bx, result.x.ToValue());
            TestUtility.AssertApproximatelyEqual(ay + by, result.y.ToValue());
        }

        [TestCase(5.12f, -7.15105f, -451.8f, 12.38f)]
        public void SubstractTests(float ax, float ay, float bx, float by)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat fpBX = new FPFloat(bx);
            FPFloat fpBY = new FPFloat(by);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);
            FPFloat2 b = new FPFloat2(fpBX, fpBY);

            FPFloat2 result = a - b;
            TestUtility.AssertApproximatelyEqual(ax - bx, result.x.ToValue());
            TestUtility.AssertApproximatelyEqual(ay - by, result.y.ToValue());
        }

        [TestCase(1.2f, 9.9f, 3.5f)]
        public void MultiplyTests1(float ax, float ay, float d)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);
            FPFloat fpD = new FPFloat(d);

            FPFloat2 result = a * fpD;
            TestUtility.AssertApproximatelyEqual(ax * d, result.x.ToValue());
            TestUtility.AssertApproximatelyEqual(ay * d, result.y.ToValue());
        }

        [TestCase(1.2f, 9.9f, 3.5f)]
        public void MultiplyTests2(float ax, float ay, float d)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);
            FPFloat fpD = new FPFloat(d);

            FPFloat2 result = fpD * a;
            TestUtility.AssertApproximatelyEqual(ax * d, result.x.ToValue());
            TestUtility.AssertApproximatelyEqual(ay * d, result.y.ToValue());
        }

        [TestCase(9.8f, 2.5f, 2.4f)]
        public void DivideTests(float ax, float ay, float d)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);
            FPFloat fpD = new FPFloat(d);

            FPFloat2 result = a / fpD;
            TestUtility.AssertApproximatelyEqual(ax / d, result.x.ToValue());
            TestUtility.AssertApproximatelyEqual(ay / d, result.y.ToValue());
        }

        [TestCase(-45.5f, 123f)]
        public void NegativeTests(float ax, float ay)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);

            FPFloat2 result = -a;
            TestUtility.AssertApproximatelyEqual(-ax, result.x.ToValue());
            TestUtility.AssertApproximatelyEqual(-ay, result.y.ToValue());
        }

        [TestCase(-45.5f, 123f, -45.5f, 123f, true)]
        [TestCase(-45.5f, 123f, -45.5f, 123.001f, false)]
        public void EqualTests(float ax, float ay, float bx, float by, bool expected)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat fpBX = new FPFloat(bx);
            FPFloat fpBY = new FPFloat(by);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);
            FPFloat2 b = new FPFloat2(fpBX, fpBY);

            bool result = a == b;
            Assert.AreEqual(expected, result);
        }

        [TestCase(-45.5f, 123f, -45.5f, 123f, false)]
        [TestCase(-45.5f, 123f, -45.5f, 123.001f, true)]
        public void NotEqualTests(float ax, float ay, float bx, float by, bool expected)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat fpBX = new FPFloat(bx);
            FPFloat fpBY = new FPFloat(by);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);
            FPFloat2 b = new FPFloat2(fpBX, fpBY);

            bool result = a != b;
            Assert.AreEqual(expected, result);
        }
        #endregion

        #region Property Tests
        [TestCase(0f, 0f, 0f, 0f)]
        [TestCase(0f, 0.001f, 0f, 1f)]
        [TestCase(-0.1f, -0.1f, -0.707106781f, -0.707106781f)]
        [TestCase(-2.5f, 4.5f, -0.485642931f, 0.874157276f)]
        [TestCase(10.126788f, 9.11649484f, 0.743207082f, 0.669061457f)]
        public void NormalizedTests(float ax, float ay, float expectedX, float expectedY)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);

            // TODO: The value becomes VERY inaccurate after dividing Magnitude, and can only provide 3 decimals of accuracy. Should be fixed.
            FPFloat2 result = a.Normalized;
            TestUtility.AssertApproximatelyEqual(expectedX, result.x.ToValue(), 0.001f);
            TestUtility.AssertApproximatelyEqual(expectedY, result.y.ToValue(), 0.001f);
        }

        [TestCase(-2.5f, 4.5f, 26.5f)]
        public void SqrMagnitudeTests(float ax, float ay, float expected)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);

            FPFloat result = a.SqrMagnitude;
            TestUtility.AssertApproximatelyEqual(expected, result.ToValue());
        }

        [TestCase(-2.5f, 4.5f, 5.14781507f)]
        public void MagnitudeTests(float ax, float ay, float expected)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);

            FPFloat result = a.Magnitude;
            TestUtility.AssertApproximatelyEqual(expected, result.ToValue());
        }
        #endregion

        [TestCase(-2.5f, 4.5f, -2.5f, 4.5f, 0f)]
        [TestCase(-2.5f, 4.5f, 1f, -5f, 10.1242284f)]
        public void DistanceTests(float ax, float ay, float bx, float by, float expected)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat fpBX = new FPFloat(bx);
            FPFloat fpBY = new FPFloat(by);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);
            FPFloat2 b = new FPFloat2(fpBX, fpBY);

            FPFloat result = FPFloat2.Distance(a, b);
            TestUtility.AssertApproximatelyEqual(expected, result.ToValue());
        }

        [TestCase(-1.355f, 6.111f, 12.05f, 9.0181f, 38.7818591f)]
        public void DotTests(float ax, float ay, float bx, float by, float expected)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat fpBX = new FPFloat(bx);
            FPFloat fpBY = new FPFloat(by);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);
            FPFloat2 b = new FPFloat2(fpBX, fpBY);

            FPFloat result = FPFloat2.Dot(a, b);
            TestUtility.AssertApproximatelyEqual(expected, result.ToValue());
        }

        [TestCase(1f, 0f, 1f, 0f, 0f)]
        [TestCase(1f, 0f, -1f, 0f, 180f)]
        [TestCase(1f, 0f, 0.707106781f, 0.707106781f, 45f)]
        [TestCase(0.707106781f, 0.707106781f, 1f, 0f, 45f)]
        [TestCase(-2f, 0f, 0.707106781f, 0.707106781f, 135f)]
        public void AngleTests(float ax, float ay, float bx, float by, float expected)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat fpBX = new FPFloat(bx);
            FPFloat fpBY = new FPFloat(by);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);
            FPFloat2 b = new FPFloat2(fpBX, fpBY);

            FPFloat result = FPFloat2.Angle(a, b);
            TestUtility.AssertApproximatelyEqual(expected, result.ToValue());
        }

        [TestCase(0f, 1f, 90f, -1f, 0f)]
        [TestCase(1f, 0f, 90f, 0f, 1f)]
        [TestCase(1f, 0f, -90f, 0f, -1f)]
        [TestCase(-0.707106781f, -0.707106781f, 45f, 0f, -1f)]
        [TestCase(-1f, 0f, 45f, -0.707106781f, -0.707106781f)]
        public void RotateTests(float ax, float ay, float degree, float expectedX, float expectedY)
        {
            FPFloat fpAX = new FPFloat(ax);
            FPFloat fpAY = new FPFloat(ay);
            FPFloat2 a = new FPFloat2(fpAX, fpAY);
            FPFloat fpDegree = new FPFloat(degree);

            FPFloat2 result = a.Rotate(fpDegree);
            TestUtility.AssertApproximatelyEqual(expectedX, expectedY, result);
        }

        [TestCase(1f, 1f, 0f, -1f, 1f, -1f)]
        [TestCase(2f, 1f, 1f, 0f, -2f, 1f)]
        [TestCase(1f, 0f, -0.707106781f, -0.707106781f, 0f, -1f)]
        public void ReflectTests(float dx, float dy, float nx, float ny, float expectedX, float expectedY)
        {
            FPFloat fpDX = new FPFloat(dx);
            FPFloat fpDY = new FPFloat(dy);
            FPFloat fpNX = new FPFloat(nx);
            FPFloat fpNY = new FPFloat(ny);
            FPFloat2 d = new FPFloat2(fpDX, fpDY);
            FPFloat2 n = new FPFloat2(fpNX, fpNY);

            FPFloat2 result = FPFloat2.Reflect(d, n);
            TestUtility.AssertApproximatelyEqual(expectedX, expectedY, result);
        }
    }
}
