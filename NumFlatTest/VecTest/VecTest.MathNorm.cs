using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class VecTest_MathNorm
    {
        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void L2Single(int count, int stride)
        {
            var x = TestVector.RandomSingle(42, count, stride);
            
            var mx = Interop.ToMathNet(x);
            var expected = mx.L2Norm();

            float actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L2Norm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6F));
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void L2Double(int count, int stride)
        {
            var x = TestVector.RandomDouble(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.L2Norm();

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L2Norm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void L2Complex(int count, int stride)
        {
            var x = TestVector.RandomComplex(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.L2Norm();

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L2Norm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void L1Single(int count, int stride)
        {
            var x = TestVector.RandomSingle(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.L1Norm();

            float actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L1Norm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6F));
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void L1Double(int count, int stride)
        {
            var x = TestVector.RandomDouble(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.L1Norm();

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L1Norm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void L1Complex(int count, int stride)
        {
            var x = TestVector.RandomComplex(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.L1Norm();

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L1Norm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void InfinitySingle(int count, int stride)
        {
            var x = TestVector.RandomSingle(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.InfinityNorm();

            float actual;
            using (x.EnsureUnchanged())
            {
                actual = x.InfinityNorm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6F));
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void InfinityDouble(int count, int stride)
        {
            var x = TestVector.RandomDouble(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.InfinityNorm();

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.InfinityNorm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void InfinityComplex(int count, int stride)
        {
            var x = TestVector.RandomComplex(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.InfinityNorm();

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.InfinityNorm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 2)]
        [TestCase(2, 1, 1)]
        [TestCase(2, 2, 3)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void NormalizeSingle(int count, int xStride, int dstStride)
        {
            var x = TestVector.RandomSingle(42, count, xStride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Normalize(2);

            Vec<float> actual = TestVector.RandomSingle(0, count, dstStride);
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
        public void NormalizeDouble(int count, int xStride, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Normalize(2);

            Vec<double> actual = TestVector.RandomDouble(0, count, dstStride);
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
        public void NormalizeComplex(int count, int xStride, int dstStride)
        {
            var x = TestVector.RandomComplex(42, count, xStride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Normalize(2);

            Vec<Complex> actual = TestVector.RandomComplex(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Noramlize(x, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }
    }
}
