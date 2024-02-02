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
    public class Mat_Math_Complex_Mul_Test
    {
        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(1, 1, 1, 2, 3, 4)]
        [TestCase(2, 2, 2, 2, 2, 2)]
        [TestCase(2, 2, 2, 5, 4, 3)]
        [TestCase(2, 4, 3, 5, 6, 5)]
        [TestCase(5, 4, 3, 5, 5, 5)]
        [TestCase(8, 7, 9, 8, 9, 8)]
        [TestCase(7, 8, 9, 10, 10, 10)]
        public void Mul_MatMat_NN(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, m, k, xStride);
            var y = Utilities.CreateRandomMatrixComplex(57, k, n, yStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, m, n, dstStride);
            Mat.Mul(x, y, destination, false, false, false, false);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var md = mx * my;

            for (var row = 0; row < destination.RowCount; row++)
            {
                for (var col = 0; col < destination.ColCount; col++)
                {
                    var actual = destination[row, col];
                    var expected = md[row, col];
                    Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
                    Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(1, 1, 1, 2, 3, 4)]
        [TestCase(2, 2, 2, 2, 2, 2)]
        [TestCase(2, 2, 2, 5, 4, 3)]
        [TestCase(2, 4, 3, 5, 6, 5)]
        [TestCase(5, 4, 3, 5, 5, 5)]
        [TestCase(8, 7, 9, 8, 9, 8)]
        [TestCase(7, 8, 9, 10, 10, 10)]
        public void Mul_MatMat_NT(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, m, k, xStride);
            var y = Utilities.CreateRandomMatrixComplex(57, n, k, yStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, m, n, dstStride);
            Mat.Mul(x, y, destination, false, false, true, false);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var md = mx * my.Transpose();

            for (var row = 0; row < destination.RowCount; row++)
            {
                for (var col = 0; col < destination.ColCount; col++)
                {
                    var actual = destination[row, col];
                    var expected = md[row, col];
                    Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
                    Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(1, 1, 1, 2, 3, 4)]
        [TestCase(2, 2, 2, 2, 2, 2)]
        [TestCase(2, 2, 2, 5, 4, 3)]
        [TestCase(2, 4, 3, 5, 6, 5)]
        [TestCase(5, 4, 3, 5, 5, 5)]
        [TestCase(8, 7, 9, 10, 9, 8)]
        [TestCase(7, 8, 9, 10, 10, 10)]
        public void Mul_MatMat_TN(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, k, m, xStride);
            var y = Utilities.CreateRandomMatrixComplex(57, k, n, yStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, m, n, dstStride);
            Mat.Mul(x, y, destination, true, false, false, false);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var md = mx.Transpose() * my;

            for (var row = 0; row < destination.RowCount; row++)
            {
                for (var col = 0; col < destination.ColCount; col++)
                {
                    var actual = destination[row, col];
                    var expected = md[row, col];
                    Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
                    Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(1, 1, 1, 2, 3, 4)]
        [TestCase(2, 2, 2, 2, 2, 2)]
        [TestCase(2, 2, 2, 5, 4, 3)]
        [TestCase(2, 4, 3, 5, 6, 5)]
        [TestCase(5, 4, 3, 5, 5, 5)]
        [TestCase(8, 7, 9, 10, 9, 8)]
        [TestCase(7, 8, 9, 10, 10, 10)]
        public void Mul_MatMat_TT(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, k, m, xStride);
            var y = Utilities.CreateRandomMatrixComplex(57, n, k, yStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, m, n, dstStride);
            Mat.Mul(x, y, destination, true, false, true, false);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var md = mx.Transpose() * my.Transpose();

            for (var row = 0; row < destination.RowCount; row++)
            {
                for (var col = 0; col < destination.ColCount; col++)
                {
                    var actual = destination[row, col];
                    var expected = md[row, col];
                    Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
                    Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(1, 1, 1, 2, 3, 4)]
        [TestCase(2, 2, 2, 2, 2, 2)]
        [TestCase(2, 2, 2, 5, 4, 3)]
        [TestCase(2, 4, 3, 5, 6, 5)]
        [TestCase(5, 4, 3, 5, 5, 5)]
        [TestCase(8, 7, 9, 8, 9, 8)]
        [TestCase(7, 8, 9, 10, 10, 10)]
        public void Mul_MatMat_CC(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, m, k, xStride);
            var y = Utilities.CreateRandomMatrixComplex(57, k, n, yStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, m, n, dstStride);
            Mat.Mul(x, y, destination, false, true, false, true);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var md = mx.Conjugate() * my.Conjugate();

            for (var row = 0; row < destination.RowCount; row++)
            {
                for (var col = 0; col < destination.ColCount; col++)
                {
                    var actual = destination[row, col];
                    var expected = md[row, col];
                    Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
                    Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(1, 1, 1, 2, 3, 4)]
        [TestCase(2, 2, 2, 2, 2, 2)]
        [TestCase(2, 2, 2, 5, 4, 3)]
        [TestCase(2, 4, 3, 5, 6, 5)]
        [TestCase(5, 4, 3, 5, 5, 5)]
        [TestCase(8, 7, 9, 10, 9, 8)]
        [TestCase(7, 8, 9, 10, 10, 10)]
        public void Mul_MatMat_HH(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, k, m, xStride);
            var y = Utilities.CreateRandomMatrixComplex(57, n, k, yStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, m, n, dstStride);
            Mat.Mul(x, y, destination, true, true, true, true);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var md = mx.ConjugateTranspose() * my.ConjugateTranspose();

            for (var row = 0; row < destination.RowCount; row++)
            {
                for (var col = 0; col < destination.ColCount; col++)
                {
                    var actual = destination[row, col];
                    var expected = md[row, col];
                    Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
                    Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(1, 1, 1, 2, 3, 4)]
        [TestCase(2, 2, 2, 2, 2, 2)]
        [TestCase(2, 2, 2, 5, 4, 3)]
        [TestCase(2, 4, 3, 5, 6, 5)]
        [TestCase(5, 4, 3, 5, 5, 5)]
        [TestCase(8, 7, 9, 8, 9, 8)]
        [TestCase(7, 8, 9, 10, 10, 10)]
        public void Mul_MatMat_NH(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, m, k, xStride);
            var y = Utilities.CreateRandomMatrixComplex(57, n, k, yStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, m, n, dstStride);
            Mat.Mul(x, y, destination, false, false, true, true);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var md = mx * my.ConjugateTranspose();

            for (var row = 0; row < destination.RowCount; row++)
            {
                for (var col = 0; col < destination.ColCount; col++)
                {
                    var actual = destination[row, col];
                    var expected = md[row, col];
                    Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
                    Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(1, 1, 1, 2, 3, 4)]
        [TestCase(2, 2, 2, 2, 2, 2)]
        [TestCase(2, 2, 2, 5, 4, 3)]
        [TestCase(2, 4, 3, 5, 6, 5)]
        [TestCase(5, 4, 3, 5, 5, 5)]
        [TestCase(8, 7, 9, 10, 9, 8)]
        [TestCase(7, 8, 9, 10, 10, 10)]
        public void Mul_MatMat_HN(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, k, m, xStride);
            var y = Utilities.CreateRandomMatrixComplex(57, k, n, yStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, m, n, dstStride);
            Mat.Mul(x, y, destination, true, true, false, false);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var md = mx.ConjugateTranspose() * my;

            for (var row = 0; row < destination.RowCount; row++)
            {
                for (var col = 0; col < destination.ColCount; col++)
                {
                    var actual = destination[row, col];
                    var expected = md[row, col];
                    Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
                    Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4, 5)]
        [TestCase(2, 2, 2, 1, 1)]
        [TestCase(3, 3, 3, 7, 6)]
        [TestCase(2, 3, 3, 1, 6)]
        [TestCase(7, 3, 7, 1, 1)]
        [TestCase(7, 4, 7, 2, 5)]
        public void Mul_MatVec_N(int rowCount, int colCount, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomVectorComplex(57, colCount, yStride);
            var destination = Utilities.CreateRandomVectorComplex(0, rowCount, dstStride);
            Mat.Mul(x, y, destination, false, false);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var md = mx * my;

            var actual = destination.ToArray();
            var expected = md.ToArray();
            for (var i = 0; i < destination.Count; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-12));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-12));
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4, 5)]
        [TestCase(2, 2, 2, 1, 1)]
        [TestCase(3, 3, 3, 7, 6)]
        [TestCase(2, 3, 3, 1, 6)]
        [TestCase(7, 3, 7, 1, 1)]
        [TestCase(7, 4, 7, 2, 5)]
        public void Mul_MatVec_T(int rowCount, int colCount, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomVectorComplex(57, rowCount, yStride);
            var destination = Utilities.CreateRandomVectorComplex(0, colCount, dstStride);
            Mat.Mul(x, y, destination, true, false);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var md = mx.Transpose() * my;

            var actual = destination.ToArray();
            var expected = md.ToArray();
            for (var i = 0; i < destination.Count; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-12));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-12));
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4, 5)]
        [TestCase(2, 2, 2, 1, 1)]
        [TestCase(3, 3, 3, 7, 6)]
        [TestCase(2, 3, 3, 1, 6)]
        [TestCase(7, 3, 7, 1, 1)]
        [TestCase(7, 4, 7, 2, 5)]
        public void Mul_MatVec_C(int rowCount, int colCount, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomVectorComplex(57, colCount, yStride);
            var destination = Utilities.CreateRandomVectorComplex(0, rowCount, dstStride);
            Mat.Mul(x, y, destination, false, true);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var md = mx.Conjugate() * my;

            var actual = destination.ToArray();
            var expected = md.ToArray();
            for (var i = 0; i < destination.Count; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-12));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-12));
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4, 5)]
        [TestCase(2, 2, 2, 1, 1)]
        [TestCase(3, 3, 3, 7, 6)]
        [TestCase(2, 3, 3, 1, 6)]
        [TestCase(7, 3, 7, 1, 1)]
        [TestCase(7, 4, 7, 2, 5)]
        public void Mul_MatVec_H(int rowCount, int colCount, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomVectorComplex(57, rowCount, yStride);
            var destination = Utilities.CreateRandomVectorComplex(0, colCount, dstStride);
            Mat.Mul(x, y, destination, true, true);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var md = mx.ConjugateTranspose() * my;

            var actual = destination.ToArray();
            var expected = md.ToArray();
            for (var i = 0; i < destination.Count; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-12));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-12));
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 1, 2, 3)]
        [TestCase(2, 2, 2, 2, 2)]
        [TestCase(2, 2, 2, 5, 4)]
        [TestCase(2, 4, 3, 5, 6)]
        [TestCase(5, 4, 3, 5, 5)]
        [TestCase(8, 7, 9, 8, 9)]
        [TestCase(7, 8, 9, 10, 10)]
        public void Operator_MatMat(int m, int n, int k, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, m, k, xStride);
            var y = Utilities.CreateRandomMatrixComplex(57, k, n, yStride);
            var destination = x * y;

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var md = mx * my;

            for (var row = 0; row < destination.RowCount; row++)
            {
                for (var col = 0; col < destination.ColCount; col++)
                {
                    var actual = destination[row, col];
                    var expected = md[row, col];
                    Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
                    Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
                }
            }
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4)]
        [TestCase(2, 2, 2, 1)]
        [TestCase(3, 3, 3, 7)]
        [TestCase(2, 3, 3, 1)]
        [TestCase(7, 3, 7, 1)]
        [TestCase(7, 4, 7, 2)]
        public void Operator_MatVec(int rowCount, int colCount, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomMatrixComplex(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomVectorComplex(57, colCount, yStride);
            var destination = x * y;

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var md = mx * my;

            var actual = destination.ToArray();
            var expected = md.ToArray();
            for (var i = 0; i < destination.Count; i++)
            {
                Assert.That(actual[i].Real, Is.EqualTo(expected[i].Real).Within(1.0E-12));
                Assert.That(actual[i].Imaginary, Is.EqualTo(expected[i].Imaginary).Within(1.0E-12));
            }
        }
    }
}
