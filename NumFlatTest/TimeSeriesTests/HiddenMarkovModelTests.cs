using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.TimeSeries;
using NumFlat.Distributions;
using MathNet.Numerics.Distributions;

namespace NumFlatTest.TimeSeriesTests
{
    public class HiddenMarkovModelTests
    {
        [Test]
        public void Decode1()
        {
            Vec<double> mean1 = [0, 0];
            Vec<double> mean2 = [10, 10];
            Vec<double> mean3 = [20, 20];

            Mat<double> cov1 = 9 * MatrixBuilder.Identity<double>(2);
            Mat<double> cov2 = 1 * MatrixBuilder.Identity<double>(2);
            Mat<double> cov3 = 25 * MatrixBuilder.Identity<double>(2);

            var dist1 = new Gaussian(mean1, cov1);
            var dist2 = new Gaussian(mean2, cov2);
            var dist3 = new Gaussian(mean3, cov3);
            IMultivariateDistribution<double>[] dists = [dist1, dist2, dist3];

            Vec<double> initial = [0.3, 0.3, 0.4];

            Mat<double> trans =
            [
                [0.80, 0.10, 0.10],
                [0.20, 0.60, 0.20],
                [0.05, 0.05, 0.90],
            ];

            var hmm = new HiddenMarkovModel(initial, trans, dists);

            int[] states = [0, 0, 0, 1, 1, 1, 2, 2, 2, 1, 1, 0, 0, 2, 2, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 2, 1, 0];
            var random = new Random(42);
            var xs = states.Select(state => dists[state].Generate(random)).ToArray();

            var path = hmm.Decode(xs).Path;

            Assert.That(path, Is.EqualTo(states));
        }

        [Test]
        public void Decode2()
        {
            Vec<double> mean1 = [0, 0];
            Vec<double> mean2 = [10, 10];
            Vec<double> mean3 = [20, 20];

            Mat<double> cov1 = 9 * MatrixBuilder.Identity<double>(2);
            Mat<double> cov2 = 1 * MatrixBuilder.Identity<double>(2);
            Mat<double> cov3 = 25 * MatrixBuilder.Identity<double>(2);

            var dist1 = new Gaussian(mean1, cov1);
            var dist2 = new Gaussian(mean2, cov2);
            var dist3 = new Gaussian(mean3, cov3);
            IMultivariateDistribution<double>[] dists = [dist1, dist2, dist3];

            Vec<double> initial = [0.0, 0.0, 1.0];

            Mat<double> trans =
            [
                [0.80, 0.10, 0.10],
                [0.20, 0.60, 0.20],
                [0.05, 0.05, 0.90],
            ];

            var hmm = new HiddenMarkovModel(initial, trans, dists);

            int[] states = [0, 0, 0, 1, 1, 1, 2, 2, 2, 1, 1, 0, 0, 2, 2, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 2, 1, 0];
            var random = new Random(42);
            var xs = states.Select(state => dists[state].Generate(random)).ToArray();

            int[] expected = [2, 0, 0, 1, 1, 1, 2, 2, 2, 1, 1, 0, 0, 2, 2, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 2, 1, 0];

            var path = hmm.Decode(xs).Path;

            Assert.That(path, Is.EqualTo(expected));
        }

        [Test]
        public void Decode3()
        {
            Vec<double> mean1 = [0, 0];
            Vec<double> mean2 = [10, 10];
            Vec<double> mean3 = [20, 20];

            Mat<double> cov1 = 9 * MatrixBuilder.Identity<double>(2);
            Mat<double> cov2 = 1 * MatrixBuilder.Identity<double>(2);
            Mat<double> cov3 = 25 * MatrixBuilder.Identity<double>(2);

            var dist1 = new Gaussian(mean1, cov1);
            var dist2 = new Gaussian(mean2, cov2);
            var dist3 = new Gaussian(mean3, cov3);
            IMultivariateDistribution<double>[] dists = [dist1, dist2, dist3];

            Vec<double> initial = [0.3, 0.3, 0.4];

            Mat<double> trans =
            [
                [0.80, 0.10, 0.10],
                [0.20, 0.60, 0.20],
                [0.00, 0.00, 1.00],
            ];

            var hmm = new HiddenMarkovModel(initial, trans, dists);

            int[] states = [0, 0, 0, 1, 1, 1, 2, 2, 2, 0, 2];
            var random = new Random(42);
            var xs = states.Select(state => dists[state].Generate(random)).ToArray();

            int[] expected = [0, 0, 0, 1, 1, 1, 2, 2, 2, 2, 2];

            var path = hmm.Decode(xs).Path;

            Assert.That(path, Is.EqualTo(expected));
        }
    }
}
