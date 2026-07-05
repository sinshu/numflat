using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NumFlat;
using NumFlat.TimeSeries;

namespace NumFlatTest.TimeSeriesTests
{
    public class DtwBarycenterAveragingTests
    {
        [Test]
        public void FitMatchesTslearnDocumentedExampleWithEqualLengthSeries()
        {
            var sourceSequences = ToSequences(
                [1, 2, 3, 4],
                [1, 2, 4, 5]);

            var actual = DtwBarycenterAveraging.Fit(sourceSequences, new DtwBarycenterAveragingOptions { MaxIterations = 5 });

            AssertSequence(actual, [1, 2, 3.5, 4.5]);
        }

        [Test]
        public void FitKeepsInitialGuessLengthForDifferentLengthSeries()
        {
            var sourceSequences = ToSequences(
                [1, 2, 3, 4, 5],
                [1, 2, 3, 4]);

            var actual = DtwBarycenterAveraging.Fit(sourceSequences, new DtwBarycenterAveragingOptions { MaxIterations = 5 });

            AssertSequence(actual, [1, 2, 3, 4, 4.5]);
        }

        [Test]
        public void GetInitialGuessSelectsMedoidWithSmallestTotalDtwDistance()
        {
            var sourceSequences = ToSequences(
                [0, 0, 0],
                [2, 2, 2],
                [100, 100, 100]);

            var actual = DtwBarycenterAveraging.GetInitialGuess(sourceSequences);

            AssertSequence(actual, [2, 2, 2]);
            Assert.That(actual, Is.Not.SameAs(sourceSequences[1]));
        }

        [Test]
        public void UpdateDoesNotIncreaseObjectiveCost()
        {
            var sourceSequences = ToSequences(
                [0, 1, 2, 3, 4],
                [0, 1, 1, 2, 3, 4],
                [0, 0, 1, 2, 3, 4],
                [0, 1, 2, 2, 3, 4]);
            var currentBarycenter = ToSequence([0, 2, 4]);
            var costBefore = AverageDtwCost(currentBarycenter, sourceSequences);

            var (updatedBarycenter, reportedAverageCost) = DtwBarycenterAveraging.Update(currentBarycenter, sourceSequences);
            var costAfter = AverageDtwCost(updatedBarycenter, sourceSequences);

            Assert.That(reportedAverageCost, Is.EqualTo(costBefore).Within(1.0E-12));
            Assert.That(costAfter, Is.LessThan(costBefore));
        }

        [Test]
        public void SingleSourceSequenceIsReturnedUnchangedByFit()
        {
            var sourceSequences = ToSequences([1.5, 2.5, 3.5]);

            var actual = DtwBarycenterAveraging.Fit(sourceSequences);

            AssertSequence(actual, [1.5, 2.5, 3.5]);
        }

        [Test]
        public void FitSupportsMultidimensionalSeries()
        {
            IReadOnlyList<IReadOnlyList<Vec<double>>> sourceSequences =
            [
                new Vec<double>[] { [1, 10], [2, 20], [3, 30] },
                new Vec<double>[] { [1, 10], [2, 20], [4, 40] }
            ];

            var actual = DtwBarycenterAveraging.Fit(sourceSequences, new DtwBarycenterAveragingOptions { MaxIterations = 5 });

            Assert.That(actual.Count, Is.EqualTo(3));
            NumAssert.AreSame(new double[] { 1, 10 }.ToVector(), actual[0], 1.0E-12);
            NumAssert.AreSame(new double[] { 2, 20 }.ToVector(), actual[1], 1.0E-12);
            NumAssert.AreSame(new double[] { 3.5, 35 }.ToVector(), actual[2], 1.0E-12);
        }

        private static IReadOnlyList<IReadOnlyList<Vec<double>>> ToSequences(params double[][] values)
        {
            return values.Select(ToSequence).ToArray();
        }

        private static Vec<double>[] ToSequence(double[] values)
        {
            return values.Select(value => value.AsSingleElementVector()).ToArray();
        }

        private static double AverageDtwCost(IReadOnlyList<Vec<double>> barycenter, IReadOnlyList<IReadOnlyList<Vec<double>>> sourceSequences)
        {
            return sourceSequences.Average(sourceSequence => DynamicTimeWarping.GetDistance(barycenter, sourceSequence, DistanceMetric.EuclideanSquared));
        }

        private static void AssertSequence(IReadOnlyList<Vec<double>> actual, double[] expected)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Length));
            for (var i = 0; i < expected.Length; i++)
            {
                NumAssert.AreSame(expected[i].AsSingleElementVector(), actual[i], 1.0E-12);
            }
        }
    }
}
