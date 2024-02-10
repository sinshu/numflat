using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MatTest_MathNorm
    {
        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(5, 4, 5)]
        [TestCase(5, 4, 6)]
        public void FrobeniusNormSingle(int rowCount, int colCount, int stride)
        {
            var x = TestMatrix.RandomSingle(42, rowCount, colCount, stride);

            float actual;
            using (x.EnsureUnchanged())
            {
                actual = x.FrobeniusNorm();
            }

            var expected = Interop.ToMathNet(x).FrobeniusNorm();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6F));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(5, 4, 5)]
        [TestCase(5, 4, 6)]
        public void FrobeniusNormDouble(int rowCount, int colCount, int stride)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, stride);

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.FrobeniusNorm();
            }

            var expected = Interop.ToMathNet(x).FrobeniusNorm();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(5, 4, 5)]
        [TestCase(5, 4, 6)]
        public void FrobeniusNormComplex(int rowCount, int colCount, int stride)
        {
            var x = TestMatrix.RandomComplex(42, rowCount, colCount, stride);

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.FrobeniusNorm();
            }

            var expected = Interop.ToMathNet(x).FrobeniusNorm();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(5, 4, 5)]
        [TestCase(5, 4, 6)]
        public void L1NormSingle(int rowCount, int colCount, int stride)
        {
            var x = TestMatrix.RandomSingle(42, rowCount, colCount, stride);

            float actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L1Norm();
            }

            var expected = Interop.ToMathNet(x).L1Norm();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6F));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(5, 4, 5)]
        [TestCase(5, 4, 6)]
        public void L1NormDouble(int rowCount, int colCount, int stride)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, stride);

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L1Norm();
            }

            var expected = Interop.ToMathNet(x).L1Norm();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(5, 4, 5)]
        [TestCase(5, 4, 6)]
        public void L1NormComplex(int rowCount, int colCount, int stride)
        {
            var x = TestMatrix.RandomComplex(42, rowCount, colCount, stride);

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L1Norm();
            }

            var expected = Interop.ToMathNet(x).L1Norm();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(5, 4, 5)]
        [TestCase(5, 4, 6)]
        public void L2NormSingle(int rowCount, int colCount, int stride)
        {
            var x = TestMatrix.RandomSingle(42, rowCount, colCount, stride);

            float actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L2Norm();
            }

            var expected = Interop.ToMathNet(x).L2Norm();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6F));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(5, 4, 5)]
        [TestCase(5, 4, 6)]
        public void L2NormDouble(int rowCount, int colCount, int stride)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, stride);

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L2Norm();
            }

            var expected = Interop.ToMathNet(x).L2Norm();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(5, 4, 5)]
        [TestCase(5, 4, 6)]
        public void L2NormComplex(int rowCount, int colCount, int stride)
        {
            var x = TestMatrix.RandomComplex(42, rowCount, colCount, stride);

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L2Norm();
            }

            var expected = Interop.ToMathNet(x).L2Norm();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(5, 4, 5)]
        [TestCase(5, 4, 6)]
        public void InfinityNormSingle(int rowCount, int colCount, int stride)
        {
            var x = TestMatrix.RandomSingle(42, rowCount, colCount, stride);

            float actual;
            using (x.EnsureUnchanged())
            {
                actual = x.InfinityNorm();
            }

            var expected = Interop.ToMathNet(x).InfinityNorm();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6F));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(5, 4, 5)]
        [TestCase(5, 4, 6)]
        public void InfinityNormDouble(int rowCount, int colCount, int stride)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, stride);

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.InfinityNorm();
            }

            var expected = Interop.ToMathNet(x).InfinityNorm();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(5, 4, 5)]
        [TestCase(5, 4, 6)]
        public void InfinityNormComplex(int rowCount, int colCount, int stride)
        {
            var x = TestMatrix.RandomComplex(42, rowCount, colCount, stride);

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.InfinityNorm();
            }

            var expected = Interop.ToMathNet(x).InfinityNorm();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }
    }
}
