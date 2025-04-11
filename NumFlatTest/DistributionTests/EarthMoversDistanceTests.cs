using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.Distributions;

namespace NumFlatTest.DistributionTests
{
    public class EarthMoversDistanceTests
    {
        [Test]
        public void Example1()
        {
            Vec<double>[] f1 =
            [
                [100, 40, 22],
                [211, 20, 2],
                [32, 190, 150],
                [2, 100, 100],
            ];
            Vec<double>[] f2 =
            [
                [0, 0, 0],
                [50, 100, 80],
                [255, 255, 255],
            ];

            double[] w1 = [0.4, 0.3, 0.2, 0.1];
            double[] w2 = [0.5, 0.3, 0.2];

            var s1 = new EmdSignature<Vec<double>>(f1, w1);
            var s2 = new EmdSignature<Vec<double>>(f2, w2);
            var actual = EarthMoversDistance.GetDistance(s1, s2, (x, y) => x.Distance(y));

            var expected = 160.542770;
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-5));
        }

        [Test]
        public void Example2()
        {
            double[][] cost =
            [
                [3, 5, 2],
                [0, 2, 5],
                [1, 1, 3],
                [8, 4, 3],
                [7, 6, 5],
            ];

            int[] f1 = [0, 1, 2, 3, 4];
            int[] f2 = [0, 1, 2];
            double[] w1 = [0.4, 0.2, 0.2, 0.1, 0.1];
            double[] w2 = [0.6, 0.2, 0.1];
            var s1 = new EmdSignature<int>(f1, w1);
            var s2 = new EmdSignature<int>(f2, w2);

            var (actualDistance, actualFlow) = EarthMoversDistance.GetDistanceAndFlow(s1, s2, (x, y) => cost[x][y]);

            var expectedDistance = 1.888889;
            Assert.That(actualDistance, Is.EqualTo(expectedDistance).Within(1.0E-5));

            Assert.That(actualFlow.Length, Is.EqualTo(6));

            (int From, int To, double Amount)[] expectedFlow =
            [
                (1, 0, 0.200000),
                (0, 0, 0.300000),
                (2, 0, 0.100000),
                (3, 1, 0.100000),
                (2, 1, 0.100000),
                (0, 2, 0.100000),
            ];

            for (var i = 0; i < actualFlow.Length; i++)
            {
                var actual = actualFlow[i];
                var expected = expectedFlow[i];
                Assert.That(actual.From, Is.EqualTo(expected.From));
                Assert.That(actual.To, Is.EqualTo(expected.To));
                Assert.That(actual.Amount, Is.EqualTo(expected.Amount).Within(1.0E-5));
            }
        }
    }
}
