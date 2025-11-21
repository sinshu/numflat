using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.VecTests
{
    public class ComplexTests
    {
        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(1, 3, 4)]
        [TestCase(2, 5, 4)]
        [TestCase(5, 7, 6)]
        public void Conjugate(int count, int xStride, int dstStride)
        {
            var x = TestVector.RandomComplex(42, count, xStride);

            var expected = x.Select(c => c.Conjugate()).ToVector();

            var actual = TestVector.RandomComplex(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Conjugate(x, actual);
            }

            NumAssert.AreSame(expected, actual, 0);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(1, 3, 4)]
        [TestCase(2, 5, 4)]
        [TestCase(5, 7, 6)]
        public void Real(int count, int xStride, int dstStride)
        {
            var x = TestVector.RandomComplex(42, count, xStride);

            var expected = x.Select(c => c.Real).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Real(x, actual);
            }

            NumAssert.AreSame(expected, actual, 0);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(1, 3, 4)]
        [TestCase(2, 5, 4)]
        [TestCase(5, 7, 6)]
        public void Imaginary(int count, int xStride, int dstStride)
        {
            var x = TestVector.RandomComplex(42, count, xStride);

            var expected = x.Select(c => c.Imaginary).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Imaginary(x, actual);
            }

            NumAssert.AreSame(expected, actual, 0);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(1, 3, 4)]
        [TestCase(2, 5, 4)]
        [TestCase(5, 7, 6)]
        public void Magnitude(int count, int xStride, int dstStride)
        {
            var x = TestVector.RandomComplex(42, count, xStride);

            var expected = x.Select(c => c.Magnitude).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Magnitude(x, actual);
            }

            NumAssert.AreSame(expected, actual, 0);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(1, 3, 4)]
        [TestCase(2, 5, 4)]
        [TestCase(5, 7, 6)]
        public void MagnitudeSquared(int count, int xStride, int dstStride)
        {
            var x = TestVector.RandomComplex(42, count, xStride);

            var expected = x.Select(c => c.Real * c.Real + c.Imaginary * c.Imaginary).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.MagnitudeSquared(x, actual);
            }

            NumAssert.AreSame(expected, actual, 0);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(1, 3, 4)]
        [TestCase(2, 5, 4)]
        [TestCase(5, 7, 6)]
        public void Phase(int count, int xStride, int dstStride)
        {
            var x = TestVector.RandomComplex(42, count, xStride);

            var expected = x.Select(c => c.Phase).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Phase(x, actual);
            }

            NumAssert.AreSame(expected, actual, 0);

            TestVector.FailIfOutOfRangeWrite(actual);
        }
    }
}
