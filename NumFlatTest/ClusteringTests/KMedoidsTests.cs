using System;
using System.Collections.Generic;
using System.IO;
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
        public void SimpleData()
        {
            var xs = new double[,]
            {
                {  1,  0,  1 },
                {  1,  1,  1 },
                {  5, -5, -5 },
                {  6, -4, -6 },
                {  6, -5, -4 },
                { -8,  2,  7 },
                { -9,  1,  8 },
                { -8,  3,  8 },
                { -8,  1,  9 },
            }
            .ToMatrix().Rows;

            var kmedoids = xs.ToKMedoids(DistanceMetric.Euclidean, 3, new Random(42));
            var cls = xs.Select(x => kmedoids.Predict(x)).ToArray();
            Assert.That(cls[0] == cls[1]);
            Assert.That(cls[2] == cls[3]);
            Assert.That(cls[2] == cls[4]);
            Assert.That(cls[5] == cls[6]);
            Assert.That(cls[5] == cls[7]);
            Assert.That(cls[5] == cls[8]);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void EnsureLocalOptimal(int classCount)
        {
            var dataset = ReadIris("iris.csv").ToArray();
            var xs = dataset.Select(tpl => tpl.Feature).ToArray();

            var func = DistanceMetric.Euclidean;
            var medoids = new KMedoids<Vec<double>>(xs, func, classCount, new Random(42));

            var originalError = 0.0;
            for (var i = 0; i < xs.Length; i++)
            {
                originalError += medoids.MedoidIndices.Select(m => func(xs[m], xs[i])).Min();
            }

            foreach (var cand in EnumerateNeighborhoodSolutions(medoids.MedoidIndices, xs.Length))
            {
                var newError = 0.0;
                for (var i = 0; i < xs.Length; i++)
                {
                    newError += cand.Select(m => func(xs[m], xs[i])).Min();
                }
                Assert.That(newError >= originalError);
            }
        }

        private static IEnumerable<(int Label, Vec<double> Feature)> ReadIris(string filename)
        {
            var path = Path.Combine("dataset", filename);
            foreach (var line in File.ReadLines(path).Skip(1))
            {
                var split = line.Split(',');
                var values = split.Take(4).Select(double.Parse);
                var label = split[4] switch
                {
                    "setosa" => 0,
                    "versicolor" => 1,
                    "virginica" => 2,
                    _ => throw new Exception()
                };
                yield return (label, values.ToVector());
            }
        }

        private static IEnumerable<int[]> EnumerateNeighborhoodSolutions(IReadOnlyList<int> original, int count)
        {
            var cand = new List<int>();
            for (var i = 0; i < count; i++)
            {
                if (!original.Contains(i))
                {
                    cand.Add(i);
                }
            }

            for (var k = 0; k < original.Count; k++)
            {
                var copy = original.ToArray();
                for (var c = 0; c < cand.Count; c++)
                {
                    copy[k] = cand[c];
                    yield return copy;
                }
            }
        }
    }
}
