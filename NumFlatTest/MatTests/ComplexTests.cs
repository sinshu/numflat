using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.MatTests
{
    public class ComplexTests
    {
        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 5)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 4, 3)]
        [TestCase(3, 2, 3, 3)]
        [TestCase(3, 2, 4, 5)]
        [TestCase(2, 3, 2, 3)]
        [TestCase(2, 3, 4, 4)]
        [TestCase(6, 3, 8, 7)]
        [TestCase(4, 7, 5, 9)]
        public void Conjugate(int rowCount, int colCount, int xStride, int dstStride)
        {
            var x = TestMatrix.RandomComplex(42, rowCount, colCount, xStride);
            var dst = TestMatrix.RandomComplex(0, rowCount, colCount, dstStride);

            using (x.EnsureUnchanged())
            {
                Mat.Conjugate(x, dst);
            }

            var expected = x.Map(value => value.Conjugate());

            NumAssert.AreSame(expected, dst, 0);

            TestMatrix.FailIfOutOfRangeWrite(dst);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 5)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 4, 3)]
        [TestCase(3, 2, 3, 3)]
        [TestCase(3, 2, 4, 5)]
        [TestCase(2, 3, 2, 3)]
        [TestCase(2, 3, 4, 4)]
        [TestCase(6, 3, 8, 7)]
        [TestCase(4, 7, 5, 9)]
        public void Real(int rowCount, int colCount, int xStride, int dstStride)
        {
            var x = TestMatrix.RandomComplex(42, rowCount, colCount, xStride);
            var dst = TestMatrix.RandomDouble(0, rowCount, colCount, dstStride);

            using (x.EnsureUnchanged())
            {
                Mat.Real(x, dst);
            }

            var expected = x.Map(value => value.Real);

            NumAssert.AreSame(expected, dst, 0);

            TestMatrix.FailIfOutOfRangeWrite(dst);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 5)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 4, 3)]
        [TestCase(3, 2, 3, 3)]
        [TestCase(3, 2, 4, 5)]
        [TestCase(2, 3, 2, 3)]
        [TestCase(2, 3, 4, 4)]
        [TestCase(6, 3, 8, 7)]
        [TestCase(4, 7, 5, 9)]
        public void Imaginary(int rowCount, int colCount, int xStride, int dstStride)
        {
            var x = TestMatrix.RandomComplex(42, rowCount, colCount, xStride);
            var dst = TestMatrix.RandomDouble(0, rowCount, colCount, dstStride);

            using (x.EnsureUnchanged())
            {
                Mat.Imaginary(x, dst);
            }

            var expected = x.Map(value => value.Imaginary);

            NumAssert.AreSame(expected, dst, 0);

            TestMatrix.FailIfOutOfRangeWrite(dst);
        }
    }
}
