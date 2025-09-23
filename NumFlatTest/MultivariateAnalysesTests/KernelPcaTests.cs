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
    public class KernelPcaTests
    {
        [Test]
        public void Iris_Reference()
        {
            var dataset = ReadIris("iris.csv").ToArray();
            var reference = ReadIris("iris_kpca_rbf_gamma0p1.csv").ToArray();

            var kpca = new KernelPrincipalComponentAnalysis(dataset, Kernel.Gaussian(0.1));

            var transformed = dataset.Select(x =>
            {
                using (x.EnsureUnchanged())
                {
                    return kpca.Transform(x)[0..2];
                }
            }).ToArray();

            var signs = GetSigns(transformed[0], reference[0]);
            transformed = transformed.Select(x => x.PointwiseMul(signs)).ToArray();

            foreach (var (expected, actual) in reference.Zip(transformed))
            {
                NumAssert.AreSame(expected, actual, 1.0E-6);
            }
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
