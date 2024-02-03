using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MatrixExtension_Test
    {
        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 3, 5)]
        [TestCase(3, 3, 3, 3)]
        [TestCase(3, 3, 5, 4)]
        [TestCase(1, 3, 1, 1)]
        [TestCase(1, 3, 5, 2)]
        [TestCase(3, 1, 3, 4)]
        [TestCase(3, 1, 7, 7)]
        [TestCase(2, 3, 2, 4)]
        [TestCase(2, 3, 4, 3)]
        [TestCase(3, 2, 3, 4)]
        [TestCase(3, 2, 6, 4)]
        public void PointwiseMul(int rowCount, int colCount, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomMatrixDouble(57, rowCount, colCount, yStride);
            var destination = x.PointwiseMul(y);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    var expected = x[row, col] * y[row, col];
                    var actual = destination[row, col];
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
                }
            }
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 3, 5)]
        [TestCase(3, 3, 3, 3)]
        [TestCase(3, 3, 5, 4)]
        [TestCase(1, 3, 1, 1)]
        [TestCase(1, 3, 5, 2)]
        [TestCase(3, 1, 3, 4)]
        [TestCase(3, 1, 7, 7)]
        [TestCase(2, 3, 2, 4)]
        [TestCase(2, 3, 4, 3)]
        [TestCase(3, 2, 3, 4)]
        [TestCase(3, 2, 6, 4)]
        public void PointwiseDiv(int rowCount, int colCount, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomMatrixNonZeroDouble(57, rowCount, colCount, yStride);
            var destination = x.PointwiseDiv(y);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    var expected = x[row, col] / y[row, col];
                    var actual = destination[row, col];
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
                }
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 1, 3)]
        [TestCase(1, 3, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 2, 3)]
        [TestCase(2, 3, 2)]
        [TestCase(4, 5, 8)]
        [TestCase(9, 6, 11)]
        public void Transpose(int rowCount, int colCount, int xStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var destination = x.Transpose();

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(x[row, col] == destination[col, row]);
                }
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 1, 3)]
        [TestCase(1, 3, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 2, 3)]
        [TestCase(2, 3, 2)]
        [TestCase(4, 5, 8)]
        [TestCase(9, 6, 11)]
        public void Conjugate(int rowCount, int colCount, int xStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, rowCount, colCount, xStride);
            var destination = x.Conjugate();

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(x[row, col].Conjugate() == destination[row, col]);
                }
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 1, 3)]
        [TestCase(1, 3, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 2, 3)]
        [TestCase(2, 3, 2)]
        [TestCase(4, 5, 8)]
        [TestCase(9, 6, 11)]
        public void ConjugateTranspose(int rowCount, int colCount, int xStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, rowCount, colCount, xStride);
            var destination = x.ConjugateTranspose();

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(x[row, col].Conjugate() == destination[col, row]);
                }
            }
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        public void Inverse_Single(int n, int xStride)
        {
            var x = Utilities.CreateRandomMatrixSingle(42, n, n, xStride);
            var destination = x.Inverse();

            var identity = destination * x;
            for (var row = 0; row < n; row++)
            {
                for (var col = 0; col < n; col++)
                {
                    if (row == col)
                    {
                        Assert.That(identity[row, col], Is.EqualTo(1.0).Within(1.0E-6));
                    }
                    else
                    {
                        Assert.That(identity[row, col], Is.EqualTo(0.0).Within(1.0E-6));
                    }
                }
            }
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        public void Inverse_Double(int n, int xStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, n, n, xStride);
            var destination = x.Inverse();

            var identity = destination * x;
            for (var row = 0; row < n; row++)
            {
                for (var col = 0; col < n; col++)
                {
                    if (row == col)
                    {
                        Assert.That(identity[row, col], Is.EqualTo(1.0).Within(1.0E-12));
                    }
                    else
                    {
                        Assert.That(identity[row, col], Is.EqualTo(0.0).Within(1.0E-12));
                    }
                }
            }
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        public void Inverse_Complex(int n, int xStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, n, n, xStride);
            var destination = x.Inverse();

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
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 2, 3)]
        [TestCase(3, 2, 5)]
        [TestCase(2, 3, 2)]
        [TestCase(2, 3, 4)]
        [TestCase(6, 3, 7)]
        [TestCase(4, 7, 5)]
        public void PseudoInverse_Single(int rowCount, int colCount, int aStride)
        {
            var a = Utilities.CreateRandomMatrixSingle(42, rowCount, colCount, aStride);
            var actual1 = a.PseudoInverse(1.0E-6F);
            var actual2 = a.PseudoInverse();

            var ma = Utilities.ToMathNet(a);
            var expected = ma.PseudoInverse();

            for (var row = 0; row < expected.RowCount; row++)
            {
                for (var col = 0; col < expected.ColumnCount; col++)
                {
                    Assert.That(actual1[row, col], Is.EqualTo(expected[row, col]).Within(1.0E-5));
                    Assert.That(actual2[row, col], Is.EqualTo(expected[row, col]).Within(1.0E-5));
                }
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 2, 3)]
        [TestCase(3, 2, 5)]
        [TestCase(2, 3, 2)]
        [TestCase(2, 3, 4)]
        [TestCase(6, 3, 7)]
        [TestCase(4, 7, 5)]
        public void PseudoInverse_Double(int rowCount, int colCount, int aStride)
        {
            var a = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, aStride);
            var actual1 = a.PseudoInverse(1.0E-12);
            var actual2 = a.PseudoInverse();

            var ma = Utilities.ToMathNet(a);
            var expected = ma.PseudoInverse();

            for (var row = 0; row < expected.RowCount; row++)
            {
                for (var col = 0; col < expected.ColumnCount; col++)
                {
                    Assert.That(actual1[row, col], Is.EqualTo(expected[row, col]).Within(1.0E-12));
                    Assert.That(actual2[row, col], Is.EqualTo(expected[row, col]).Within(1.0E-12));
                }
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 2, 3)]
        [TestCase(3, 2, 5)]
        [TestCase(2, 3, 2)]
        [TestCase(2, 3, 4)]
        [TestCase(6, 3, 7)]
        [TestCase(4, 7, 5)]
        public void PseudoInverse_Complex(int rowCount, int colCount, int aStride)
        {
            var a = Utilities.CreateRandomMatrixComplex(42, rowCount, colCount, aStride);
            var actual1 = a.PseudoInverse(1.0E-12);
            var actual2 = a.PseudoInverse();

            var ma = Utilities.ToMathNet(a);
            var expected = ma.PseudoInverse();

            for (var row = 0; row < expected.RowCount; row++)
            {
                for (var col = 0; col < expected.ColumnCount; col++)
                {
                    Assert.That(actual1[row, col].Real, Is.EqualTo(expected[row, col].Real).Within(1.0E-12));
                    Assert.That(actual1[row, col].Imaginary, Is.EqualTo(expected[row, col].Imaginary).Within(1.0E-12));
                    Assert.That(actual2[row, col].Real, Is.EqualTo(expected[row, col].Real).Within(1.0E-12));
                    Assert.That(actual2[row, col].Imaginary, Is.EqualTo(expected[row, col].Imaginary).Within(1.0E-12));
                }
            }
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 5)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 3, 7)]
        [TestCase(3, 3, 3, 3)]
        [TestCase(3, 3, 5, 8)]
        [TestCase(1, 3, 1, 1)]
        [TestCase(1, 3, 5, 1)]
        [TestCase(3, 1, 3, 3)]
        [TestCase(3, 1, 7, 7)]
        [TestCase(2, 3, 2, 3)]
        [TestCase(2, 3, 4, 5)]
        [TestCase(3, 2, 3, 6)]
        [TestCase(3, 2, 6, 3)]
        public void Map(int rowCount, int colCount, int xStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var destination = x.Map(value => new Complex(0, -value));

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(destination[row, col].Real == 0);
                    Assert.That(destination[row, col].Imaginary == -x[row, col]);
                }
            }
        }
    }
}
