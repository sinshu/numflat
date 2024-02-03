using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class Mat_Math_Complex_Test
    {
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        [TestCase(5, 6)]
        public void Determinant(int n, int xStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, n, n, xStride);
            var actual = Mat.Determinant(x);

            var mathNet = Utilities.ToMathNet(x);
            var expected = mathNet.Determinant();

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
            var x = Utilities.CreateRandomMatrixComplex(42, n, n, xStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, n, n, dstStride);
            Mat.Inverse(x, destination);

            var identity = destination * x;
            for (var row = 0; row < n; row++)
            {
                for (var col = 0; col < n; col++)
                {
                    if (row == col)
                    {
                        Assert.That(identity[row, col].Real, Is.EqualTo(1.0).Within(1.0E-12));
                        Assert.That(identity[row, col].Imaginary, Is.EqualTo(0.0).Within(1.0E-12));
                    }
                    else
                    {
                        Assert.That(identity[row, col].Real, Is.EqualTo(0.0).Within(1.0E-12));
                        Assert.That(identity[row, col].Imaginary, Is.EqualTo(0.0).Within(1.0E-12));
                    }
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Rank_Zero(int n)
        {
            var x = new Mat<Complex>(n, n);
            Assert.That(x.Rank() == 0);
            Assert.That(x.Rank(1.0E-12) == 0);
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
                var src = Utilities.CreateRandomMatrixComplex(42, rank, n, rank);
                var x = src * src.ConjugateTranspose();
                Assert.That(x.Rank() == rank);
                Assert.That(x.Rank(1.0E-12) == rank);
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

            Assert.True(a.Rank() == 2);
            Assert.True(a.Rank(5) == 1);
            Assert.True(a.Rank(10) == 1);
            Assert.True(a.Rank(15) == 1);
            Assert.True(a.Rank(20) == 0);
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
        public void PseudoInverse_Arg3(int rowCount, int colCount, int aStride, int dstStride)
        {
            var a = Utilities.CreateRandomMatrixComplex(42, rowCount, colCount, aStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, colCount, rowCount, dstStride);
            Mat.PseudoInverse(a, destination, 1.0E-12);

            var ma = Utilities.ToMathNet(a);
            var expected = ma.PseudoInverse();

            for (var row = 0; row < expected.RowCount; row++)
            {
                for (var col = 0; col < expected.ColumnCount; col++)
                {
                    Assert.That(destination[row, col].Real, Is.EqualTo(expected[row, col].Real).Within(1.0E-12));
                    Assert.That(destination[row, col].Imaginary, Is.EqualTo(expected[row, col].Imaginary).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
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
        public void PseudoInverse_Arg2(int rowCount, int colCount, int aStride, int dstStride)
        {
            var a = Utilities.CreateRandomMatrixComplex(42, rowCount, colCount, aStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, colCount, rowCount, dstStride);
            Mat.PseudoInverse(a, destination);

            var ma = Utilities.ToMathNet(a);
            var expected = ma.PseudoInverse();

            for (var row = 0; row < expected.RowCount; row++)
            {
                for (var col = 0; col < expected.ColumnCount; col++)
                {
                    Assert.That(destination[row, col].Real, Is.EqualTo(expected[row, col].Real).Within(1.0E-12));
                    Assert.That(destination[row, col].Imaginary, Is.EqualTo(expected[row, col].Imaginary).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
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

            {
                var pinv = a.PseudoInverse(0.5);
                var error = (pinv - result1).Cols.SelectMany(col => col).Select(Complex.Abs).Max();
                Assert.That(error < 1.0E-6);
            }

            {
                var pinv = a.PseudoInverse(5);
                var error = (pinv - result2).Cols.SelectMany(col => col).Select(Complex.Abs).Max();
                Assert.That(error < 1.0E-6);
            }

            {
                var pinv = a.PseudoInverse(20);
                var error = pinv.Cols.SelectMany(col => col).Select(Complex.Abs).Max();
                Assert.That(error < 1.0E-6);
            }
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 4, 3)]
        [TestCase(3, 1, 3, 3)]
        [TestCase(3, 1, 4, 4)]
        [TestCase(1, 3, 1, 1)]
        [TestCase(1, 3, 6, 5)]
        [TestCase(3, 2, 3, 3)]
        [TestCase(3, 2, 4, 4)]
        [TestCase(2, 3, 2, 2)]
        [TestCase(2, 3, 6, 5)]
        public void Conjugate(int rowCount, int colCount, int xStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, rowCount, colCount, xStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, rowCount, colCount, dstStride);
            Mat.Conjugate(x, destination);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(x[row, col].Conjugate() == destination[row, col]);
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 4, 3)]
        [TestCase(3, 1, 3, 1)]
        [TestCase(3, 1, 3, 4)]
        [TestCase(1, 3, 1, 3)]
        [TestCase(1, 3, 2, 5)]
        [TestCase(3, 2, 3, 2)]
        [TestCase(2, 3, 2, 3)]
        [TestCase(4, 5, 8, 7)]
        [TestCase(9, 6, 11, 7)]
        public void ConjugateTranspose(int rowCount, int colCount, int xStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, rowCount, colCount, xStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, colCount, rowCount, dstStride);
            Mat.ConjugateTranspose(x, destination);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(x[row, col].Conjugate() == destination[col, row]);
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }
    }
}
