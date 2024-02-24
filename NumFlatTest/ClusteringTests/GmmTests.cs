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

            Assert.That(actual.Components.Select(c => c.Weight).Sum(), Is.EqualTo(1.0).Within(1.0E-12));
        }

        [Test]
        public void Update()
        {
            var xs = ReadIris("iris.csv").ToArray();
            var kmeans = xs.ToKMeans(3, 3, new Random(42));

            var model = kmeans.ToGmm(xs);
            var likelihood = xs.Select(x => model.LogPdf(x)).Sum();
            for (var i = 0; i < 10; i++)
            {
                var newModel = model.Update(xs);
                var newLikelihood = xs.Select(x => newModel.LogPdf(x)).Sum();

                Assert.IsTrue(newLikelihood > likelihood);
                Assert.That(newModel.Components.Select(c => c.Weight).Sum(), Is.EqualTo(1.0).Within(1.0E-12));

                model = newModel;
                likelihood = newLikelihood;
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
    }
}
