using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.VecTests
{
    public class NormalizeTests
    {
        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 2)]
        [TestCase(2, 1, 1)]
        [TestCase(2, 2, 3)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void Single(int count, int xStride, int dstStride)
        {
            var x = TestVector.RandomSingle(42, count, xStride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Normalize(2);

            var actual = TestVector.RandomSingle(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Noramlize(x, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-6F);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 2)]
        [TestCase(2, 1, 1)]
        [TestCase(2, 2, 3)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void Double(int count, int xStride, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Normalize(2);

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Noramlize(x, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 2)]
        [TestCase(2, 1, 1)]
        [TestCase(2, 2, 3)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void Complex(int count, int xStride, int dstStride)
        {
            var x = TestVector.RandomComplex(42, count, xStride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Normalize(2);

            var actual = TestVector.RandomComplex(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Noramlize(x, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 2)]
        [TestCase(1, 2, 2, 3)]
        [TestCase(2, 1, 1, 4)]
        [TestCase(2, 2, 3, 2)]
        [TestCase(5, 2, 3, 3)]
        [TestCase(7, 3, 4, 4)]
        public void Single(int count, int xStride, int dstStride, float p)
        {
            var x = TestVector.RandomSingle(42, count, xStride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Normalize(p);

            var actual = TestVector.RandomSingle(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Noramlize(x, actual, p);
            }

            NumAssert.AreSame(expected, actual, 1.0E-6F);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 2)]
        [TestCase(1, 2, 2, 3)]
        [TestCase(2, 1, 1, 4)]
        [TestCase(2, 2, 3, 2)]
        [TestCase(5, 2, 3, 3)]
        [TestCase(7, 3, 4, 4)]
        public void Double(int count, int xStride, int dstStride, double p)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Normalize(p);

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Noramlize(x, actual, p);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 2)]
        [TestCase(1, 2, 2, 3)]
        [TestCase(2, 1, 1, 4)]
        [TestCase(2, 2, 3, 2)]
        [TestCase(5, 2, 3, 3)]
        [TestCase(7, 3, 4, 4)]
        public void Complex(int count, int xStride, int dstStride, double p)
        {
            var x = TestVector.RandomComplex(42, count, xStride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Normalize(p);

            var actual = TestVector.RandomComplex(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Noramlize(x, actual, p);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }
    }
}
