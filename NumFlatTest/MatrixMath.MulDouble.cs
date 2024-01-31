using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MatrixMathMulDouble
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
            var x = Utilities.CreateRandomMatrixDouble(42, m, k, xStride);
            var y = Utilities.CreateRandomMatrixDouble(57, k, n, yStride);
            var destination = Utilities.CreateRandomMatrixDouble(0, m, n, dstStride);
            Mat.Mul(x, y, destination, false, false);

            var mx = DenseMatrix.OfArray(x.ToArray());
            var my = DenseMatrix.OfArray(y.ToArray());
            var md = mx * my;

            for (var row = 0; row < destination.RowCount; row++)
            {
                for (var col = 0; col < destination.ColCount; col++)
                {
                    var expected = md[row, col];
                    var actual = destination[row, col];
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));
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
            var x = Utilities.CreateRandomMatrixDouble(42, m, k, xStride);
            var y = Utilities.CreateRandomMatrixDouble(57, n, k, yStride);
            var destination = Utilities.CreateRandomMatrixDouble(0, m, n, dstStride);
            Mat.Mul(x, y, destination, false, true);

            var mx = DenseMatrix.OfArray(x.ToArray());
            var my = DenseMatrix.OfArray(y.ToArray());
            var md = mx * my.Transpose();

            for (var row = 0; row < destination.RowCount; row++)
            {
                for (var col = 0; col < destination.ColCount; col++)
                {
                    var expected = md[row, col];
                    var actual = destination[row, col];
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));
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
            var x = Utilities.CreateRandomMatrixDouble(42, k, m, xStride);
            var y = Utilities.CreateRandomMatrixDouble(57, k, n, yStride);
            var destination = Utilities.CreateRandomMatrixDouble(0, m, n, dstStride);
            Mat.Mul(x, y, destination, true, false);

            var mx = DenseMatrix.OfArray(x.ToArray());
            var my = DenseMatrix.OfArray(y.ToArray());
            var md = mx.Transpose() * my;

            for (var row = 0; row < destination.RowCount; row++)
            {
                for (var col = 0; col < destination.ColCount; col++)
                {
                    var expected = md[row, col];
                    var actual = destination[row, col];
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));
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
            var x = Utilities.CreateRandomMatrixDouble(42, k, m, xStride);
            var y = Utilities.CreateRandomMatrixDouble(57, n, k, yStride);
            var destination = Utilities.CreateRandomMatrixDouble(0, m, n, dstStride);
            Mat.Mul(x, y, destination, true, true);

            var mx = DenseMatrix.OfArray(x.ToArray());
            var my = DenseMatrix.OfArray(y.ToArray());
            var md = mx.Transpose() * my.Transpose();

            for (var row = 0; row < destination.RowCount; row++)
            {
                for (var col = 0; col < destination.ColCount; col++)
                {
                    var expected = md[row, col];
                    var actual = destination[row, col];
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));
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
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomVectorDouble(57, colCount, yStride);
            var destination = Utilities.CreateRandomVectorDouble(0, rowCount, dstStride);
            Mat.Mul(x, y, destination, false);

            var mx = DenseMatrix.OfArray(x.ToArray());
            var my = DenseVector.OfArray(y.ToArray());
            var md = mx * my;

            var expected = md.ToArray();
            var actual = destination.ToArray();
            Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));

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
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomVectorDouble(57, rowCount, yStride);
            var destination = Utilities.CreateRandomVectorDouble(0, colCount, dstStride);
            Mat.Mul(x, y, destination, true);

            var mx = DenseMatrix.OfArray(x.ToArray()).Transpose();
            var my = DenseVector.OfArray(y.ToArray());
            var md = mx * my;

            var expected = md.ToArray();
            var actual = destination.ToArray();
            Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));

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
            var x = Utilities.CreateRandomMatrixDouble(42, m, k, xStride);
            var y = Utilities.CreateRandomMatrixDouble(57, k, n, yStride);
            var destination = x * y;

            var mx = DenseMatrix.OfArray(x.ToArray());
            var my = DenseMatrix.OfArray(y.ToArray());
            var md = mx * my;

            for (var row = 0; row < destination.RowCount; row++)
            {
                for (var col = 0; col < destination.ColCount; col++)
                {
                    var expected = md[row, col];
                    var actual = destination[row, col];
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));
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
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomVectorDouble(57, colCount, yStride);
            var destination = x * y;

            var mx = DenseMatrix.OfArray(x.ToArray());
            var my = DenseVector.OfArray(y.ToArray());
            var md = mx * my;

            var expected = md.ToArray();
            var actual = destination.ToArray();
            Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));
        }
    }
}
