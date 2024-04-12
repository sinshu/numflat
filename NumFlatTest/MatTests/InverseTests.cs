using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.MatTests
{
    public class InverseTests
    {
        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(4, 6, 5)]
        public void Single(int n, int xStride, int dstStride)
        {
            var x = TestMatrix.RandomSingle(42, n, n, xStride);

            var inverse = TestMatrix.RandomSingle(0, n, n, dstStride);
            using (x.EnsureUnchanged())
            {
                Mat.Inverse(x, inverse);
            }

            var actual = inverse * x;
            var expected = MatrixBuilder.Identity<float>(n);
            NumAssert.AreSame(expected, actual, 1.0E-6F);

            TestMatrix.FailIfOutOfRangeWrite(inverse);
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(4, 6, 5)]
        public void Double(int n, int xStride, int dstStride)
        {
            var x = TestMatrix.RandomDouble(42, n, n, xStride);

            var inverse = TestMatrix.RandomDouble(0, n, n, dstStride);
            using (x.EnsureUnchanged())
            {
                Mat.Inverse(x, inverse);
            }

            var actual = inverse * x;
            var expected = MatrixBuilder.Identity<double>(n);
            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(inverse);
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(4, 6, 5)]
        public void Complex(int n, int xStride, int dstStride)
        {
            var x = TestMatrix.RandomComplex(42, n, n, xStride);

            var inverse = TestMatrix.RandomComplex(0, n, n, dstStride);
            using (x.EnsureUnchanged())
            {
                Mat.Inverse(x, inverse);
            }

            var actual = inverse * x;
            var expected = MatrixBuilder.Identity<Complex>(n);
            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(inverse);
        }
    }
}
