using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class Mat_Math_Single_Test
    {
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        [TestCase(5, 6)]
        public void Determinant(int n, int xStride)
        {
            var x = TestMatrix.RandomSingle(42, n, n, xStride);

            var actual = Mat.Determinant(x);
            var expected = Interop.ToMathNet(x).Determinant();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6F));
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(4, 6, 5)]
        public void Inverse(int n, int xStride, int dstStride)
        {
            var x = TestMatrix.RandomSingle(42, n, n, xStride);

            var inverse = TestMatrix.RandomSingle(0, n, n, dstStride);
            using (x.EnsureUnchanged())
            {
                Mat.Inverse(x, inverse);
            }

            var actual = inverse * x;
            var expected = MatrixBuilder.Identity<float>(n);
            NumAssert.AreSame(expected, actual, 1.0E-6F);

            TestMatrix.FailIfOutOfRangeWrite(inverse);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Rank_Zero(int n)
        {
            var x = new Mat<float>(n, n);

            Assert.That(x.Rank(), Is.EqualTo(0));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Rank_OneOrMore(int n)
        {
            for (var rank = 1; rank <= n; rank++)
            {
                var src = TestMatrix.RandomSingle(42, rank, n, rank);
                var x = src * src.Transpose();

                Assert.That(x.Rank(), Is.EqualTo(rank));
            }
        }

        [Test]
        public void Rank_Tolerance()
        {
            var a = new float[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            }
            .ToMatrix();

            Assert.That(a.Rank(), Is.EqualTo(2));
            Assert.That(a.Rank(5), Is.EqualTo(1));
            Assert.That(a.Rank(10), Is.EqualTo(1));
            Assert.That(a.Rank(15), Is.EqualTo(1));
            Assert.That(a.Rank(20), Is.EqualTo(0));
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
            var a = TestMatrix.RandomSingle(42, rowCount, colCount, aStride);

            var ma = Interop.ToMathNet(a);
            var expected = ma.PseudoInverse();

            var actual = TestMatrix.RandomSingle(0, colCount, rowCount, dstStride);
            using (a.EnsureUnchanged())
            {
                Mat.PseudoInverse(a, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-5F); // 1.0E-6F is too small.

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [Test]
        public void PseudoInverse_Tolerance()
        {
            var a = new float[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            }
            .ToMatrix();

            var result1 = new float[,]
            {
                { -0.63888889F, -0.16666667F, 0.30555556F },
                { -0.05555556F, 0.00000000F, 0.05555556F },
                { 0.52777778F, 0.16666667F, -0.19444444F },
            }
            .ToMatrix();

            var result2 = new float[,]
            {
                { 0.00611649F, 0.0148213F, 0.02352611F },
                { 0.0072985F, 0.01768552F, 0.02807254F },
                { 0.00848052F, 0.02054974F, 0.03261896F },
            }
            .ToMatrix();

            NumAssert.AreSame(result1, a.PseudoInverse(0.5F), 1.0E-6F);
            NumAssert.AreSame(result2, a.PseudoInverse(5F), 1.0E-6F);
            NumAssert.AreSame(new Mat<float>(3, 3), a.PseudoInverse(20F), 1.0E-6F);
        }
    }
}
