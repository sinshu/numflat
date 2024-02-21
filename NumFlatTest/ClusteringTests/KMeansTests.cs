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

        [Test]
        public void SimpleData()
        {
            var xs = new double[,]
            {
                {  1,  0,  1 },
                {  1,  1,  1 },
                {  5, -5, -5 },
                {  6, -4, -6 },
                {  6, -5, -4 },
                { -8,  2,  7 },
                { -9,  1,  8 },
                { -8,  3,  8 },
                { -8,  1,  9 },
            }
            .ToMatrix().Rows;

            var kmeans = new KMeans(xs, 3, new Random(42));
            var cls = xs.Select(x => kmeans.Predict(x)).ToArray();
            Assert.IsTrue(cls[0] == cls[1]);
            Assert.IsTrue(cls[2] == cls[3]);
            Assert.IsTrue(cls[2] == cls[4]);
            Assert.IsTrue(cls[5] == cls[6]);
            Assert.IsTrue(cls[5] == cls[7]);
            Assert.IsTrue(cls[5] == cls[8]);
        }

        [Test]
        public void Iris()
        {
            var xs = ReadIris("iris.csv").ToArray();
            var ys = ReadIrisReference("iris_kmeans_gpt4.csv");

            var kmeans = new KMeans(xs, 3, new Random(57));
            var result = new Mat<int>(3, 3);
            foreach (var (x, y) in xs.Zip(ys))
            {
                var actual = kmeans.Predict(x);
                result[y, actual]++;
            }

            foreach (var row in result.Rows)
            {
                Assert.IsTrue(row.Max() > 0);
                Assert.IsTrue(row.Count(x => x == 0) == 2);
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

        private static IEnumerable<int> ReadIrisReference(string filename)
        {
            var path = Path.Combine("dataset", filename);
            foreach (var line in File.ReadLines(path).Skip(1))
            {
                yield return int.Parse(line);
            }
        }
    }
}
