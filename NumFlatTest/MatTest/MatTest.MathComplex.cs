using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MatTest_MathComplex
    {
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        [TestCase(5, 6)]
        public void Determinant(int n, int xStride)
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

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(4, 6, 5)]
        public void Inverse(int n, int xStride, int dstStride)
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

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Rank_Zero(int n)
        {
            var x = new Mat<Complex>(n, n);

            int rank;
            using (x.EnsureUnchanged())
            {
                rank = x.Rank();
            }

            Assert.That(rank, Is.EqualTo(0));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Rank_OneOrMore(int n)
        {
            for (var expected = 1; expected <= n; expected++)
            {
                var src = TestMatrix.RandomComplex(42, expected, n, expected);
                var x = src * src.Transpose();

                int rank;
                using (x.EnsureUnchanged())
                {
                    rank = x.Rank();
                }

                Assert.That(rank, Is.EqualTo(expected));
            }
        }

        [Test]
        public void Rank_Tolerance()
        {
            var a = new Complex[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            }
            .ToMatrix();

            using (a.EnsureUnchanged())
            {
                Assert.That(a.Rank(), Is.EqualTo(2));
                Assert.That(a.Rank(5), Is.EqualTo(1));
                Assert.That(a.Rank(10), Is.EqualTo(1));
                Assert.That(a.Rank(15), Is.EqualTo(1));
                Assert.That(a.Rank(20), Is.EqualTo(0));
            }
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 5)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 4, 3)]
        [TestCase(3, 2, 3, 2)]
        [TestCase(3, 2, 5, 4)]
        [TestCase(2, 3, 2, 3)]
        [TestCase(2, 3, 4, 4)]
        [TestCase(6, 3, 7, 4)]
        [TestCase(4, 7, 5, 9)]
        public void PseudoInverse(int rowCount, int colCount, int aStride, int dstStride)
        {
            var a = TestMatrix.RandomComplex(42, rowCount, colCount, aStride);

            var ma = Interop.ToMathNet(a);
            var expected = ma.PseudoInverse();

            var actual = TestMatrix.RandomComplex(0, colCount, rowCount, dstStride);
            using (a.EnsureUnchanged())
            {
                Mat.PseudoInverse(a, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [Test]
        public void PseudoInverse_Tolerance()
        {
            var a = new Complex[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            }
            .ToMatrix();

            var result1 = new Complex[,]
            {
                { -0.63888889, -0.16666667, 0.30555556 },
                { -0.05555556, 0.00000000, 0.05555556 },
                { 0.52777778, 0.16666667, -0.19444444 },
            }
            .ToMatrix();

            var result2 = new Complex[,]
            {
                { 0.00611649, 0.0148213, 0.02352611 },
                { 0.0072985, 0.01768552, 0.02807254 },
                { 0.00848052, 0.02054974, 0.03261896 },
            }
            .ToMatrix();

            using (a.EnsureUnchanged())
            {
                NumAssert.AreSame(result1, a.PseudoInverse(0.5), 1.0E-6);
                NumAssert.AreSame(result2, a.PseudoInverse(5), 1.0E-6);
                NumAssert.AreSame(new Mat<Complex>(3, 3), a.PseudoInverse(20), 1.0E-6);
            }
        }
    }
}
