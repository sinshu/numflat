using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.MultivariateAnalyses;

namespace NumFlatTest
{
    public class LdaTests
    {
        [Test]
        public void Iris_Reference()
        {
            var dataset = ReadIris("iris.csv").ToArray();
            var reference = ReadGpt4Reference("iris_lda_gpt4.csv").ToArray();

            var xs = dataset.Select(tpl => tpl.Item1).ToArray();
            var ys = dataset.Select(tpl => tpl.Item2).ToArray();
            var lda = xs.Lda(ys);

            var transformed = xs.Select(x =>
            {
                using (x.EnsureUnchanged())
                {
                    return lda.Transform(x).Take(2).ToVector();
                }
            }).ToArray();

            var signs = GetSigns(transformed[0], reference[0]);
            transformed = transformed.Select(x => x.PointwiseMul(signs)).ToArray();

            foreach (var (expected, actual) in reference.Zip(transformed))
            {
                NumAssert.AreSame(expected, actual, 1.0E-6);
            }
        }

        private static IEnumerable<(Vec<double>, int)> ReadIris(string filename)
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
                yield return (values.ToVector(), label);
            }
        }

        private static IEnumerable<Vec<double>> ReadGpt4Reference(string filename)
        {
            var path = Path.Combine("dataset", filename);
            foreach (var line in File.ReadLines(path).Skip(1))
            {
                var values = line.Split(',').Take(2).Select(double.Parse);
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
