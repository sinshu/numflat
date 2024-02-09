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
        [TestCase(1, 1, 555)]
        [TestCase(2, 2, 666)]
        [TestCase(2, 3, 777)]
        [TestCase(3, 2, 888)]
        [TestCase(5, 3, 999)]
        [TestCase(3, 5, 101010)]
        public void Fill(int rowCOunt, int colCount, int value)
        {
            var mat = MatrixBuilder.Fill(rowCOunt, colCount, value);

            for (var row = 0; row < rowCOunt; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(mat[row, col], Is.EqualTo(value));
                }
            }
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 2)]
        [TestCase(5, 3)]
        [TestCase(3, 5)]
        public void FromFunc(int rowCOunt, int colCount)
        {
            var mat = MatrixBuilder.FromFunc(rowCOunt, colCount, (row, col) => 10 * row + col);

            for (var row = 0; row < rowCOunt; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    var value = 10 * row + col;
                    Assert.That(mat[row, col], Is.EqualTo(value));
                }
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(2)]
        [TestCase(5)]
        public void Identity(int count)
        {
            var identity = MatrixBuilder.Identity<double>(count);

            for (var row = 0; row < count; row++)
            {
                for (var col = 0; col < count; col++)
                {
                    if (row == col)
                    {
                        Assert.That(identity[row, col], Is.EqualTo(1));
                    }
                    else
                    {
                        Assert.That(identity[row, col], Is.EqualTo(0));
                    }
                }
            }
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 2)]
        [TestCase(5, 3)]
        [TestCase(3, 5)]
        public void Identity(int rowCOunt, int colCount)
        {
            var identity = MatrixBuilder.Identity<double>(rowCOunt, colCount);

            for (var row = 0; row < rowCOunt; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    if (row == col)
                    {
                        Assert.That(identity[row, col], Is.EqualTo(1));
                    }
                    else
                    {
                        Assert.That(identity[row, col], Is.EqualTo(0));
                    }
                }
            }
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(2, 4)]
        [TestCase(5, 3)]
        public void ToMatrix(int rowCount, int colCount)
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

            Assert.That(actual.RowCount, Is.EqualTo(rowCount));
            Assert.That(actual.ColCount, Is.EqualTo(colCount));

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(actual[row, col], Is.EqualTo(expected[row, col]));
                }
            }
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(2, 4)]
        [TestCase(5, 3)]
        public void RowsToMatrix(int rowCount, int colCount)
        {
            var random = new Random(42);

            var expected = new List<double[]>();
            for (var row = 0; row < rowCount; row++)
            {
                var data = Enumerable.Range(0, colCount).Select(i => random.NextDouble()).ToArray();
                expected.Add(data);
            }

            var actual = expected.RowsToMatrix();

            Assert.That(actual.RowCount, Is.EqualTo(rowCount));
            Assert.That(actual.ColCount, Is.EqualTo(colCount));

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(actual[row, col], Is.EqualTo(expected[row][col]));
                }
            }
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(2, 4)]
        [TestCase(5, 3)]
        public void ColsToMatrix(int rowCount, int colCount)
        {
            var random = new Random(42);

            var expected = new List<double[]>();
            for (var col = 0; col < colCount; col++)
            {
                var data = Enumerable.Range(0, rowCount).Select(i => random.NextDouble()).ToArray();
                expected.Add(data);
            }

            var actual = expected.ColsToMatrix();

            Assert.That(actual.RowCount, Is.EqualTo(rowCount));
            Assert.That(actual.ColCount, Is.EqualTo(colCount));

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(actual[row, col], Is.EqualTo(expected[col][row]));
                }
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        public void ToDiagonalMatrix_NoArg(int count)
        {
            var random = new Random(42);

            var expected = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();
            var actual = expected.ToDiagonalMatrix();

            Assert.That(actual.RowCount, Is.EqualTo(count));
            Assert.That(actual.ColCount, Is.EqualTo(count));

            for (var row = 0; row < count; row++)
            {
                for (var col = 0; col < count; col++)
                {
                    if (row == col)
                    {
                        Assert.That(actual[row, col], Is.EqualTo(expected[row]));
                    }
                    else
                    {
                        Assert.That(actual[row, col], Is.EqualTo(0));
                    }
                }
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 2, 2)]
        [TestCase(2, 3, 2)]
        [TestCase(5, 4, 3)]
        [TestCase(4, 5, 2)]
        [TestCase(4, 3, 0)]
        public void ToDiagonalMatrix_OneArg(int rowCount, int colCount, int elementCount)
        {
            var random = new Random(42);

            var expected = Enumerable.Range(0, elementCount).Select(i => random.NextDouble()).ToArray();
            var actual = expected.ToDiagonalMatrix(rowCount, colCount);

            Assert.That(actual.RowCount, Is.EqualTo(rowCount));
            Assert.That(actual.ColCount, Is.EqualTo(colCount));

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    if (row == col && row < elementCount)
                    {
                        Assert.That(actual[row, col], Is.EqualTo(expected[row]));
                    }
                    else
                    {
                        Assert.That(actual[row, col], Is.EqualTo(0));
                    }
                }
            }
        }
    }
}
