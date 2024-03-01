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
    public class GmmTests
    {
        [Test]
        public void KMeansInitialModel()
        {
            var xs = ReadIris("iris.csv").ToArray();
            var kmeans = xs.ToKMeans(3, 1, new Random(42));

            var groups = Enumerable.Range(0, kmeans.ClassCount).Select(i => xs.Where(x => kmeans.Predict(x) == i).ToArray()).ToArray();
            var expected = groups.Select(group => group.MeanAndCovariance(0)).ToArray();

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
            var likelihood = double.MinValue;
            for (var i = 0; i < 10; i++)
            {
                var (newModel, newLikelihood) = model.Update(xs);

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
            var gaussians = new Mat<double>.RowList[] { cls1, cls2, cls3 }.Select(xs => xs.ToGaussian(1.0E-6)).ToArray();

            var gmm = xs.ToGmm(3);

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
                NumAssert.AreSame(expected.Covariance, actual.Gaussian.Covariance, 1.0E-6);
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
            var weight1 = 0.36539575;
            var mean1 = new double[] { 6.54639415, 2.94946365, 5.48364578, 1.98726565 };
            var cov1 = new double[,]
            {
                { 0.38744093, 0.09223276, 0.30244302, 0.06087397 },
                { 0.09223276, 0.11040914, 0.08385112, 0.05574334 },
                { 0.30244302, 0.08385112, 0.32589574, 0.07276776 },
                { 0.06087397, 0.05574334, 0.07276776, 0.08484505 },
            };

            var weight2 = 0.33333333;
            var mean2 = new double[] { 5.006, 3.428, 1.462, 0.246 };
            var cov2 = new double[,]
            {
                { 0.121765, 0.097232, 0.016028, 0.010124 },
                { 0.097232, 0.140817, 0.011464, 0.009112 },
                { 0.016028, 0.011464, 0.029557, 0.005948 },
                { 0.010124, 0.009112, 0.005948, 0.010885 },
            };

            var weight3 = 0.30127092;
            var mean3 = new double[] { 5.9170732, 2.77804839, 4.20540364, 1.29848217 };
            var cov3 = new double[,]
            {
                { 0.2755171, 0.09662295, 0.18547072, 0.05478901 },
                { 0.09662295, 0.09255152, 0.09103431, 0.04299899 },
                { 0.18547072, 0.09103431, 0.20235849, 0.06171383 },
                { 0.05478901, 0.04299899, 0.06171383, 0.03233775 },
            };

            var expectedWeights = new double[] { weight1, weight2, weight3 };
            var expectedMeans = new Vec<double>[] { mean1.ToVector(), mean2.ToVector(), mean3.ToVector() };
            var expectedCovs = new Mat<double>[] { cov1.ToMatrix(), cov2.ToMatrix(), cov3.ToMatrix() };

            var xs = ReadIris("iris.csv").ToArray();
            var gmm = xs.ToGmm(3, 1.0E-6, 3, new Random(42));
            var actual = gmm.Components.OrderByDescending(c => c.Weight).ToArray();

            for (var i = 0; i < 3; i++)
            {
                Assert.That(actual[i].Weight, Is.EqualTo(expectedWeights[i]).Within(1.0E-6));
                NumAssert.AreSame(expectedMeans[i], actual[i].Gaussian.Mean, 1.0E-6);
                NumAssert.AreSame(expectedCovs[i], actual[i].Gaussian.Covariance, 1.0E-6);
            }

            Assert.That(xs.Select(x => gmm.LogPdf(x)).Average(), Is.EqualTo(-1.201311085098418).Within(1.0E-6));
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
