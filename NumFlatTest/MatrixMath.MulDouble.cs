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
        public void Mul(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, m, k, xStride);
            var y = Utilities.CreateRandomMatrixDouble(57, k, n, yStride);

            var destination = Utilities.CreateRandomMatrixDouble(0, m, n, dstStride);
            Mat.Mul(x, y, destination);

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

            var offset = 0;
            for (var col = 0; col < destination.ColCount; col++)
            {
                var position = offset;
                for (var row = 0; row < dstStride; row++)
                {
                    if (row < destination.RowCount)
                    {
                        Assert.That(!double.IsNaN(destination.Memory.Span[position]));
                    }
                    else if (position < destination.Memory.Length)
                    {
                        Assert.That(double.IsNaN(destination.Memory.Span[position]));
                    }
                    position++;
                }
                offset += dstStride;
            }
        }
    }
}
