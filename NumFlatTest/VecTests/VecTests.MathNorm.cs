using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class VecTests_MathNorm
    {
        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void NormSingle(int count, int stride)
        {
            var x = TestVector.RandomSingle(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.L2Norm();

            float actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Norm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6F));
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void NormDouble(int count, int stride)
        {
            var x = TestVector.RandomDouble(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.L2Norm();

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Norm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void NormComplex(int count, int stride)
        {
            var x = TestVector.RandomComplex(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.L2Norm();

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Norm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 2)]
        [TestCase(1, 2, 3)]
        [TestCase(2, 1, 4)]
        [TestCase(2, 2, 2)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void NormSingle(int count, int stride, float p)
        {
            var x = TestVector.RandomSingle(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Norm(p);

            float actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Norm(p);
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6F));
        }

        [TestCase(1, 1, 2)]
        [TestCase(1, 2, 3)]
        [TestCase(2, 1, 4)]
        [TestCase(2, 2, 2)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void NormDouble(int count, int stride, double p)
        {
            var x = TestVector.RandomDouble(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Norm(p);

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Norm(p);
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 2)]
        [TestCase(1, 2, 3)]
        [TestCase(2, 1, 4)]
        [TestCase(2, 2, 2)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void NormComplex(int count, int stride, double p)
        {
            var x = TestVector.RandomComplex(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Norm(p);

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Norm(p);
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void L1NormSingle(int count, int stride)
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
        public void L1NormDouble(int count, int stride)
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
        public void L1NormComplex(int count, int stride)
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
        public void InfinityNormSingle(int count, int stride)
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
        public void InfinityNormDouble(int count, int stride)
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
        public void InfinityNormComplex(int count, int stride)
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
        public void NormalizeDouble(int count, int xStride, int dstStride)
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
        public void NormalizeComplex(int count, int xStride, int dstStride)
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
        public void NormalizeSingle(int count, int xStride, int dstStride, float p)
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
        public void NormalizeDouble(int count, int xStride, int dstStride, double p)
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
        public void NormalizeComplex(int count, int xStride, int dstStride, double p)
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

        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 2)]
        [TestCase(2, 1, 1)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 2, 3)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void DistanceSingle(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomSingle(42, count, xStride);
            var y = TestVector.RandomSingle(57, count, yStride);

            var expected1 = (x - y).Norm();
            var expected2 = expected1 * expected1;

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var actual1 = x.Distance(y);
                var actual2 = x.DistanceSquared(y);
                Assert.That(actual1, Is.EqualTo(expected1).Within(1.0E-6F));
                Assert.That(actual2, Is.EqualTo(expected2).Within(1.0E-6F));
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 2)]
        [TestCase(2, 1, 1)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 2, 3)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void DistanceDouble(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.RandomDouble(57, count, yStride);

            var expected1 = (x - y).Norm();
            var expected2 = expected1 * expected1;

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var actual1 = x.Distance(y);
                var actual2 = x.DistanceSquared(y);
                Assert.That(actual1, Is.EqualTo(expected1).Within(1.0E-12));
                Assert.That(actual2, Is.EqualTo(expected2).Within(1.0E-12));
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 2)]
        [TestCase(2, 1, 1)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 2, 3)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void DistanceComplex(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomComplex(42, count, xStride);
            var y = TestVector.RandomComplex(57, count, yStride);

            var expected1 = (x - y).Norm();
            var expected2 = expected1 * expected1;

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var actual1 = x.Distance(y);
                var actual2 = x.DistanceSquared(y);
                Assert.That(actual1, Is.EqualTo(expected1).Within(1.0E-12));
                Assert.That(actual2, Is.EqualTo(expected2).Within(1.0E-12));
            }
        }
    }
}
