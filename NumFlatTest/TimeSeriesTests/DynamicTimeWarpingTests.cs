using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.TimeSeries;

namespace NumFlatTest.TimeSeriesTests
{
    public class DynamicTimeWarpingTests
    {
        private static DistanceMetric<int, int> intAbs = (x, y) => Math.Abs(x - y);
        private static DistanceMetric<double, double> doubleAbs = (x, y) => Math.Abs(x - y);
        private static DistanceMetric<char, char> levenshtein = (x, y) => x == y ? 0 : 1;

        [Test]
        public void Zero()
        {
            int[] xs = [1, 1, 1, 2, 2, 2, 3, 3, 3, 1, 1, 1, 2, 2, 2, 3, 3, 3];
            int[] ys = [1, 1, 1, 1, 2, 2, 3, 3, 3, 3, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3];

            {
                var distance = DynamicTimeWarping.GetDistance(xs, ys, intAbs);
                Assert.That(distance, Is.EqualTo(0));
            }

            {
                var distance = DynamicTimeWarping.GetDistance(ys, xs, intAbs);
                Assert.That(distance, Is.EqualTo(0));
            }
        }

        [Test]
        public void One()
        {
            int[] xs = [1, 1, 1, 2, 2, 2, 3, 3, 3, 1, 1, 1, 2, 2, 2, 3, 3, 3];
            int[] ys = [1, 1, 1, 1, 2, 2, 3, 3, 4, 3, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3];

            {
                var distance = DynamicTimeWarping.GetDistance(xs, ys, intAbs);
                Assert.That(distance, Is.EqualTo(1));
            }

            {
                var distance = DynamicTimeWarping.GetDistance(ys, xs, intAbs);
                Assert.That(distance, Is.EqualTo(1));
            }
        }

        [Test]
        public void Alignment1()
        {
            int[] xs = [1, 1, 1, 2, 2, 2, 3, 3, 3, 1, 1, 1, 2, 2, 2, 3, 3, 3];
            int[] ys = [1, 1, 1, 1, 2, 2, 3, 3, 3, 3, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3];

            {
                var result = DynamicTimeWarping.GetDistanceAndAlignment(xs, ys, intAbs);
                Assert.That(result.Distance, Is.EqualTo(0));

                {
                    var head = result.Alignment.First();
                    Assert.That(head.First == 0);
                    Assert.That(head.Second == 0);
                    var tail = result.Alignment.Last();
                    Assert.That(tail.First == xs.Length - 1);
                    Assert.That(tail.Second == ys.Length - 1);
                    foreach (var pair in result.Alignment)
                    {
                        Assert.That(xs[pair.First] == ys[pair.Second]);
                    }
                }
            }

            {
                var result = DynamicTimeWarping.GetDistanceAndAlignment(ys, xs, intAbs);
                Assert.That(result.Distance, Is.EqualTo(0));

                {
                    var head = result.Alignment.First();
                    Assert.That(head.First == 0);
                    Assert.That(head.Second == 0);
                    var tail = result.Alignment.Last();
                    Assert.That(tail.First == ys.Length - 1);
                    Assert.That(tail.Second == xs.Length - 1);
                    foreach (var pair in result.Alignment)
                    {
                        Assert.That(ys[pair.First] == xs[pair.Second]);
                    }
                }
            }
        }

        [Test]
        public void Alignment2()
        {
            int[] xs = [1, 1, 1, 2, 2, 2, 3, 3, 3, 1, 1, 1, 2, 2, 2, 3, 3, 3];
            int[] ys = [1, 1, 1, 1, 2, 2, 3, 3, 4, 3, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3];

            {
                var result = DynamicTimeWarping.GetDistanceAndAlignment(xs, ys, intAbs);
                Assert.That(result.Distance, Is.EqualTo(1));

                {
                    var head = result.Alignment.First();
                    Assert.That(head.First == 0);
                    Assert.That(head.Second == 0);
                    var tail = result.Alignment.Last();
                    Assert.That(tail.First == xs.Length - 1);
                    Assert.That(tail.Second == ys.Length - 1);
                    foreach (var pair in result.Alignment)
                    {
                        if (ys[pair.Second] != 4)
                        {
                            Assert.That(xs[pair.First] == ys[pair.Second]);
                        }
                        else
                        {
                            Assert.That(xs[pair.First] == 3);
                        }
                    }
                }
            }

            {
                var result = DynamicTimeWarping.GetDistanceAndAlignment(ys, xs, intAbs);
                Assert.That(result.Distance, Is.EqualTo(1));

                {
                    var head = result.Alignment.First();
                    Assert.That(head.First == 0);
                    Assert.That(head.Second == 0);
                    var tail = result.Alignment.Last();
                    Assert.That(tail.First == ys.Length - 1);
                    Assert.That(tail.Second == xs.Length - 1);
                    foreach (var pair in result.Alignment)
                    {
                        if (ys[pair.First] != 4)
                        {
                            Assert.That(ys[pair.First] == xs[pair.Second]);
                        }
                        else
                        {
                            Assert.That(xs[pair.Second] == 3);
                        }
                    }
                }
            }
        }

        [TestCase("a", "b", 1)]
        [TestCase("abc", "def", 3)]
        [TestCase("abcdef", "azced", 3)]
        [TestCase("kitten", "sitting", 3)]
        [TestCase("intention", "execution", 5)]
        [TestCase("Saturday", "Sunday", 3)]
        [TestCase("distance", "editing", 5)]
        [TestCase("abcdefg", "acdfg", 2)]
        public void Levenshtein(string x, string y, double expected)
        {
            {
                var distance = DynamicTimeWarping.GetDistance(x.ToArray(), y.ToArray(), levenshtein);
                Assert.That(distance, Is.EqualTo(expected));
            }

            {
                var distance = DynamicTimeWarping.GetDistance(y.ToArray(), x.ToArray(), levenshtein);
                Assert.That(distance, Is.EqualTo(expected));
            }

            {
                var distance = DynamicTimeWarping.GetDistanceAndAlignment(x.ToArray(), y.ToArray(), levenshtein).Distance;
                Assert.That(distance, Is.EqualTo(expected));
            }

            {
                var distance = DynamicTimeWarping.GetDistanceAndAlignment(y.ToArray(), x.ToArray(), levenshtein).Distance;
                Assert.That(distance, Is.EqualTo(expected));
            }
        }

        [Test]
        public void Gpt4()
        {
            var seq1 = new double[] {
                0.0469, 0.2466, 0.2881, 0.5178, 0.6026, 0.7694, 0.8331, 0.9941, 0.9711, 1.0111,
                0.9767, 0.9765, 0.8479, 0.8409, 0.6595, 0.5734, 0.4431, 0.3204, 0.1355, 0.0077,
               -0.1393, -0.3245, -0.4185, -0.5802, -0.6456, -0.7918, -0.8471, -0.9405, -0.9814,
               -1.0104, -0.9790, -0.9384, -0.8851, -0.7575, -0.6613, -0.5381, -0.4089, -0.2612,
               -0.0996, 0.0230, 0.1492, 0.3385, 0.4534, 0.6036, 0.7056, 0.8251, 0.9076, 0.9750,
                1.0115, 1.0134, 0.9763, 0.9020, 0.7820, 0.6404, 0.5093, 0.3153, 0.2062
            };

            var seq2 = new double[] {
               -0.0362, 0.1607, 0.2561, 0.3786, 0.5097, 0.6113, 0.7645, 0.8382, 0.9637, 1.0132,
                0.9874, 1.0168, 0.9653, 0.9173, 0.8608, 0.7453, 0.6611, 0.5670, 0.4308, 0.2829,
                0.1766, 0.0420, -0.1050, -0.2193, -0.3637, -0.4775, -0.5941, -0.6791, -0.8192,
               -0.8703, -0.9311, -0.9745, -0.9950, -0.9605, -0.9110, -0.8535, -0.7576, -0.6575,
               -0.5464, -0.4072, -0.2892, -0.1242, -0.0001, 0.1513, 0.2593, 0.4063, 0.5400, 0.6690,
                0.7736, 0.8764, 0.9568, 1.0011, 0.9937, 0.9823, 0.9339, 0.8443, 0.7160, 0.5985,
                0.4562, 0.3205, 0.1777, 0.0629, -0.0556, -0.1251
            };

            {
                var distance = DynamicTimeWarping.GetDistance(seq1, seq2, doubleAbs);
                Assert.That(distance, Is.EqualTo(2.573).Within(1.0E-12));
            }

            {
                var distance = DynamicTimeWarping.GetDistance(seq2, seq1, doubleAbs);
                Assert.That(distance, Is.EqualTo(2.573).Within(1.0E-12));
            }

            {
                var result = DynamicTimeWarping.GetDistanceAndAlignment(seq1, seq2, doubleAbs);
                Assert.That(result.Distance, Is.EqualTo(2.573).Within(1.0E-12));
                var sum = result.Alignment.Select(pair => Math.Abs(seq1[pair.First] - seq2[pair.Second])).Sum();
                Assert.That(sum, Is.EqualTo(2.573).Within(1.0E-12));
            }

            {
                var result = DynamicTimeWarping.GetDistanceAndAlignment(seq2, seq1, doubleAbs);
                Assert.That(result.Distance, Is.EqualTo(2.573).Within(1.0E-12));
                var sum = result.Alignment.Select(pair => Math.Abs(seq2[pair.First] - seq1[pair.Second])).Sum();
                Assert.That(sum, Is.EqualTo(2.573).Within(1.0E-12));
            }
        }
    }
}
