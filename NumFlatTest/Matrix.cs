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
        public void Fill(int rowCount, int colCount, int stride, int[] memory)
        {
            var matrix = new Mat<int>(rowCount, colCount, stride, memory);
            matrix.Fill(int.MaxValue);
            for (var col = 0; col < colCount; col++)
            {
                for (var row = 0; row < rowCount; row++)
                {
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
        public void Clear(int rowCount, int colCount, int stride, int[] memory)
        {
            var matrix = new Mat<int>(rowCount, colCount, stride, memory);
            matrix.Clear();
            for (var col = 0; col < colCount; col++)
            {
                for (var row = 0; row < rowCount; row++)
                {
                    Assert.That(matrix[row, col] == 0);
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
                        Assert.That(matrix.Memory.Span[position] == 0);
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

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 3, 5)]
        [TestCase(1, 3, 1)]
        [TestCase(1, 3, 5)]
        [TestCase(3, 1, 3)]
        [TestCase(3, 1, 7)]
        [TestCase(2, 3, 2)]
        [TestCase(2, 3, 4)]
        [TestCase(3, 2, 3)]
        [TestCase(3, 2, 6)]
        public void Cols(int rowCount, int colCount, int stride)
        {
            var matrix = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, stride);

            Assert.That(matrix.Cols.Count == colCount);

            for (var col = 0; col < colCount; col++)
            {
                var expected = Enumerable.Range(0, rowCount).Select(row => matrix[row, col]).ToArray();
                var actual = matrix.Cols[col].ToArray();
                Assert.That(actual, Is.EqualTo(expected));
            }

            var cols = matrix.Cols.ToArray();
            for (var col = 0; col < colCount; col++)
            {
                var expected = cols[col];
                var actual = matrix.Cols[col].ToArray();
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 3, 5)]
        [TestCase(1, 3, 1)]
        [TestCase(1, 3, 5)]
        [TestCase(3, 1, 3)]
        [TestCase(3, 1, 7)]
        [TestCase(2, 3, 2)]
        [TestCase(2, 3, 4)]
        [TestCase(3, 2, 3)]
        [TestCase(3, 2, 6)]
        public void Rows(int rowCount, int colCount, int stride)
        {
            var matrix = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, stride);

            Assert.That(matrix.Rows.Count == rowCount);

            for (var row = 0; row < rowCount; row++)
            {
                var expected = Enumerable.Range(0, colCount).Select(col => matrix[row, col]).ToArray();
                var actual = matrix.Rows[row].ToArray();
                Assert.That(actual, Is.EqualTo(expected));
            }

            var rows = matrix.Rows.ToArray();
            for (var row = 0; row < rowCount; row++)
            {
                var expected = rows[row];
                var actual = matrix.Rows[row].ToArray();
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [TestCase(1, 1, 1, 0, 0, 1, 1)]
        [TestCase(1, 1, 3, 0, 0, 1, 1)]
        [TestCase(2, 2, 2, 0, 0, 2, 2)]
        [TestCase(2, 2, 3, 0, 0, 2, 2)]
        [TestCase(2, 2, 3, 1, 1, 1, 1)]
        [TestCase(3, 1, 3, 1, 0, 2, 1)]
        [TestCase(3, 1, 7, 1, 0, 2, 1)]
        [TestCase(1, 3, 1, 0, 2, 1, 1)]
        [TestCase(1, 3, 6, 0, 1, 1, 2)]
        [TestCase(3, 4, 5, 1, 2, 1, 2)]
        [TestCase(3, 4, 7, 0, 2, 1, 2)]
        [TestCase(3, 4, 7, 1, 0, 1, 2)]
        [TestCase(8, 7, 9, 3, 1, 4, 5)]
        [TestCase(7, 8, 9, 1, 3, 5, 4)]
        public void Submatrix(int srcRowCount, int srcColCount, int srcStride, int dstStartRow, int dstStartCol, int dstRowCount, int dstColCount)
        {
            var matrix = Utilities.CreateRandomMatrixDouble(42, srcRowCount, srcColCount, srcStride);

            var expected = new double[dstRowCount, dstColCount];
            for (var row = 0; row < dstRowCount; row++)
            {
                for (var col = 0; col < dstColCount; col++)
                {
                    expected[row, col] = matrix[dstStartRow + row, dstStartCol + col];
                }
            }

            var submatrix = matrix.Submatrix(dstStartRow, dstStartCol, dstRowCount, dstColCount);
            var actual = new double[submatrix.RowCount, submatrix.ColCount];
            for (var row = 0; row < dstRowCount; row++)
            {
                for (var col = 0; col < dstColCount; col++)
                {
                    actual[row, col] = submatrix[row, col];
                }
            }

            for (var row = 0; row < dstRowCount; row++)
            {
                for (var col = 0; col < dstColCount; col++)
                {
                    Assert.That(expected[row, col] == actual[row, col]);
                }
            }
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 3, 4)]
        [TestCase(3, 3, 3, 3)]
        [TestCase(3, 3, 5, 7)]
        [TestCase(4, 3, 5, 7)]
        [TestCase(8, 9, 11, 10)]
        public void CopyTo(int rowCount, int colCount, int srcStride, int dstStride)
        {
            var source = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, srcStride);
            var destination = Utilities.CreateRandomMatrixDouble(57, rowCount, colCount, dstStride);

            source.CopyTo(destination);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(source[row, col] == destination[row, col]);
                }
            }

            var offset = 0;
            for (var col = 0; col < colCount; col++)
            {
                var position = offset;
                for (var row = 0; row < dstStride; row++)
                {
                    if (row < rowCount)
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

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 3, 5)]
        [TestCase(1, 3, 1)]
        [TestCase(1, 3, 5)]
        [TestCase(3, 1, 3)]
        [TestCase(3, 1, 7)]
        [TestCase(2, 3, 2)]
        [TestCase(2, 3, 4)]
        [TestCase(3, 2, 3)]
        [TestCase(3, 2, 6)]
        public void ToArray(int rowCount, int colCount, int stride)
        {
            var matrix = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, stride);
            var array = matrix.ToArray();

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(array[row, col] == matrix[row, col]);
                }
            }
        }
    }
}
