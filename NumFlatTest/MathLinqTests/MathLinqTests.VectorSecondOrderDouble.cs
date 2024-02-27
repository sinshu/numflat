using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.Statistics;
using NUnit.Framework;
using NumFlat;

using MVec = MathNet.Numerics.LinearAlgebra.Vector<double>;
using MMat = MathNet.Numerics.LinearAlgebra.Matrix<double>;

namespace NumFlatTest.MathLinqTests
{
    public class VectorSecondOrderDouble
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
            var expected = MathNetMean(data);

            var actual = TestVector.RandomDouble(0, dim, dstStride);
            MathLinq.Mean(data.Select(x => x.ToVector()), actual);

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
            var expected = MathNetMean(data);
            var actual = data.Select(x => x.ToVector()).Mean();
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
            var expected = MathNetVar(data, ddof);

            var mean = TestVector.RandomDouble(0, dim, meanStride);
            MathLinq.Mean(data.Select(x => x.ToVector()), mean);

            var actual = TestVector.RandomDouble(0, dim, varStride);
            using (mean.EnsureUnchanged())
            {
                MathLinq.Variance(data.Select(x => x.ToVector()), mean, actual, ddof);
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
            var expectedMean = MathNetMean(data);
            var expectedVar = MathNetVar(data, ddof);

            var result = data.Select(x => x.ToVector()).MeanAndVariance(ddof);
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
            var expected = MathNetCov(data, ddof);

            var mean = TestVector.RandomDouble(0, dim, meanStride);
            MathLinq.Mean(data.Select(x => x.ToVector()), mean);

            var actual = TestMatrix.RandomDouble(0, dim, dim, covStride);
            using (mean.EnsureUnchanged())
            {
                MathLinq.Covariance(data.Select(x => x.ToVector()), mean, actual, ddof);
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
            var expectedMean = MathNetMean(data);
            var expectedCov = MathNetCov(data, ddof);

            var result = data.Select(x => x.ToVector()).MeanAndCovariance(ddof);
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
            var expectedMean = MathNetMean(data);
            var expectedVar = MathNetStd(data, ddof);

            var result = data.Select(x => x.ToVector()).MeanAndStandardDeviation(ddof);
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
            var expected = MathNetVar(data, ddof);
            var actual = data.Select(x => x.ToVector()).Variance(ddof);
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
            var expected = MathNetCov(data, ddof);
            var actual = data.Select(x => x.ToVector()).Covariance(ddof);
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
            var expected = MathNetStd(data, ddof);
            var actual = data.Select(x => x.ToVector()).StandardDeviation(ddof);
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        private static MVec MathNetMean(IEnumerable<MVec> xs)
        {
            MVec sum = new MathNet.Numerics.LinearAlgebra.Double.DenseVector(xs.First().Count);
            var count = 0;
            foreach (var x in xs)
            {
                sum += x;
                count++;
            }
            return sum / count;
        }

        private static MVec MathNetVar(IEnumerable<MVec> xs, int ddof)
        {
            var mean = MathNetMean(xs);
            if (ddof == 1)
            {
                var variance = Enumerable.Range(0, mean.Count).Select(i => xs.Select(x => x[i]).Variance());
                return MathNet.Numerics.LinearAlgebra.Double.DenseVector.OfEnumerable(variance);
            }
            else if (ddof == 0)
            {
                var variance = Enumerable.Range(0, mean.Count).Select(i => xs.Select(x => x[i]).PopulationVariance());
                return MathNet.Numerics.LinearAlgebra.Double.DenseVector.OfEnumerable(variance);
            }
            else
            {
                throw new Exception();
            }
        }

        private static MMat MathNetCov(IEnumerable<MVec> xs, int ddof)
        {
            var mean = MathNetMean(xs);

            MMat sum = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(xs.First().Count);
            var count = 0;
            foreach (var x in xs)
            {
                var d = x - mean;
                sum += d.OuterProduct(d);
                count++;
            }
            return sum / (count - ddof);
        }

        private static MVec MathNetStd(IEnumerable<MVec> xs, int ddof)
        {
            var mean = MathNetMean(xs);
            if (ddof == 1)
            {
                var variance = Enumerable.Range(0, mean.Count).Select(i => xs.Select(x => x[i]).StandardDeviation());
                return MathNet.Numerics.LinearAlgebra.Double.DenseVector.OfEnumerable(variance);
            }
            else if (ddof == 0)
            {
                var variance = Enumerable.Range(0, mean.Count).Select(i => xs.Select(x => x[i]).PopulationStandardDeviation());
                return MathNet.Numerics.LinearAlgebra.Double.DenseVector.OfEnumerable(variance);
            }
            else
            {
                throw new Exception();
            }
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
    }
}
