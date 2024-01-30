using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MatrixMathMulSingle
    {
        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(1, 1, 1, 2, 3, 4)]
        [TestCase(2, 2, 2, 2, 2, 2)]
        [TestCase(2, 2, 2, 5, 4, 3)]
        [TestCase(2, 4, 3, 5, 6, 5)]
        [TestCase(5, 4, 3, 5, 5, 5)]
        [TestCase(8, 7, 9, 8, 9, 8)]
        [TestCase(7, 8, 9, 10, 10, 10)]
        public void MulMatMat_NN(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixSingle(42, m, k, xStride);
            var y = Utilities.CreateRandomMatrixSingle(57, k, n, yStride);
            var destination = Utilities.CreateRandomMatrixSingle(0, m, n, dstStride);
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
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-6));
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
        public void MulMatMat_NT(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixSingle(42, m, k, xStride);
            var y = Utilities.CreateRandomMatrixSingle(57, n, k, yStride);
            var destination = Utilities.CreateRandomMatrixSingle(0, m, n, dstStride);
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
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-6));
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
        public void MulMatMat_TN(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixSingle(42, k, m, xStride);
            var y = Utilities.CreateRandomMatrixSingle(57, k, n, yStride);
            var destination = Utilities.CreateRandomMatrixSingle(0, m, n, dstStride);
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
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-6));
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
        public void MulMatMat_TT(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixSingle(42, k, m, xStride);
            var y = Utilities.CreateRandomMatrixSingle(57, n, k, yStride);
            var destination = Utilities.CreateRandomMatrixSingle(0, m, n, dstStride);
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
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-6));
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
        public void MulMatVec(int rowCount, int colCount, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixSingle(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomVectorSingle(57, colCount, yStride);
            var destination = Utilities.CreateRandomVectorSingle(0, rowCount, dstStride);
            Mat.Mul(x, y, destination);

            var mx = DenseMatrix.OfArray(x.ToArray());
            var my = DenseVector.OfArray(y.ToArray());
            var md = mx * my;

            var expected = md.ToArray();
            var actual = destination.ToArray();
            Assert.That(expected, Is.EqualTo(actual).Within(1.0E-6));

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
        public void MulOperatorMatMat(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixSingle(42, m, k, xStride);
            var y = Utilities.CreateRandomMatrixSingle(57, k, n, yStride);
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
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-6));
                }
            }
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4, 5)]
        [TestCase(2, 2, 2, 1, 1)]
        [TestCase(3, 3, 3, 7, 6)]
        [TestCase(2, 3, 3, 1, 6)]
        [TestCase(7, 3, 7, 1, 1)]
        [TestCase(7, 4, 7, 2, 5)]
        public void MulOperatorMatVec(int rowCount, int colCount, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixSingle(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomVectorSingle(57, colCount, yStride);
            var destination = x * y;

            var mx = DenseMatrix.OfArray(x.ToArray());
            var my = DenseVector.OfArray(y.ToArray());
            var md = mx * my;

            var expected = md.ToArray();
            var actual = destination.ToArray();
            Assert.That(expected, Is.EqualTo(actual).Within(1.0E-6));
        }
    }
}
