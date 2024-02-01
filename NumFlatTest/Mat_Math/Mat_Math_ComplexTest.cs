﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class Mat_Math_ComplexTest
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

            var mathNet = new DenseMatrix(n, n);
            for (var row = 0; row < n; row++)
            {
                for (var col = 0; col < n; col++)
                {
                    mathNet[row, col] = x[row, col];
                }
            }
            var expected = mathNet.Determinant();

            Assert.That(expected.Real, Is.EqualTo(actual.Real).Within(1.0E-12));
            Assert.That(expected.Imaginary, Is.EqualTo(actual.Imaginary).Within(1.0E-12));
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