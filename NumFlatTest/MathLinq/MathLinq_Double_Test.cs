using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

using MVec = MathNet.Numerics.LinearAlgebra.Vector<double>;
using MMat = MathNet.Numerics.LinearAlgebra.Matrix<double>;

namespace NumFlatTest
{
    public class MathLinq_Double_Test
    {
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

        [TestCase(3, 1, 0, 1, 3)]
        [TestCase(3, 1, 0, 2, 4)]
        [TestCase(3, 10, 1, 1, 3)]
        [TestCase(3, 10, 1, 4, 7)]
        [TestCase(5, 20, 2, 1, 5)]
        [TestCase(5, 20, 2, 5, 6)]
        public void Covariance(int dim, int count, int ddot, int meanStride, int covStride)
        {
            var data = CreateData(42, dim, count);
            var expected = MathNetCov(data, ddot);

            var mean = TestVector.RandomDouble(0, dim, meanStride);
            MathLinq.Mean(data.Select(x => x.ToVector()), mean);

            var actual = TestMatrix.RandomDouble(0, dim, dim, covStride);
            using (mean.EnsureUnchanged())
            {
                MathLinq.Covariance(data.Select(x => x.ToVector()), mean, actual, ddot);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(mean);
            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(3, 1, 0)]
        [TestCase(3, 10, 1)]
        [TestCase(5, 20, 2)]
        public void Covariance_ExtensionMethod_OneArg(int dim, int count, int ddot)
        {
            var data = CreateData(42, dim, count);
            var expected = MathNetCov(data, ddot);
            var actual = data.Select(x => x.ToVector()).Covariance(ddot);
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(3, 2)]
        [TestCase(3, 10)]
        [TestCase(5, 20)]
        public void Covariance_ExtensionMethod_NoArg(int dim, int count)
        {
            var data = CreateData(42, dim, count);
            var expected = MathNetCov(data, 1);
            var actual = data.Select(x => x.ToVector()).Covariance();
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(3, 1, 0)]
        [TestCase(3, 10, 1)]
        [TestCase(5, 20, 2)]
        public void MeanAndCovariance_OneArg(int dim, int count, int ddot)
        {
            var data = CreateData(42, dim, count);
            var expectedMean = MathNetMean(data);
            var expectedCov = MathNetCov(data, ddot);

            var result = data.Select(x => x.ToVector()).MeanAndCovariance(ddot);
            NumAssert.AreSame(expectedMean, result.Mean, 1.0E-12);
            NumAssert.AreSame(expectedCov, result.Covariance, 1.0E-12);
        }

        [TestCase(3, 2)]
        [TestCase(3, 10)]
        [TestCase(5, 20)]
        public void MeanAndCovariance_NoArg(int dim, int count)
        {
            var data = CreateData(42, dim, count);
            var expectedMean = MathNetMean(data);
            var expectedCov = MathNetCov(data, 1);

            var result = data.Select(x => x.ToVector()).MeanAndCovariance();
            NumAssert.AreSame(expectedMean, result.Mean, 1.0E-12);
            NumAssert.AreSame(expectedCov, result.Covariance, 1.0E-12);
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

        private static MMat MathNetCov(IEnumerable<MVec> xs, int ddot)
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
            return sum / (count - ddot);
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
