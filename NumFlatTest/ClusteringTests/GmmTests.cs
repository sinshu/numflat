using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.Clustering;
using System.IO;

namespace NumFlatTest
{
    public class GmmTests
    {
        [Test]
        public void KMeansInitialModel()
        {
            var xs = ReadIris("iris.csv").ToArray();
            var kmeans = xs.ToKMeans(3, 1, new Random(42));

            var groups = Enumerable.Range(0, kmeans.ClassCount).Select(i => xs.Where(x => kmeans.Predict(x) == i).ToArray()).ToArray();
            var expected = groups.Select(group => group.MeanAndCovariance()).ToArray();

            var actual = kmeans.ToGmm(xs);

            for (var i = 0; i < kmeans.ClassCount; i++)
            {
                var weight = (double)groups[i].Length / groups.Select(group => group.Length).Sum();
                var logWeight = Math.Log(weight);
                NumAssert.AreSame(expected[i].Mean, actual.Components[i].Gaussian.Mean, 1.0E-12);
                NumAssert.AreSame(expected[i].Covariance, actual.Components[i].Gaussian.Covariance, 1.0E-12);
                Assert.That(actual.Components[i].Weight, Is.EqualTo(weight).Within(1.0E-12));
                Assert.That(actual.Components[i].LogWeight, Is.EqualTo(logWeight).Within(1.0E-12));
            }
        }

        private static IEnumerable<Vec<double>> ReadIris(string filename)
        {
            var path = Path.Combine("dataset", filename);
            foreach (var line in File.ReadLines(path).Skip(1))
            {
                var values = line.Split(',').Take(4).Select(double.Parse);
                yield return values.ToVector();
            }
        }

        private static IEnumerable<int> ReadIrisReference(string filename)
        {
            var path = Path.Combine("dataset", filename);
            foreach (var line in File.ReadLines(path).Skip(1))
            {
                yield return int.Parse(line);
            }
        }
    }
}
