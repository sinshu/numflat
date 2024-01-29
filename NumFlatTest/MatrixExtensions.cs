using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MatrixExtensions
    {
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

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        public void InverseSingle(int n, int xStride)
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
        public void InverseDouble(int n, int xStride)
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
        public void InverseComplex(int n, int xStride)
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
    }
}
