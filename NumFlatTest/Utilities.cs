using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public static class Utilities
    {
        public static float NextSingle(Random random)
        {
            return 2 * random.NextSingle() - 1;
        }

        public static float NextSingleNonZero(Random random)
        {
            var value = 2 * random.NextSingle() - 1;
            if (value >= 0)
            {
                return value + 1;
            }
            else
            {
                return value - 1;
            }
        }

        public static double NextDouble(Random random)
        {
            return 2 * random.NextDouble() - 1;
        }

        public static double NextDoubleNonZero(Random random)
        {
            var value = 2 * random.NextDouble() - 1;
            if (value >= 0)
            {
                return value + 1;
            }
            else
            {
                return value - 1;
            }
        }

        public static Complex NextComplex(Random random)
        {
            return new Complex(NextDouble(random), NextDouble(random));
        }

        public static Complex NextComplexNonZero(Random random)
        {
            return new Complex(NextDoubleNonZero(random), NextDoubleNonZero(random));
        }

        public static Vec<float> CreateRandomVectorSingle(int seed, int count, int stride)
        {
            var random = new Random(seed);

            var memory = new float[stride * (count - 1) + 1];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = float.NaN;
            }
            for (var position = 0; position < memory.Length; position += stride)
            {
                memory[position] = NextSingle(random);
            }

            return new Vec<float>(count, stride, memory);
        }

        public static Vec<float> CreateRandomVectorNonZeroSingle(int seed, int count, int stride)
        {
            var random = new Random(seed);

            var memory = new float[stride * (count - 1) + 1];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = float.NaN;
            }
            for (var position = 0; position < memory.Length; position += stride)
            {
                memory[position] = NextSingleNonZero(random);
            }

            return new Vec<float>(count, stride, memory);
        }

        public static Mat<float> CreateRandomMatrixSingle(int seed, int rowCount, int colCount, int stride)
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
                    memory[position] = NextSingle(random);
                }
            }

            return new Mat<float>(rowCount, colCount, stride, memory);
        }

        public static Vec<double> CreateRandomVectorDouble(int seed, int count, int stride)
        {
            var random = new Random(seed);

            var memory = new double[stride * (count - 1) + 1];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = double.NaN;
            }
            for (var position = 0; position < memory.Length; position += stride)
            {
                memory[position] = NextDouble(random);
            }

            return new Vec<double>(count, stride, memory);
        }

        public static Vec<double> CreateRandomVectorNonZeroDouble(int seed, int count, int stride)
        {
            var random = new Random(seed);

            var memory = new double[stride * (count - 1) + 1];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = double.NaN;
            }
            for (var position = 0; position < memory.Length; position += stride)
            {
                memory[position] = NextDoubleNonZero(random);
            }

            return new Vec<double>(count, stride, memory);
        }

        public static Mat<double> CreateRandomMatrixDouble(int seed, int rowCount, int colCount, int stride)
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
                    memory[position] = NextDouble(random);
                }
            }

            return new Mat<double>(rowCount, colCount, stride, memory);
        }

        public static Mat<double> CreateRandomMatrixNonZeroDouble(int seed, int rowCount, int colCount, int stride)
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
                    memory[position] = NextDoubleNonZero(random);
                }
            }

            return new Mat<double>(rowCount, colCount, stride, memory);
        }

        public static Vec<Complex> CreateRandomVectorComplex(int seed, int count, int stride)
        {
            var random = new Random(seed);

            var memory = new Complex[stride * (count - 1) + 1];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = Complex.NaN;
            }
            for (var position = 0; position < memory.Length; position += stride)
            {
                memory[position] = NextComplex(random);
            }

            return new Vec<Complex>(count, stride, memory);
        }

        public static Vec<Complex> CreateRandomVectorNonZeroComplex(int seed, int count, int stride)
        {
            var random = new Random(seed);

            var memory = new Complex[stride * (count - 1) + 1];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = double.NaN;
            }
            for (var position = 0; position < memory.Length; position += stride)
            {
                memory[position] = NextComplexNonZero(random);
            }

            return new Vec<Complex>(count, stride, memory);
        }

        public static Mat<Complex> CreateRandomMatrixComplex(int seed, int rowCount, int colCount, int stride)
        {
            var random = new Random(seed);

            var memory = new Complex[stride * (colCount - 1) + rowCount];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = new Complex(double.NaN, double.NaN);
            }
            for (var col = 0; col < colCount; col++)
            {
                var offset = stride * col;
                for (var i = 0; i < rowCount; i++)
                {
                    var position = offset + i;
                    memory[position] = NextComplex(random);
                }
            }

            return new Mat<Complex>(rowCount, colCount, stride, memory);
        }

        public static void FailIfOutOfRangeWrite(Vec<double> vector)
        {
            for (var position = 0; position < vector.Memory.Length; position++)
            {
                if (position % vector.Stride == 0)
                {
                    Assert.That(!double.IsNaN(vector.Memory.Span[position]));
                }
                else
                {
                    Assert.That(double.IsNaN(vector.Memory.Span[position]));
                }
            }
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
