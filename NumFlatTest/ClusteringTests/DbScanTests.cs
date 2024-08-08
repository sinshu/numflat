using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.Clustering;
using System.IO;
using NumFlat.Distributions;

namespace NumFlatTest.ClusteringTests
{
    public class DbScanTests
    {
        [Test]
        public void Test1()
        {
            Vec<double>[] xs =
            [
                // Noise
                [-100, -100],

                // cluster 1
                [0, 0],
                [0, 1],
                [1, 0],
                [1, 1],

                // Noise
                [100, 100],

                // cluster 2
                [10, 0],
                [10, 1],
                [11, 0],
                [11, 1],

                // Noise
                [200, 200],

                // cluster 3
                [0, 10],
                [0, 11],
                [1, 10],
                [1, 11],

                // Noise
                [1000, 1000],
            ];

            int[] expected = [-1, 0, 0, 0, 0, -1, 1, 1, 1, 1, -1, 2, 2, 2, 2, -1];

            var actual = xs.DbScan(2, 3);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Test2()
        {
            Vec<double>[] xs =
            [
                // Noise
                [-100, -100],

                // cluster 1
                [0, 0],
                [0, 1],
                [1, 0],
                [1, 1],

                // Noise
                [100, 100],

                // cluster 2
                [10, 0],
                [10, 1],
                [11, 0],
                [11, 1],

                // Noise
                [200, 200],

                // cluster 3
                [0, 10],
                [0, 11],
                [1, 10],
                [1, 11],

                // Noise
                [1000, 1000],
            ];

            int[] expected = Enumerable.Range(0, xs.Length).ToArray();

            var actual = xs.DbScan(0.5, 1);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Test3()
        {
            Vec<double>[] xs =
            [
                // Noise
                [-100, -100],

                // cluster 1
                [0, 0],
                [0, 1],
                [1, 0],
                [1, 1],

                // Noise
                [100, 100],

                // cluster 2
                [10, 0],
                [10, 1],
                [11, 0],
                [11, 1],

                // Noise
                [200, 200],

                // cluster 3
                [0, 10],
                [0, 11],
                [1, 10],
                [1, 11],

                // Noise
                [1000, 1000],
            ];

            int[] expected = xs.Select(x=>-1).ToArray();

            var actual = xs.DbScan(0.5, 2);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Test4()
        {
            Vec<double>[] xs =
            [
                // Noise
                [-100, -100],

                // cluster 1
                [0, 0],
                [0, 1],
                [1, 0],
                [1, 1],

                // Noise
                [100, 100],

                // cluster 2
                [10, 0],
                [10, 1],
                [11, 0],
                [11, 1],

                // Noise
                [200, 200],

                // cluster 3
                [0, 10],
                [0, 11],
                [1, 10],
                [1, 11],

                // Noise
                [1000, 1000],
            ];

            int[] expected = xs.Select(x => 0).ToArray();

            var actual = xs.DbScan(10000, 10);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Test5()
        {
            Vec<double>[] xs =
            [
                [0, 0],
                [10, 10],
                [20, 20],

                [100, 100],

                [0, 1],
                [10, 11],
                [20, 21],

                [200, 200],

                [1, 0],
                [11, 10],
                [21, 20],

                [300, 300],

                [1, 1],
                [11, 11],
                [21, 21],
            ];

            int[] expected = [0, 1, 2, -1, 0, 1, 2, -1, 0, 1, 2, -1, 0, 1, 2];

            var actual = xs.DbScan(2, 3);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
