using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class Matrix
    {
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(1, 3)]
        [TestCase(3, 1)]
        [TestCase(2, 3)]
        [TestCase(3, 2)]
        public void New(int rowCount, int colCount)
        {
            var matrix = new Mat<int>(rowCount, colCount);

            Assert.That(matrix.RowCount == rowCount);
            Assert.That(matrix.ColCount == colCount);
            Assert.That(matrix.Stride == rowCount);
            Assert.That(matrix.Memory.Length == rowCount * colCount);

            for (var col = 0; col < colCount; col++)
            {
                for (var row = 0; row < rowCount; row++)
                {
                    Assert.That(matrix[row, col] == 0);
                }
            }
        }
    }
}
