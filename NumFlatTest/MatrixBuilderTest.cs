﻿using System;
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
            Assert.That(actual.RowCount == rowCount);
            Assert.That(actual.ColCount == colCount);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(actual[row, col] == expected[row][col]);
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
            Assert.That(actual.RowCount == rowCount);
            Assert.That(actual.ColCount == colCount);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(actual[row, col] == expected[col][row]);
                }
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        public void ToDiagonalMatrix(int count)
        {
            var random = new Random(42);
            var expected = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();

            var actual = expected.ToDiagonalMatrix();
            Assert.That(actual.RowCount == count);
            Assert.That(actual.ColCount == count);

            for (var row = 0; row < count; row++)
            {
                for (var col = 0; col < count; col++)
                {
                    if (row == col)
                    {
                        Assert.That(actual[row, col] == expected[row]);
                    }
                    else
                    {
                        Assert.That(actual[row, col] == 0);
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
        public void ToDiagonalMatrix2(int rowCount, int colCount, int elementCount)
        {
            var random = new Random(42);
            var expected = Enumerable.Range(0, elementCount).Select(i => random.NextDouble()).ToArray();

            var actual = expected.ToDiagonalMatrix(rowCount, colCount);
            Assert.That(actual.RowCount == rowCount);
            Assert.That(actual.ColCount == colCount);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    if (row == col && row < elementCount)
                    {
                        Assert.That(actual[row, col] == expected[row]);
                    }
                    else
                    {
                        Assert.That(actual[row, col] == 0);
                    }
                }
            }
        }
    }
}