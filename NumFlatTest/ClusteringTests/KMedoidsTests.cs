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
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Iris(int classCount)
        {
            var dataset = ReadIris("iris.csv").ToArray();
            var xs = dataset.Select(tpl => tpl.Feature).ToArray();

            var func = Distance.Euclidean;
            var medoids = new KMedoids<Vec<double>>(xs, classCount, func, new Random(42));

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
