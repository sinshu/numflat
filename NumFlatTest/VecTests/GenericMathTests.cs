using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.VecTests
{
    public class GenericMathTests
    {
        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 3, 2, 4)]
        [TestCase(3, 1, 1, 1)]
        [TestCase(3, 3, 2, 4)]
        [TestCase(5, 1, 2, 3)]
        [TestCase(11, 7, 2, 5)]
        public void Add(int count, int xStride, int yStride, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.RandomDouble(57, count, yStride);

            var expected = x.Zip(y, (val1, val2) => val1 + val2).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.Add(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1.5, 1)]
        [TestCase(1, 3, 2.3, 4)]
        [TestCase(3, 1, 1.4, 1)]
        [TestCase(3, 3, 2.7, 4)]
        [TestCase(5, 1, 2.8, 3)]
        [TestCase(11, 7, 2.1, 5)]
        public void AddScalar(int count, int xStride, double y, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            var expected = x.Select(value => value + y).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Add(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 3, 2, 4)]
        [TestCase(3, 1, 1, 1)]
        [TestCase(3, 3, 2, 4)]
        [TestCase(5, 1, 2, 3)]
        [TestCase(11, 7, 2, 5)]
        public void Sub(int count, int xStride, int yStride, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.RandomDouble(57, count, yStride);

            var expected = x.Zip(y, (val1, val2) => val1 - val2).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.Sub(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1.5, 1)]
        [TestCase(1, 3, 2.3, 4)]
        [TestCase(3, 1, 1.4, 1)]
        [TestCase(3, 3, 2.7, 4)]
        [TestCase(5, 1, 2.8, 3)]
        [TestCase(11, 7, 2.1, 5)]
        public void SubScalar(int count, int xStride, double y, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            var expected = x.Select(value => value - y).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Sub(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1.5, 1)]
        [TestCase(1, 3, 2.3, 4)]
        [TestCase(3, 1, 0.1, 1)]
        [TestCase(3, 3, 0.7, 4)]
        [TestCase(5, 1, 3.5, 3)]
        [TestCase(11, 7, 7.9, 5)]
        public void Mul(int count, int xStride, double y, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            var expected = x.Select(value => value * y).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Mul(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1.5, 1)]
        [TestCase(1, 3, 2.3, 4)]
        [TestCase(3, 1, 0.1, 1)]
        [TestCase(3, 3, 0.7, 4)]
        [TestCase(5, 1, 3.5, 3)]
        [TestCase(11, 7, 7.9, 5)]
        public void Div(int count, int xStride, double y, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            var expected = x.Select(value => value / y).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Div(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 3, 2, 4)]
        [TestCase(3, 1, 1, 1)]
        [TestCase(3, 3, 2, 4)]
        [TestCase(5, 1, 2, 3)]
        [TestCase(11, 7, 2, 5)]
        public void PointwiseMul(int count, int xStride, int yStride, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.RandomDouble(57, count, yStride);

            var expected = x.Zip(y, (val1, val2) => val1 * val2).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.PointwiseMul(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 3, 2, 4)]
        [TestCase(3, 1, 1, 1)]
        [TestCase(3, 3, 2, 4)]
        [TestCase(5, 1, 2, 3)]
        [TestCase(11, 7, 2, 5)]
        public void PointwiseDiv(int count, int xStride, int yStride, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.NonZeroRandomDouble(57, count, yStride);

            var expected = x.Zip(y, (val1, val2) => val1 / val2).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.PointwiseDiv(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 4)]
        [TestCase(2, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 4, 2)]
        [TestCase(4, 1, 1)]
        [TestCase(4, 2, 2)]
        [TestCase(5, 1, 1)]
        [TestCase(5, 3, 3)]
        public void Reverse(int count, int xStride, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            var expected = x.ToArray().Reverse().ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Reverse(x, actual);
            }

            NumAssert.AreSame(expected, actual, 0);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 4)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 4)]
        [TestCase(5, 1, 3)]
        [TestCase(11, 7, 5)]
        public void Map(int count, int xStride, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            var expected = x.Select(value => new Complex(0, -value)).ToVector();

            var actual = TestVector.RandomComplex(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Map(x, value => new Complex(0, -value), actual);
            }

            NumAssert.AreSame(expected, actual, 0);

            TestVector.FailIfOutOfRangeWrite(actual);
        }
    }
}
