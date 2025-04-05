using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.Clustering;

namespace NumFlatTest.ClusteringTests
{
    public class KMedoidsTests
    {
        [Test]
        public void Build()
        {
            (int ClassIndex, Vec<double> Feature)[] source =
            [
                (0, [0, 0]),
                (0, [0, 1]),
                (0, [1, 0]),
                (0, [1, 1]),
                (0, [0.5, 0.5]),

                (1, [10.1, 0.6]),
                (1, [10.2, 0.4]),
                (1, [10.3, 0.3]),
                (1, [10.4, 0.5]),

                (2, [0, 10]),
                (2, [0, 10]),
                (2, [0, 10]),
                (2, [0, 10]),
                (2, [0, 10.5]),

                (3, [20.1, 10.8]),
                (3, [20.0, 10.9]),
                (3, [20.3, 10.7]),
                (3, [20.2, 10.5]),

                (4, [5.0, 20.3]),
                (4, [5.3, 20.3]),
                (4, [5.3, 20.0]),
                (4, [5.0, 20.1]),
                (4, [5.2, 20.5]),
            ];

            Shuffle(source);

            var items = source.Select(tpl => tpl.Feature).ToArray();

            var result = KMedoids<Vec<double>>.GetInitialGuessBuild(items, Distance.Euclidean, 5);

            int[] expected = [0, 1, 2, 3, 4];
            var actual = result.Medoids.Select(m => source[m].ClassIndex).Order().ToArray();
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static void Shuffle<T>(T[] array)
        {
            var random = new Random(42);
            for (int i = array.Length - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                var tmp = array[i];
                array[i] = array[j];
                array[j] = tmp;
            }
        }
    }
}
