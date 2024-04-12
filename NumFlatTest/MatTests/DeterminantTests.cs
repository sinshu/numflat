using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.MatTests
{
    public class DeterminantTests
    {
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        [TestCase(5, 6)]
        public void Single(int n, int xStride)
        {
            var x = TestMatrix.RandomSingle(42, n, n, xStride);

            float actual;
            using (x.EnsureUnchanged())
            {
                actual = Mat.Determinant(x);
            }

            var expected = Interop.ToMathNet(x).Determinant();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6F));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        [TestCase(5, 6)]
        public void Double(int n, int xStride)
        {
            var x = TestMatrix.RandomDouble(42, n, n, xStride);

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = Mat.Determinant(x);
            }

            var expected = Interop.ToMathNet(x).Determinant();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        [TestCase(5, 6)]
        public void Complex(int n, int xStride)
        {
            var x = TestMatrix.RandomComplex(42, n, n, xStride);

            Complex actual;
            using (x.EnsureUnchanged())
            {
                actual = Mat.Determinant(x);
            }

            var expected = Interop.ToMathNet(x).Determinant();

            Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
            Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
        }
    }
}
