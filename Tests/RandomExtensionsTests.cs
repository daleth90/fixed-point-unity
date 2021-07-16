using NUnit.Framework;
using System;

namespace DeltaTimer.FixedPoint.Tests
{
    public class RandomExtensionsTests
    {
        // Run sufficient times, to make sure that the values keep in range
        [Test]
        public void RandomMaxTests()
        {
            Random random = new Random();
            for (int i = 0; i < 1000000; i++)
            {
                FPFloat value = random.NextFloat(13);
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
                FPFloat value = random.NextFloat(9, 13);
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
                FPFloat value = random.NextFloat01();
                if (value < FPFloat.Zero || value >= FPFloat.One)
                {
                    Assert.Fail();
                }
            }
        }
    }
}
