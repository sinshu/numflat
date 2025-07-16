using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.TimeSeries;

namespace NumFlatTest.TimeSeriesTests
{
    public class SubsequenceDynamicTimeWarpingTests
    {
        private static DistanceMetric<int, int> intAbs = (x, y) => Math.Abs(x - y);

        [Test]
        public void Best1()
        {
            int[] query = [3, 1, 4, 1, 5];
            int[] longSequence = [0, 0, 0, 3, 3, 1, 1, 4, 1, 1, 1, 5, 5, 5, 0, 0, 0];

            (int, int)[] expected =
            {
                (0, 4),
                (1, 5),
                (1, 6),
                (2, 7),
                (3, 8),
                (3, 9),
                (3, 10),
                (4, 11),
            };

            var actual = SubsequenceDynamicTimeWarping.GetBestAlignment(query, longSequence, intAbs);

            Assert.That(actual.Length == expected.Length);

            for (var i = 0; i < actual.Length; i++)
            {
                Assert.That(actual[i].First == expected[i].Item1);
                Assert.That(actual[i].Second == expected[i].Item2);
            }
        }

        [Test]
        public void Best2()
        {
            int[] query = [3, 1, 4, 1, 5];
            int[] longSequence = [0, 0, 0, 3, 3, 1, 1, 1, 1, 1, 1, 5, 5, 5, 0, 0, 0];

            var best = SubsequenceDynamicTimeWarping.GetBestAlignment(query, longSequence, intAbs);
            var start = best.First();
            var end = best.Last();

            Assert.That(start.First == 0);
            Assert.That(start.Second == 4);
            Assert.That(end.First == 4);
            Assert.That(end.Second == 11);
        }

        [Test]
        public void Alignment()
        {
            int[] query = [1, 2, 3];
            int[] longSequence = [0, 0, 0, 0, 1, 2, 3, 0, 0, 1, 1, 2, 2, 3, 3, 0, 0, 0, 1, 2, 3, 0];

            var sdtw = SubsequenceDynamicTimeWarping.Compute(query, longSequence, intAbs);
            var cost = sdtw.CostMatrix.Rows.Last();
            Console.WriteLine(cost);
            for (var i = 0; i < longSequence.Length; i++)
            {
                if (longSequence[i] == 3)
                {
                    Assert.That(cost[i] == 0);
                    var alignment = sdtw.GetAlignment(i);
                    foreach (var pair in alignment)
                    {
                        Assert.That(query[pair.First] == longSequence[pair.Second]);
                    }
                }
                else
                {
                    Assert.That(cost[i] > 0);
                }
            }
        }
    }
}
