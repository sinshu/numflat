using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MatrixBuilderTest
    {
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(2, 4)]
        [TestCase(5, 3)]
        public void Array2DToMatrix(int rowCount, int colCount)
        {
            var random = new Random(42);
            var expected = new double[rowCount, colCount];
            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    expected[row, col] = random.NextDouble();
                }
            }

            var actual = expected.ToMatrix();
            Assert.That(actual.RowCount == rowCount);
            Assert.That(actual.ColCount == colCount);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(actual[row, col] == expected[row, col]);
                }
            }
        }
    }
}
