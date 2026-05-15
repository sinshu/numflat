using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.MultivariateAnalyses;

namespace NumFlatTest.MultivariateAnalysesTests
{
    public class PcaTests
    {
        [Test]
        public void Iris_Reference()
        {
            var dataset = ReadIris("iris.csv").ToArray();
            var reference = ReadIris("iris_pca_gpt4.csv").ToArray();

            var pca = dataset.Pca();

            var transformed = dataset.Select(x =>
            {
                using (x.EnsureUnchanged())
                {
                    return pca.Transform(x);
                }
            }).ToArray();

            var signs = GetSigns(transformed[0], reference[0]);
            transformed = transformed.Select(x => x.PointwiseMul(signs)).ToArray();

            foreach (var (expected, actual) in reference.Zip(transformed))
            {
                NumAssert.AreSame(expected, actual, 1.0E-6);
            }
        }

        [Test]
        public void Iris_Inverse()
        {
            var dataset = ReadIris("iris.csv").ToArray();

            var pca = dataset.Pca();
            var transformed = dataset.Select(x => pca.Transform(x)).ToArray();

            var inverseTransformed = transformed.Select(x =>
            {
                using (x.EnsureUnchanged())
                {
                    return pca.InverseTransform(x);
                }
            }).ToArray();

            foreach (var (expected, actual) in dataset.Zip(inverseTransformed))
            {
                NumAssert.AreSame(expected, actual, 1.0E-12);
            }
        }

        [Test]
        public void ConstructorFromParametersStoresInputsDirectly()
        {
            var mean = new Vec<double>(new[] { 1.0, 2.0 });
            var eigenValues = new Vec<double>(new[] { 3.0, 4.0 });
            var eigenVectors = new Mat<double>(2, 2, 2, new[]
            {
                1.0, 0.0,
                0.0, 1.0
            });

            var pca = new PrincipalComponentAnalysis(mean, eigenValues, eigenVectors);

            mean[0] = 10.0;
            eigenValues[0] = 30.0;
            eigenVectors[0, 0] = 100.0;

            Assert.That(pca.Mean[0], Is.EqualTo(10.0));
            Assert.That(pca.EigenValues[0], Is.EqualTo(30.0));
            Assert.That(pca.EigenVectors[0, 0], Is.EqualTo(100.0));
        }

        private static IEnumerable<Vec<double>> ReadIris(string filename)
        {
            var path = Path.Combine("dataset", filename);
            foreach (var line in File.ReadLines(path).Skip(1))
            {
                var values = line.Split(',').Take(4).Select(value => double.Parse(value));
                yield return values.ToVector();
            }
        }

        private static Vec<double> GetSigns(Vec<double> actual, Vec<double> expected)
        {
            var signs = new Vec<double>(actual.Count);

            for (var i = 0; i < signs.Count; i++)
            {
                var e1 = Math.Abs(expected[i] - actual[i]);
                var e2 = Math.Abs(expected[i] - -actual[i]);
                signs[i] = e1 < e2 ? 1 : -1;
            }

            return signs;
        }
    }
}
