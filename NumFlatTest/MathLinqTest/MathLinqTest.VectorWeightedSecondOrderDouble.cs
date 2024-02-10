using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.Statistics;
using NUnit.Framework;
using NumFlat;

using MVec = MathNet.Numerics.LinearAlgebra.Vector<double>;
using MMat = MathNet.Numerics.LinearAlgebra.Matrix<double>;

namespace NumFlatTest
{
    public class MathLinqTest_VectorWeightedSecondOrderDouble
    {
        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(1, 3, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 1, 4)]
        [TestCase(3, 10, 1)]
        [TestCase(3, 10, 2)]
        [TestCase(5, 20, 3)]
        public void Mean(int dim, int count, int dstStride)
        {
            var data = CreateData(42, dim, count);
            var weights = CreateWeights(57, count);
            var expected = MathNetMean(data, weights);

            var actual = TestVector.RandomDouble(0, dim, dstStride);
            MathLinq.Mean(data.Select(x => x.ToVector()), weights, actual);

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(1, 3, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 1, 4)]
        [TestCase(3, 10, 1)]
        [TestCase(3, 10, 2)]
        [TestCase(5, 20, 3)]
        public void Mean_SameWeights(int dim, int count, int dstStride)
        {
            var data = CreateData(42, dim, count);
            var weights = Enumerable.Repeat(100.0, count);
            var expected = data.Select(x => x.ToVector()).Mean();

            var actual = TestVector.RandomDouble(0, dim, dstStride);
            MathLinq.Mean(data.Select(x => x.ToVector()), weights, actual);

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(3, 1)]
        [TestCase(3, 10)]
        [TestCase(5, 20)]
        public void Mean_ExtensionMethod(int dim, int count)
        {
            var data = CreateData(42, dim, count);
            var weights = CreateWeights(57, count);
            var expected = MathNetMean(data, weights);
            var actual = data.Select(x => x.ToVector()).Mean(weights);
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(3, 1)]
        [TestCase(3, 10)]
        [TestCase(5, 20)]
        public void Mean_ExtensionMethod_SameWeights(int dim, int count)
        {
            var data = CreateData(42, dim, count);
            var weights = Enumerable.Repeat(100.0, count);
            var expected = data.Select(x => x.ToVector()).Mean();
            var actual = data.Select(x => x.ToVector()).Mean(weights);
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 0, 1, 1)]
        [TestCase(1, 1, 0, 3, 2)]
        [TestCase(1, 2, 0, 1, 1)]
        [TestCase(1, 2, 0, 2, 3)]
        [TestCase(3, 1, 0, 1, 3)]
        [TestCase(3, 1, 0, 2, 4)]
        [TestCase(3, 10, 1, 1, 1)]
        [TestCase(3, 10, 1, 4, 3)]
        [TestCase(5, 20, 0, 1, 1)]
        [TestCase(5, 20, 1, 5, 6)]
        public void Variance(int dim, int count, int ddof, int meanStride, int varStride)
        {
            var data = CreateData(42, dim, count);
            var weights = CreateWeights(57, count);
            var expected = MathNetVar(data, weights, ddof);

            var mean = TestVector.RandomDouble(0, dim, meanStride);
            MathLinq.Mean(data.Select(x => x.ToVector()), weights, mean);

            var actual = TestVector.RandomDouble(0, dim, varStride);
            using (mean.EnsureUnchanged())
            {
                MathLinq.Variance(data.Select(x => x.ToVector()), weights, mean, actual, ddof);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(mean);
            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 0, 1, 1)]
        [TestCase(1, 1, 0, 3, 2)]
        [TestCase(1, 2, 0, 1, 1)]
        [TestCase(1, 2, 0, 2, 3)]
        [TestCase(3, 1, 0, 1, 3)]
        [TestCase(3, 1, 0, 2, 4)]
        [TestCase(3, 10, 1, 1, 1)]
        [TestCase(3, 10, 1, 4, 3)]
        [TestCase(5, 20, 0, 1, 1)]
        [TestCase(5, 20, 1, 5, 6)]
        public void Variance_SameWeights(int dim, int count, int ddof, int meanStride, int varStride)
        {
            var data = CreateData(42, dim, count);
            var weights = Enumerable.Repeat(100.0, count);
            var expected = MathNetVar(data, weights, ddof);

            var mean = TestVector.RandomDouble(0, dim, meanStride);
            MathLinq.Mean(data.Select(x => x.ToVector()), weights, mean);

            var actual = TestVector.RandomDouble(0, dim, varStride);
            using (mean.EnsureUnchanged())
            {
                MathLinq.Variance(data.Select(x => x.ToVector()), weights, mean, actual, ddof);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(mean);
            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 0)]
        [TestCase(1, 3, 0)]
        [TestCase(1, 3, 1)]
        [TestCase(3, 1, 0)]
        [TestCase(3, 10, 1)]
        [TestCase(5, 20, 0)]
        public void MeanAndVariance(int dim, int count, int ddof)
        {
            var data = CreateData(42, dim, count);
            var weights = CreateWeights(57, count);
            var expectedMean = MathNetMean(data, weights);
            var expectedVar = MathNetVar(data, weights, ddof);

            var result = data.Select(x => x.ToVector()).MeanAndVariance(weights, ddof);
            NumAssert.AreSame(expectedMean, result.Mean, 1.0E-12);
            NumAssert.AreSame(expectedVar, result.Variance, 1.0E-12);
        }

        [TestCase(1, 1, 0, 1, 1)]
        [TestCase(1, 1, 0, 3, 3)]
        [TestCase(1, 3, 0, 1, 1)]
        [TestCase(1, 3, 0, 3, 3)]
        [TestCase(1, 3, 1, 1, 1)]
        [TestCase(1, 3, 1, 3, 3)]
        [TestCase(3, 1, 0, 1, 3)]
        [TestCase(3, 1, 0, 2, 4)]
        [TestCase(3, 10, 1, 1, 3)]
        [TestCase(3, 10, 1, 4, 7)]
        [TestCase(5, 20, 2, 1, 5)]
        [TestCase(5, 20, 2, 5, 6)]
        public void Covariance(int dim, int count, int ddof, int meanStride, int covStride)
        {
            var data = CreateData(42, dim, count);
            var weights = CreateWeights(57, count);
            var expected = MathNetCov(data, weights, ddof);

            var mean = TestVector.RandomDouble(0, dim, meanStride);
            MathLinq.Mean(data.Select(x => x.ToVector()), weights, mean);

            var actual = TestMatrix.RandomDouble(0, dim, dim, covStride);
            using (mean.EnsureUnchanged())
            {
                MathLinq.Covariance(data.Select(x => x.ToVector()), weights, mean, actual, ddof);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(mean);
            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 0, 1, 1)]
        [TestCase(1, 1, 0, 3, 3)]
        [TestCase(1, 3, 0, 1, 1)]
        [TestCase(1, 3, 0, 3, 3)]
        [TestCase(1, 3, 1, 1, 1)]
        [TestCase(1, 3, 1, 3, 3)]
        [TestCase(3, 1, 0, 1, 3)]
        [TestCase(3, 1, 0, 2, 4)]
        [TestCase(3, 10, 1, 1, 3)]
        [TestCase(3, 10, 1, 4, 7)]
        [TestCase(5, 20, 2, 1, 5)]
        [TestCase(5, 20, 2, 5, 6)]
        public void Covariance_SameWeights(int dim, int count, int ddof, int meanStride, int covStride)
        {
            var data = CreateData(42, dim, count);
            var weights = Enumerable.Repeat(100.0, count);
            var expected = data.Select(x => x.ToVector()).Covariance(ddof);

            var mean = TestVector.RandomDouble(0, dim, meanStride);
            MathLinq.Mean(data.Select(x => x.ToVector()), weights, mean);

            var actual = TestMatrix.RandomDouble(0, dim, dim, covStride);
            using (mean.EnsureUnchanged())
            {
                MathLinq.Covariance(data.Select(x => x.ToVector()), weights, mean, actual, ddof);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(mean);
            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 0)]
        [TestCase(1, 2, 1)]
        [TestCase(3, 1, 0)]
        [TestCase(3, 10, 1)]
        [TestCase(5, 20, 2)]
        public void MeanAndCovariance(int dim, int count, int ddof)
        {
            var data = CreateData(42, dim, count);
            var weights = CreateWeights(57, count);
            var expectedMean = MathNetMean(data, weights);
            var expectedCov = MathNetCov(data, weights, ddof);

            var result = data.Select(x => x.ToVector()).MeanAndCovariance(weights, ddof);
            NumAssert.AreSame(expectedMean, result.Mean, 1.0E-12);
            NumAssert.AreSame(expectedCov, result.Covariance, 1.0E-12);
        }

        [TestCase(1, 1, 0)]
        [TestCase(1, 3, 0)]
        [TestCase(1, 3, 1)]
        [TestCase(3, 1, 0)]
        [TestCase(3, 10, 1)]
        [TestCase(5, 20, 0)]
        public void MeanAndStandardDeviation(int dim, int count, int ddof)
        {
            var data = CreateData(42, dim, count);
            var weights = CreateWeights(57, count);
            var expectedMean = MathNetMean(data, weights);
            var expectedVar = MathNetStd(data, weights, ddof);

            var result = data.Select(x => x.ToVector()).MeanAndStandardDeviation(weights, ddof);
            NumAssert.AreSame(expectedMean, result.Mean, 1.0E-12);
            NumAssert.AreSame(expectedVar, result.StandardDeviation, 1.0E-12);
        }

        [TestCase(1, 1, 0)]
        [TestCase(1, 2, 1)]
        [TestCase(3, 1, 0)]
        [TestCase(3, 10, 1)]
        [TestCase(5, 20, 0)]
        public void Variance_ExtensionMethod(int dim, int count, int ddof)
        {
            var data = CreateData(42, dim, count);
            var weights = CreateWeights(57, count);
            var expected = MathNetVar(data, weights, ddof);
            var actual = data.Select(x => x.ToVector()).Variance(weights, ddof);
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 0)]
        [TestCase(1, 2, 1)]
        [TestCase(3, 1, 0)]
        [TestCase(3, 10, 1)]
        [TestCase(5, 20, 2)]
        public void Covariance_ExtensionMethod(int dim, int count, int ddof)
        {
            var data = CreateData(42, dim, count);
            var weights = CreateWeights(57, count);
            var expected = MathNetCov(data, weights, ddof);
            var actual = data.Select(x => x.ToVector()).Covariance(weights, ddof);
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 0)]
        [TestCase(1, 2, 1)]
        [TestCase(3, 1, 0)]
        [TestCase(3, 10, 1)]
        [TestCase(5, 20, 0)]
        public void StandardDeviation_ExtensionMethod(int dim, int count, int ddof)
        {
            var data = CreateData(42, dim, count);
            var weights = CreateWeights(57, count);
            var expected = MathNetStd(data, weights, ddof);
            var actual = data.Select(x => x.ToVector()).StandardDeviation(weights, ddof);
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        private static MVec MathNetMean(IEnumerable<MVec> xs, IEnumerable<double> weights)
        {
            MVec sum = new MathNet.Numerics.LinearAlgebra.Double.DenseVector(xs.First().Count);
            var count = 0.0;
            foreach (var (x, w) in xs.Zip(weights))
            {
                sum += w * x;
                count += w;
            }
            return sum / count;
        }

        private static MVec MathNetVar(IEnumerable<MVec> xs, IEnumerable<double> weights, int ddof)
        {
            var mean = MathNetMean(xs, weights);

            MVec sum = new MathNet.Numerics.LinearAlgebra.Double.DenseVector(xs.First().Count);
            var w1Sum = 0.0;
            var w2Sum = 0.0;
            foreach (var (x, w) in xs.Zip(weights))
            {
                var d = x - mean;
                sum += w * d.PointwiseMultiply(d);
                w1Sum += w;
                w2Sum += w * w;
            }
            var den = w1Sum - ddof * (w2Sum / w1Sum);
            return sum / den;
        }

        private static MMat MathNetCov(IEnumerable<MVec> xs, IEnumerable<double> weights, int ddof)
        {
            var mean = MathNetMean(xs, weights);

            MMat sum = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(xs.First().Count);
            var w1Sum = 0.0;
            var w2Sum = 0.0;
            foreach (var (x, w) in xs.Zip(weights))
            {
                var d = x - mean;
                sum += w * d.OuterProduct(d);
                w1Sum += w;
                w2Sum += w * w;
            }
            var den = w1Sum - ddof * (w2Sum / w1Sum);
            return sum / den;
        }

        private static MVec MathNetStd(IEnumerable<MVec> xs, IEnumerable<double> weights, int ddof)
        {
            return MathNetVar(xs, weights, ddof).PointwiseSqrt();
        }

        private static MVec[] CreateData(int seed, int dim, int count)
        {
            var random = new Random(seed);

            var data = new List<MVec>();
            for (var i = 0; i < count; i++)
            {
                var elements = Enumerable.Range(0, dim).Select(i => random.NextDouble()).ToArray();
                var x = new MathNet.Numerics.LinearAlgebra.Double.DenseVector(elements);
                data.Add(x);
            }
            return data.ToArray();
        }

        private static double[] CreateWeights(int seed, int count)
        {
            var random = new Random(seed);

            var data = new List<double>();
            for (var i = 0; i < count; i++)
            {
                data.Add(1 + random.NextDouble());
            }
            return data.ToArray();
        }
    }
}
