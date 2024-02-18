using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class SpecialTest
    {
        [TestCase(2.3F)]
        [TestCase(-2.3F)]
        [TestCase(4.2F)]
        [TestCase(-4.2F)]
        [TestCase(99.99F)]
        [TestCase(-99.99F)]
        public void Eps_Float(float x)
        {
            Assert.That(Special.Eps(x), Is.EqualTo(Precision.EpsilonOf(x)));
        }

        [TestCase(2.3)]
        [TestCase(-2.3)]
        [TestCase(4.2)]
        [TestCase(-4.2)]
        [TestCase(99.99)]
        [TestCase(-99.99)]
        public void Eps_Double(double x)
        {
            Assert.That(Special.Eps(x), Is.EqualTo(Precision.EpsilonOf(x)));
        }

        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(2, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        public void LowerTriangularToHermitianInplace_Double(int n, int stride)
        {
            var original = TestMatrix.RandomDouble(42, n, n, n);

            var x = TestMatrix.RandomDouble(42, n, n, stride);
            Special.LowerTriangularToHermitianInplace(x);

            for (var row = 0; row < n; row++)
            {
                for (var col = 0; col < n; col++)
                {
                    Assert.That(x[row, col], Is.EqualTo(x[col, row]));

                    if (row >= col)
                    {
                        Assert.That(x[row, col], Is.EqualTo(original[row, col]));
                    }
                }
            }

            TestMatrix.FailIfOutOfRangeWrite(x);
        }

        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(2, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        public void LowerTriangularToHermitianInplace_Complex(int n, int stride)
        {
            var original = TestMatrix.RandomComplex(42, n, n, n);

            var x = TestMatrix.RandomComplex(42, n, n, stride);
            Special.LowerTriangularToHermitianInplace(x);

            for (var row = 0; row < n; row++)
            {
                for (var col = 0; col < n; col++)
                {
                    if (row != col)
                    {
                        Assert.That(x[row, col], Is.EqualTo(NumFlat.ComplexExtensions.Conjugate(x[col, row])));
                    }

                    if (row >= col)
                    {
                        Assert.That(x[row, col], Is.EqualTo(original[row, col]));
                    }
                }
            }

            TestMatrix.FailIfOutOfRangeWrite(x);
        }

        [TestCase(1, 1, 0.5)]
        [TestCase(1, 3, 0.1)]
        [TestCase(2, 2, 2.6)]
        [TestCase(2, 3, 3.1)]
        [TestCase(3, 3, 4.1)]
        [TestCase(3, 4, 0.1)]
        [TestCase(4, 4, 4.1)]
        [TestCase(4, 6, 0.1)]
        [TestCase(5, 5, 4.1)]
        [TestCase(5, 7, 0.1)]
        public void IncreaseDiagonalElementsInplace(int n, int stride, double value)
        {
            var x = TestMatrix.RandomDouble(42, n, n, n);
            var expected = x + value * MatrixBuilder.Identity<double>(n);

            Special.IncreaseDiagonalElementsInplace(x, value);

            NumAssert.AreSame(expected, x, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(x);
        }

        [Test]
        public void ReverseEigenValueOrder()
        {
            var a = TestMatrix.RandomDouble(42, 10, 5, 10).Rows.Covariance();
            var evd = a.Evd();

            Special.ReverseEigenValueOrder(evd);

            var reconstructed = evd.V * evd.D.ToDiagonalMatrix() * evd.V.Transpose();
            NumAssert.AreSame(a, reconstructed, 1.0E-12);
        }

        [Test]
        public void ReverseEigenValueOrder_General()
        {
            var a = TestMatrix.RandomDouble(42, 10, 5, 10).Rows.Covariance();
            var b = TestMatrix.RandomDouble(57, 10, 5, 10).Rows.Covariance();
            var gevd = a.Gevd(b);

            Special.ReverseEigenValueOrder(gevd);

            for (var i = 0; i < a.RowCount; i++)
            {
                var left = a * gevd.V.Cols[i];
                var right = gevd.D[i] * b * gevd.V.Cols[i];
                NumAssert.AreSame(left, right, 1.0E-12);
            }
        }
    }
}
