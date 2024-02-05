using System;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public static class TestMatrix
    {
        public static Mat<double> RandomDouble(int seed, int rowCount, int colCount, int stride)
        {
            var random = new Random(seed);

            var memory = new double[stride * (colCount - 1) + rowCount];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = double.NaN;
            }

            for (var col = 0; col < colCount; col++)
            {
                var offset = stride * col;
                for (var i = 0; i < rowCount; i++)
                {
                    var position = offset + i;
                    memory[position] = Utilities.NextDouble(random);
                }
            }

            return new Mat<double>(rowCount, colCount, stride, memory);
        }

        public static void FailIfOutOfRangeWrite(Mat<double> matrix)
        {
            var offset = 0;
            for (var col = 0; col < matrix.ColCount; col++)
            {
                var position = offset;
                for (var row = 0; row < matrix.Stride; row++)
                {
                    if (row < matrix.RowCount)
                    {
                        Assert.That(!double.IsNaN(matrix.Memory.Span[position]));
                    }
                    else if (position < matrix.Memory.Length)
                    {
                        Assert.That(double.IsNaN(matrix.Memory.Span[position]));
                    }
                    position++;
                }
                offset += matrix.Stride;
            }
        }
    }
}
