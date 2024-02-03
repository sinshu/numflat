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
            var x = Utilities.CreateRandomMatrixSingle(42, n, n, xStride);
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

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(4, 6, 5)]
        public void Inverse(int n, int xStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixSingle(42, n, n, xStride);
            var destination = Utilities.CreateRandomMatrixSingle(0, n, n, dstStride);
            Mat.Inverse(x, destination);

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

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Rank_Zero(int n)
        {
            var x = new Mat<float>(n, n);
            Assert.That(x.Rank() == 0);
            Assert.That(x.Rank(1.0E-6F) == 0);
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
                var src = Utilities.CreateRandomMatrixSingle(42, rank, n, rank);
                var x = src * src.Transpose();
                Assert.That(x.Rank() == rank);
                Assert.That(x.Rank(1.0E-6F) == rank);
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
            var a = Utilities.CreateRandomMatrixSingle(42, rowCount, colCount, aStride);
            var resultArg2 = Utilities.CreateRandomMatrixSingle(0, colCount, rowCount, dstStride);
            Mat.PseudoInverse(a, resultArg2, 1.0E-6F);

            var ma = Utilities.ToMathNet(a);
            var expected = ma.PseudoInverse();

            for (var row = 0; row < expected.RowCount; row++)
            {
                for (var col = 0; col < expected.ColumnCount; col++)
                {
                    Assert.That(resultArg2[row, col], Is.EqualTo(expected[row, col]).Within(1.0E-5));
                }
            }

            Utilities.FailIfOutOfRangeWrite(resultArg2);
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
            var a = Utilities.CreateRandomMatrixSingle(42, rowCount, colCount, aStride);
            var destination = Utilities.CreateRandomMatrixSingle(0, colCount, rowCount, dstStride);
            Mat.PseudoInverse(a, destination);

            var ma = Utilities.ToMathNet(a);
            var expected = ma.PseudoInverse();

            for (var row = 0; row < expected.RowCount; row++)
            {
                for (var col = 0; col < expected.ColumnCount; col++)
                {
                    Assert.That(destination[row, col], Is.EqualTo(expected[row, col]).Within(1.0E-5));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }
    }
}
