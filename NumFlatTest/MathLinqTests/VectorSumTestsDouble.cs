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
    public class VectorSumTestsDouble
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
        public void Sum(int dim, int count, int dstStride)
        {
            var data = CreateData(42, dim, count);
            var expected = MathNetSum(data);

            var actual = TestVector.RandomDouble(0, dim, dstStride);
            MathLinq.Sum(data.Select(x => x.ToVector()), actual);

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(3, 1)]
        [TestCase(3, 10)]
        [TestCase(5, 20)]
        public void Sum_ExtensionMethod(int dim, int count)
        {
            var data = CreateData(42, dim, count);
            var expected = MathNetSum(data);
            var actual = data.Select(x => x.ToVector()).Sum();
            NumAssert.AreSame(expected, actual, 1.0E-12);
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
        public void WeightedSum(int dim, int count, int dstStride)
        {
            var data = CreateData(42, dim, count);
            var weights = CreateWeights(57, count);
            var expected = MathNetSum(data, weights);

            var actual = TestVector.RandomDouble(0, dim, dstStride);
            MathLinq.Sum(data.Select(x => x.ToVector()), weights, actual);

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(3, 1)]
        [TestCase(3, 10)]
        [TestCase(5, 20)]
        public void WeightedSum_ExtensionMethod(int dim, int count)
        {
            var data = CreateData(42, dim, count);
            var weights = CreateWeights(57, count);
            var expected = MathNetSum(data, weights);
            var actual = data.Select(x => x.ToVector()).Sum(weights);
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        private static MVec MathNetSum(IEnumerable<MVec> xs)
        {
            MVec sum = new MathNet.Numerics.LinearAlgebra.Double.DenseVector(xs.First().Count);
            foreach (var x in xs)
            {
                sum += x;
            }
            return sum;
        }

        private static MVec MathNetSum(IEnumerable<MVec> xs, IEnumerable<double> weights)
        {
            MVec sum = new MathNet.Numerics.LinearAlgebra.Double.DenseVector(xs.First().Count);
            foreach (var (x, w) in xs.Zip(weights))
            {
                sum += w * x;
            }
            return sum;
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
