using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MatrixOperators
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
        public void Add(int rowCount, int colCount, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomMatrixDouble(57, rowCount, colCount, yStride);
            var destination = x + y;

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    var expected = x[row, col] + y[row, col];
                    var actual = destination[row, col];
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));
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
        public void Sub(int rowCount, int colCount, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomMatrixDouble(57, rowCount, colCount, yStride);
            var destination = x - y;

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    var expected = x[row, col] - y[row, col];
                    var actual = destination[row, col];
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));
                }
            }
        }
    }
}
