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
    public class DiagonalGmmTests
    {
        [Test]
        public void KMeansInitialModel()
        {
            var xs = ReadIris("iris.csv").ToArray();
            var kmeans = xs.ToKMeans(3, 1, new Random(42));

            var groups = Enumerable.Range(0, kmeans.ClassCount).Select(i => xs.Where(x => kmeans.Predict(x) == i).ToArray()).ToArray();
            var expected = groups.Select(group => group.MeanAndVariance()).ToArray();

            var actual = kmeans.ToDiagonalGmm(xs);

            for (var i = 0; i < kmeans.ClassCount; i++)
            {
                var weight = (double)groups[i].Length / groups.Select(group => group.Length).Sum();
                var logWeight = Math.Log(weight);
                NumAssert.AreSame(expected[i].Mean, actual.Components[i].Gaussian.Mean, 1.0E-12);
                NumAssert.AreSame(expected[i].Variance, actual.Components[i].Gaussian.Variance, 1.0E-12);
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

            var model = kmeans.ToDiagonalGmm(xs);
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

        [Test]
        public void SimpleData()
        {
            var cls1 = new double[,]
            {
                { 1, -3 },
                { 0, -2 },
                { 1, -2 },
                { 0, -1 },
                { 2, -1 },
                { 3, -2 },
            }
            .ToMatrix().Rows;

            var cls2 = new double[,]
            {
                { 7, 8 },
                { 8, 7 },
                { 9, 8 },
                { 9, 7 },
                { 8, 8 },
            }
            .ToMatrix().Rows;

            var cls3 = new double[,]
            {
                { 7, 1 },
                { 8, 1 },
                { 9, 1 },
                { 7, 2 },
                { 8, 2 },
                { 9, 2 },
                { 8, 3 },
            }
            .ToMatrix().Rows;

            var xs = cls1.Concat(cls2).Concat(cls3).ToArray();
            var ys = cls1.Select(x => 0).Concat(cls2.Select(x => 1)).Concat(cls3.Select(x => 2)).ToArray();
            var gaussians = new Mat<double>.RowList[] { cls1, cls2, cls3 }.Select(xs => xs.ToDiagonalGaussian(1.0E-6)).ToArray();

            var gmm = xs.ToDiagonalGmm(3);

            var result = new Mat<int>(3, 3);
            foreach (var (x, y) in xs.Zip(ys))
            {
                var actual = gmm.Predict(x);
                result[y, actual]++;
            }
            foreach (var row in result.Rows)
            {
                Assert.IsTrue(row.Max() > 0);
                Assert.IsTrue(row.Count(x => x == 0) == 2);
            }

            var permutation = new int[3];
            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (result[row, col] > 0)
                    {
                        permutation[row] = col;
                    }
                }
            }

            for (var i = 0; i < 3; i++)
            {
                var expected = gaussians[i];
                var actual = gmm.Components[permutation[i]];
                Assert.That(actual.Weight, Is.EqualTo((double)ys.Count(y => y == i) / ys.Length).Within(1.0E-6));
                NumAssert.AreSame(expected.Mean, actual.Gaussian.Mean, 1.0E-6);
                NumAssert.AreSame(expected.Variance, actual.Gaussian.Variance, 1.0E-6);
            }

            foreach (var (x, y) in xs.Zip(ys))
            {
                var expected = new Vec<double>(3);
                expected[permutation[y]] = 1;
                var actual = gmm.PredictProbability(x);
                NumAssert.AreSame(expected, actual, 1.0E-6);
            }
        }

        [Test]
        public void Iris()
        {
            var xs = ReadIris("iris.csv").ToArray();
            var gmm = xs.ToDiagonalGmm(3, 1.0E-6, 3, new Random(42));
            Assert.That(xs.Select(x => gmm.LogPdf(x)).Average(), Is.EqualTo(-2.0478794780624234).Within(1.0E-3));
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
