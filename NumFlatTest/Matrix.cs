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

        [TestCase(1, 1, 1, new int[] { 1 })]
        [TestCase(1, 1, 3, new int[] { 1 })]
        [TestCase(2, 2, 2, new int[] { 1, 2, 3, 4 })]
        [TestCase(2, 2, 3, new int[] { 1, 2, -1, 3, 4 })]
        [TestCase(3, 3, 3, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })]
        [TestCase(3, 3, 5, new int[] { 1, 2, 3, -1, -1, 4, 5, 6, -1, -1, 7, 8, 9 })]
        [TestCase(1, 3, 1, new int[] { 1, 2, 3 })]
        [TestCase(1, 3, 5, new int[] { 1, -1, -1, -1, -1, 2, -1, -1, -1, -1, 3 })]
        [TestCase(3, 1, 3, new int[] { 1, 2, 3 })]
        [TestCase(3, 1, 7, new int[] { 1, 2, 3 })]
        [TestCase(2, 3, 2, new int[] { 1, 2, 3, 4, 5, 6 })]
        [TestCase(2, 3, 4, new int[] { 1, 2, -1, -1, 3, 4, -1, -1, 5, 6 })]
        [TestCase(3, 2, 3, new int[] { 1, 2, 3, 4, 5, 6 })]
        [TestCase(3, 2, 6, new int[] { 1, 2, 3, -1, -1, -1, 4, 5, 6 })]
        public void Stride(int rowCount, int colCount, int stride, int[] memory)
        {
            var matrix = new Mat<int>(rowCount, colCount, stride, memory);

            {
                var expected = 1;
                for (var col = 0; col < colCount; col++)
                {
                    for (var row = 0; row < rowCount; row++)
                    {
                        Assert.That(matrix[row, col] == expected);
                        expected++;
                    }
                }
            }

            {
                var offset = 0;
                var expected = 1;
                for (var col = 0; col < colCount; col++)
                {
                    var position = offset;
                    for (var row = 0; row < rowCount; row++)
                    {
                        Assert.That(matrix.Memory.Span[position] == expected);
                        expected++;
                        position++;
                    }
                    offset += stride;
                }
            }
        }

        [TestCase(1, 1, 1, new int[] { 1 })]
        [TestCase(1, 1, 3, new int[] { 1 })]
        [TestCase(2, 2, 2, new int[] { 1, 2, 3, 4 })]
        [TestCase(2, 2, 3, new int[] { 1, 2, -1, 3, 4 })]
        [TestCase(3, 3, 3, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })]
        [TestCase(3, 3, 5, new int[] { 1, 2, 3, -1, -1, 4, 5, 6, -1, -1, 7, 8, 9 })]
        [TestCase(1, 3, 1, new int[] { 1, 2, 3 })]
        [TestCase(1, 3, 5, new int[] { 1, -1, -1, -1, -1, 2, -1, -1, -1, -1, 3 })]
        [TestCase(3, 1, 3, new int[] { 1, 2, 3 })]
        [TestCase(3, 1, 7, new int[] { 1, 2, 3 })]
        [TestCase(2, 3, 2, new int[] { 1, 2, 3, 4, 5, 6 })]
        [TestCase(2, 3, 4, new int[] { 1, 2, -1, -1, 3, 4, -1, -1, 5, 6 })]
        [TestCase(3, 2, 3, new int[] { 1, 2, 3, 4, 5, 6 })]
        [TestCase(3, 2, 6, new int[] { 1, 2, 3, -1, -1, -1, 4, 5, 6 })]
        public void Set(int rowCount, int colCount, int stride, int[] memory)
        {
            var matrix = new Mat<int>(rowCount, colCount, stride, memory);

            for (var col = 0; col < colCount; col++)
            {
                for (var row = 0; row < rowCount; row++)
                {
                    matrix[row, col] = int.MaxValue;
                    Assert.That(matrix[row, col] == int.MaxValue);
                }
            }

            var offset = 0;
            for (var col = 0; col < colCount; col++)
            {
                var position = offset;
                for (var row = 0; row < stride; row++)
                {
                    if (row < rowCount)
                    {
                        Assert.That(matrix.Memory.Span[position] == int.MaxValue);
                    }
                    else if (position < memory.Length)
                    {
                        Assert.That(matrix.Memory.Span[position] == -1);
                    }
                    position++;
                }
                offset += stride;
            }
        }
    }
}
