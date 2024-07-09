using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class RandomExtensionsTests
    {
        [TestCase(42)]
        [TestCase(57)]
        [TestCase(137)]
        public void NextGaussian(int seed)
        {
            var random = new Random(seed);
            var values = Enumerable.Range(0, 1000).Select(i => random.NextGaussian()).ToArray();
            var (mean, sd) = values.MeanAndStandardDeviation();
            Assert.That(mean, Is.EqualTo(0.0).Within(0.1));
            Assert.That(sd, Is.EqualTo(1.0).Within(0.1));
        }

        [TestCase(42)]
        [TestCase(57)]
        [TestCase(137)]
        public void NextComplexGaussian(int seed)
        {
            var random = new Random(seed);
            var values = Enumerable.Range(0, 1000).Select(i => random.NextComplexGaussian()).ToArray();

            {
                var (mean, sd) = values.Select(x => x.Real).MeanAndStandardDeviation();
                Assert.That(mean, Is.EqualTo(0.0).Within(0.1));
                Assert.That(sd, Is.EqualTo(1.0).Within(0.1));
            }

            {
                var (mean, sd) = values.Select(x => x.Imaginary).MeanAndStandardDeviation();
                Assert.That(mean, Is.EqualTo(0.0).Within(0.1));
                Assert.That(sd, Is.EqualTo(1.0).Within(0.1));
            }
        }
    }
}
