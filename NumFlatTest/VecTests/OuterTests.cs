using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.VecTests
{
    public class OuterTests
    {
        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 2, 1, 3, 4)]
        [TestCase(2, 1, 2, 1, 2)]
        [TestCase(2, 3, 2, 4, 5)]
        [TestCase(3, 1, 3, 1, 3)]
        [TestCase(3, 3, 3, 4, 5)]
        [TestCase(2, 1, 3, 1, 4)]
        [TestCase(2, 3, 3, 4, 5)]
        [TestCase(3, 1, 2, 1, 3)]
        [TestCase(3, 3, 2, 4, 5)]
        public void Single(int xCount, int xStride, int yCount, int yStride, int dstStride)
        {
            var x = TestVector.RandomSingle(42, xCount, xStride);
            var y = TestVector.RandomSingle(57, yCount, yStride);

            var mx = Interop.ToMathNet(x);
            var my = Interop.ToMathNet(y);
            var expected = mx.OuterProduct(my);

            var actual = TestMatrix.RandomSingle(0, x.Count, y.Count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.Outer(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-6F);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 2, 1, 3, 4)]
        [TestCase(2, 1, 2, 1, 2)]
        [TestCase(2, 3, 2, 4, 5)]
        [TestCase(3, 1, 3, 1, 3)]
        [TestCase(3, 3, 3, 4, 5)]
        [TestCase(2, 1, 3, 1, 4)]
        [TestCase(2, 3, 3, 4, 5)]
        [TestCase(3, 1, 2, 1, 3)]
        [TestCase(3, 3, 2, 4, 5)]
        public void Double(int xCount, int xStride, int yCount, int yStride, int dstStride)
        {
            var x = TestVector.RandomDouble(42, xCount, xStride);
            var y = TestVector.RandomDouble(57, yCount, yStride);

            var mx = Interop.ToMathNet(x);
            var my = Interop.ToMathNet(y);
            var expected = mx.OuterProduct(my);

            var actual = TestMatrix.RandomDouble(0, x.Count, y.Count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.Outer(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 2, 1, 3, 4)]
        [TestCase(2, 1, 2, 1, 2)]
        [TestCase(2, 3, 2, 4, 5)]
        [TestCase(3, 1, 3, 1, 3)]
        [TestCase(3, 3, 3, 4, 5)]
        [TestCase(2, 1, 3, 1, 4)]
        [TestCase(2, 3, 3, 4, 5)]
        [TestCase(3, 1, 2, 1, 3)]
        [TestCase(3, 3, 2, 4, 5)]
        public void Complex(int xCount, int xStride, int yCount, int yStride, int dstStride)
        {
            var x = TestVector.RandomComplex(42, xCount, xStride);
            var y = TestVector.RandomComplex(57, yCount, yStride);

            var mx = Interop.ToMathNet(x);
            var my = Interop.ToMathNet(y);
            var expected = mx.OuterProduct(my);

            var actual = TestMatrix.RandomComplex(0, x.Count, y.Count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.Outer(x, y, actual, false);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 2, 1, 3, 4)]
        [TestCase(2, 1, 2, 1, 2)]
        [TestCase(2, 3, 2, 4, 5)]
        [TestCase(3, 1, 3, 1, 3)]
        [TestCase(3, 3, 3, 4, 5)]
        [TestCase(2, 1, 3, 1, 4)]
        [TestCase(2, 3, 3, 4, 5)]
        [TestCase(3, 1, 2, 1, 3)]
        [TestCase(3, 3, 2, 4, 5)]
        public void ComplexConj(int xCount, int xStride, int yCount, int yStride, int dstStride)
        {
            var x = TestVector.RandomComplex(42, xCount, xStride);
            var y = TestVector.RandomComplex(57, yCount, yStride);

            var mx = Interop.ToMathNet(x);
            var my = Interop.ToMathNet(y);
            var expected = mx.OuterProduct(my.Conjugate());

            var actual = TestMatrix.RandomComplex(0, x.Count, y.Count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.Outer(x, y, actual, true);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }
    }
}
