using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.Clustering;

namespace NumFlatTest
{
    public class KMeansTests
    {
        [TestCase(42)]
        [TestCase(57)]
        [TestCase(66)]
        [TestCase(77)]
        [TestCase(88)]
        public void GetInitialCentroids1(int seed)
        {
            var xs = new double[,]
            {
                { 1,  0 },
                { 2, -3 },
                { 3,  5 },
                { 4, -1 },
                { 5,  0 },
            }
            .ToMatrix().Rows;

            var model = KMeans.GetInitialModel(xs, 5, new Random(seed));

            var i = 0;
            foreach (var centroid in model.Centroids.OrderBy(x => x[0]))
            {
                NumAssert.AreSame(xs[i], centroid, 0.0);
                i++;
            }

            Assert.That(model.GetSumOfSquaredDistances(xs), Is.EqualTo(0).Within(0));
            Assert.That(model.Update(xs).GetSumOfSquaredDistances(xs), Is.EqualTo(0).Within(0));
        }

        [TestCase(42)]
        [TestCase(57)]
        [TestCase(66)]
        [TestCase(77)]
        [TestCase(88)]
        public void GetInitialCentroids2(int seed)
        {
            var expected = new double[,]
            {
                { 1, -3 },
                { 2,  4 },
                { 3, -5 },
                { 4,  2 },
                { 5,  1 },
            }
            .ToMatrix().Rows;

            var xs = new double[,]
            {
                { 1, -3 },
                { 1, -3 },
                { 1, -3 },
                { 1, -3 },
                { 1, -3 },
                { 2,  4 },
                { 3, -5 },
                { 3, -5 },
                { 3, -5 },
                { 3, -5 },
                { 4,  2 },
                { 4,  2 },
                { 4,  2 },
                { 4,  2 },
                { 5,  1 },
                { 5,  1 },
                { 5,  1 },
            }
            .ToMatrix().Rows;

            var model = KMeans.GetInitialModel(xs, 5, new Random(seed));

            var i = 0;
            foreach (var centroid in model.Centroids.OrderBy(x => x[0]))
            {
                NumAssert.AreSame(expected[i], centroid, 0.0);
                i++;
            }

            Assert.That(model.GetSumOfSquaredDistances(xs), Is.EqualTo(0).Within(0));
            Assert.That(model.Update(xs).GetSumOfSquaredDistances(xs), Is.EqualTo(0).Within(0));
        }

        [TestCase(42, 2, 20, 3)]
        [TestCase(57, 3, 30, 4)]
        [TestCase(66, 4, 40, 2)]
        public void SumOfSquaredDistances(int seed, int dimension, int count, int clusters)
        {
            var xs = TestMatrix.RandomDouble(seed, count, dimension, count).Rows;

            var model = KMeans.GetInitialModel(xs, clusters, new Random(seed));
            var error = model.GetSumOfSquaredDistances(xs);
            while (true)
            {
                model = model.Update(xs);
                var newError = model.GetSumOfSquaredDistances(xs);
                Assert.That(newError <= error);
                if (newError == error)
                {
                    break;
                }
                error = newError;
            }
        }
    }
}
