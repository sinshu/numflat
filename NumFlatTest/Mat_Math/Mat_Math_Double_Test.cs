﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class Mat_Math_Double_Test
    {
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        [TestCase(5, 6)]
        public void Determinant(int n, int xStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, n, n, xStride);
            var actual = Mat.Determinant(x);

            var mathNet = Utilities.ToMathNet(x);
            var expected = mathNet.Determinant();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(4, 6, 5)]
        public void Inverse(int n, int xStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, n, n, xStride);
            var destination = Utilities.CreateRandomMatrixDouble(0, n, n, dstStride);
            Mat.Inverse(x, destination);

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

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Rank_Zero(int n)
        {
            var x = new Mat<double>(n, n);
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
                var src = Utilities.CreateRandomMatrixDouble(42, rank, n, rank);
                var x = src * src.Transpose();
                Assert.That(x.Rank() == rank);
                Assert.That(x.Rank(1.0E-12) == rank);
            }
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
            var a = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, aStride);
            var resultArg2 = Utilities.CreateRandomMatrixDouble(0, colCount, rowCount, dstStride);
            Mat.PseudoInverse(a, resultArg2, 1.0E-12);

            var ma = Utilities.ToMathNet(a);
            var expected = ma.PseudoInverse();

            for (var row = 0; row < expected.RowCount; row++)
            {
                for (var col = 0; col < expected.ColumnCount; col++)
                {
                    Assert.That(resultArg2[row, col], Is.EqualTo(expected[row, col]).Within(1.0E-12));
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
            var a = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, aStride);
            var destination = Utilities.CreateRandomMatrixDouble(0, colCount, rowCount, dstStride);
            Mat.PseudoInverse(a, destination);

            var ma = Utilities.ToMathNet(a);
            var expected = ma.PseudoInverse();

            for (var row = 0; row < expected.RowCount; row++)
            {
                for (var col = 0; col < expected.ColumnCount; col++)
                {
                    Assert.That(destination[row, col], Is.EqualTo(expected[row, col]).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }
    }
}
