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
    public class LogisticRegressionTests
    {
        [Test]
        public void Iris1_MatchesLda()
        {
            var dataset = ReadIris1("iris.csv").ToArray();
            var xs = dataset.Select(tpl => tpl.Item1).ToArray();
            var ys = dataset.Select(tpl => tpl.Item2).ToArray();

            var lda = xs.Lda(ys);
            var ldaVector = lda.EigenVectors.Cols[0].Normalize();

            var lr = xs.LogisticRegression(ys);
            var lrVector = lr.Coefficients.Normalize();
            if (Math.Sign(ldaVector[0]) != Math.Sign(lrVector[0]))
            {
                lrVector *= -1;
            }

            NumAssert.AreSame(ldaVector, lrVector, 0.12);
            NumAssert.AreSame(ldaVector.Map(x => (double)Math.Sign(x)), lrVector.Map(x => (double)Math.Sign(x)), 0.0);
        }

        [Test]
        public void Iris2_PerfectSeparation()
        {
            var dataset = ReadIris2("iris.csv").ToArray();
            var xs = dataset.Select(tpl => tpl.Item1).ToArray();
            var ys = dataset.Select(tpl => tpl.Item2).ToArray();

            var lr = xs.LogisticRegression(ys);

            foreach (var (x, y) in xs.Zip(ys))
            {
                Assert.That(lr.Predict(x) == y);

                var prob = lr.PredictProbability(x);
                Assert.That(prob.Sum(), Is.EqualTo(1.0).Within(1.0E-12));
                Assert.That(Math.Round(prob[1]) == y);
            }
        }

        [Test]
        public void Iris3_PerfectSeparation()
        {
            var dataset = ReadIris3("iris.csv").ToArray();
            var xs = dataset.Select(tpl => tpl.Item1).ToArray();
            var ys = dataset.Select(tpl => tpl.Item2).ToArray();

            var lr = xs.LogisticRegression(ys);

            foreach (var (x, y) in xs.Zip(ys))
            {
                Assert.That(lr.Predict(x) == y);

                var prob = lr.PredictProbability(x);
                Assert.That(prob.Sum(), Is.EqualTo(1.0).Within(1.0E-12));
                Assert.That(Math.Round(prob[1]) == y);
            }
        }

        private static IEnumerable<(Vec<double>, int)> ReadIris1(string filename)
        {
            var path = Path.Combine("dataset", filename);
            foreach (var line in File.ReadLines(path).Skip(1))
            {
                var split = line.Split(',');
                var values = split.Take(4).Select(double.Parse);
                var label = split[4] switch
                {
                    "versicolor" => 0,
                    "virginica" => 1,
                    _ => -1
                };

                if (label != -1)
                {
                    yield return (values.ToVector(), label);
                }
            }
        }

        private static IEnumerable<(Vec<double>, int)> ReadIris2(string filename)
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
                    _ => -1
                };

                if (label != -1)
                {
                    yield return (values.ToVector(), label);
                }
            }
        }

        private static IEnumerable<(Vec<double>, int)> ReadIris3(string filename)
        {
            var path = Path.Combine("dataset", filename);
            foreach (var line in File.ReadLines(path).Skip(1))
            {
                var split = line.Split(',');
                var values = split.Take(4).Select(double.Parse);
                var label = split[4] switch
                {
                    "setosa" => 0,
                    "virginica" => 1,
                    _ => -1
                };

                if (label != -1)
                {
                    yield return (values.ToVector(), label);
                }
            }
        }
    }
}
