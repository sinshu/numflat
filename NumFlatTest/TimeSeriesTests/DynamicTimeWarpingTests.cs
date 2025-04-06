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
        [Test]
        public void Zero()
        {
            int[] xs = [1, 1, 1, 2, 2, 2, 3, 3, 3, 1, 1, 1, 2, 2, 2, 3, 3, 3];
            int[] ys = [1, 1, 1, 1, 2, 2, 3, 3, 3, 3, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3];

            {
                var distance = DynamicTimeWarping.GetDistance(xs, ys, new IntAbs());
                Assert.That(distance, Is.EqualTo(0));
            }

            {
                var distance = DynamicTimeWarping.GetDistance(ys, xs, new IntAbs());
                Assert.That(distance, Is.EqualTo(0));
            }
        }

        [Test]
        public void One()
        {
            int[] xs = [1, 1, 1, 2, 2, 2, 3, 3, 3, 1, 1, 1, 2, 2, 2, 3, 3, 3];
            int[] ys = [1, 1, 1, 1, 2, 2, 3, 3, 4, 3, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3];

            {
                var distance = DynamicTimeWarping.GetDistance(xs, ys, new IntAbs());
                Assert.That(distance, Is.EqualTo(1));
            }

            {
                var distance = DynamicTimeWarping.GetDistance(ys, xs, new IntAbs());
                Assert.That(distance, Is.EqualTo(1));
            }
        }

        [Test]
        public void Alignment1()
        {
            int[] xs = [1, 1, 1, 2, 2, 2, 3, 3, 3, 1, 1, 1, 2, 2, 2, 3, 3, 3];
            int[] ys = [1, 1, 1, 1, 2, 2, 3, 3, 3, 3, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3];

            {
                var result = DynamicTimeWarping.GetDistanceAndAlignment(xs, ys, new IntAbs());
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
                var result = DynamicTimeWarping.GetDistanceAndAlignment(ys, xs, new IntAbs());
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
                var result = DynamicTimeWarping.GetDistanceAndAlignment(xs, ys, new IntAbs());
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
                var result = DynamicTimeWarping.GetDistanceAndAlignment(ys, xs, new IntAbs());
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
                var distance = DynamicTimeWarping.GetDistance(x.ToArray(), y.ToArray(), new Levenshtein());
                Assert.That(distance, Is.EqualTo(expected));
            }

            {
                var distance = DynamicTimeWarping.GetDistance(y.ToArray(), x.ToArray(), new Levenshtein());
                Assert.That(distance, Is.EqualTo(expected));
            }

            {
                var distance = DynamicTimeWarping.GetDistanceAndAlignment(x.ToArray(), y.ToArray(), new Levenshtein()).Distance;
                Assert.That(distance, Is.EqualTo(expected));
            }

            {
                var distance = DynamicTimeWarping.GetDistanceAndAlignment(y.ToArray(), x.ToArray(), new Levenshtein()).Distance;
                Assert.That(distance, Is.EqualTo(expected));
            }
        }
    }

    class Levenshtein : IDistance<char, char>
    {
        public double GetDistance(char x, char y)
        {
            return x == y ? 0 : 1;
        }
    }

    class IntAbs : IDistance<int, int>
    {
        public double GetDistance(int x, int y)
        {
            return Math.Abs(x - y);
        }
    }
}
