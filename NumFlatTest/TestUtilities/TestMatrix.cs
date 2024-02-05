using System;
using NUnit.Framework;
using NumFlat;
using System.Numerics;

namespace NumFlatTest
{
    public static class TestMatrix
    {
        public static Mat<float> RandomSingle(int seed, int rowCount, int colCount, int stride)
        {
            var random = new Random(seed);

            var memory = new float[stride * (colCount - 1) + rowCount];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = float.NaN;
            }

            for (var col = 0; col < colCount; col++)
            {
                var offset = stride * col;
                for (var i = 0; i < rowCount; i++)
                {
                    var position = offset + i;
                    memory[position] = Utilities.NextSingle(random);
                }
            }

            return new Mat<float>(rowCount, colCount, stride, memory);
        }

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

        public static Mat<Complex> RandomComplex(int seed, int rowCount, int colCount, int stride)
        {
            var random = new Random(seed);

            var memory = new Complex[stride * (colCount - 1) + rowCount];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = Complex.NaN;
            }

            for (var col = 0; col < colCount; col++)
            {
                var offset = stride * col;
                for (var i = 0; i < rowCount; i++)
                {
                    var position = offset + i;
                    memory[position] = Utilities.NextComplex(random);
                }
            }

            return new Mat<Complex>(rowCount, colCount, stride, memory);
        }

        public static void FailIfOutOfRangeWrite(Mat<float> matrix)
        {
            var offset = 0;
            for (var col = 0; col < matrix.ColCount; col++)
            {
                var position = offset;
                for (var row = 0; row < matrix.Stride; row++)
                {
                    if (row < matrix.RowCount)
                    {
                        Assert.That(!float.IsNaN(matrix.Memory.Span[position]));
                    }
                    else if (position < matrix.Memory.Length)
                    {
                        Assert.That(float.IsNaN(matrix.Memory.Span[position]));
                    }
                    position++;
                }
                offset += matrix.Stride;
            }
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

        public static void FailIfOutOfRangeWrite(Mat<Complex> matrix)
        {
            var offset = 0;
            for (var col = 0; col < matrix.ColCount; col++)
            {
                var position = offset;
                for (var row = 0; row < matrix.Stride; row++)
                {
                    if (row < matrix.RowCount)
                    {
                        Assert.That(!double.IsNaN(matrix.Memory.Span[position].Real));
                        Assert.That(!double.IsNaN(matrix.Memory.Span[position].Imaginary));
                    }
                    else if (position < matrix.Memory.Length)
                    {
                        Assert.That(double.IsNaN(matrix.Memory.Span[position].Real));
                        Assert.That(double.IsNaN(matrix.Memory.Span[position].Imaginary));
                    }
                    position++;
                }
                offset += matrix.Stride;
            }
        }
    }
}
