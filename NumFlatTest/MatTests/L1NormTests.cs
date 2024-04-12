using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.MatTests
{
    public class L1NormTests
    {
        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(5, 4, 5)]
        [TestCase(5, 4, 6)]
        public void Single(int rowCount, int colCount, int stride)
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
        public void Double(int rowCount, int colCount, int stride)
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
        public void Complex(int rowCount, int colCount, int stride)
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
    }
}
