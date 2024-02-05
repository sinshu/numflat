using System;
using NUnit.Framework;
using NumFlat;
using System.Numerics;

namespace NumFlatTest
{
    public static class TestVector
    {
        public static Vec<float> RandomSingle(int seed, int count, int stride)
        {
            var random = new Random(seed);

            var memory = new float[stride * (count - 1) + 1];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = float.NaN;
            }

            for (var position = 0; position < memory.Length; position += stride)
            {
                memory[position] = Utility.NextSingle(random);
            }

            return new Vec<float>(count, stride, memory);
        }

        public static Vec<float> NonZeroRandomSingle(int seed, int count, int stride)
        {
            var random = new Random(seed);

            var memory = new float[stride * (count - 1) + 1];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = float.NaN;
            }

            for (var position = 0; position < memory.Length; position += stride)
            {
                memory[position] = Utility.NextSingleNonZero(random);
            }

            return new Vec<float>(count, stride, memory);
        }

        public static Vec<double> RandomDouble(int seed, int count, int stride)
        {
            var random = new Random(seed);

            var memory = new double[stride * (count - 1) + 1];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = double.NaN;
            }

            for (var position = 0; position < memory.Length; position += stride)
            {
                memory[position] = Utility.NextDouble(random);
            }

            return new Vec<double>(count, stride, memory);
        }

        public static Vec<double> NonZeroRandomDouble(int seed, int count, int stride)
        {
            var random = new Random(seed);

            var memory = new double[stride * (count - 1) + 1];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = double.NaN;
            }

            for (var position = 0; position < memory.Length; position += stride)
            {
                memory[position] = Utility.NextDoubleNonZero(random);
            }

            return new Vec<double>(count, stride, memory);
        }

        public static Vec<Complex> RandomComplex(int seed, int count, int stride)
        {
            var random = new Random(seed);

            var memory = new Complex[stride * (count - 1) + 1];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = Complex.NaN;
            }

            for (var position = 0; position < memory.Length; position += stride)
            {
                memory[position] = Utility.NextComplex(random);
            }

            return new Vec<Complex>(count, stride, memory);
        }

        public static Vec<Complex> NonZeroRandomComplex(int seed, int count, int stride)
        {
            var random = new Random(seed);

            var memory = new Complex[stride * (count - 1) + 1];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = Complex.NaN;
            }

            for (var position = 0; position < memory.Length; position += stride)
            {
                memory[position] = Utility.NextDoubleNonZero(random);
            }

            return new Vec<Complex>(count, stride, memory);
        }

        public static void FailIfOutOfRangeWrite(Vec<float> vector)
        {
            for (var position = 0; position < vector.Memory.Length; position++)
            {
                if (position % vector.Stride == 0)
                {
                    Assert.That(!float.IsNaN(vector.Memory.Span[position]));
                }
                else
                {
                    Assert.That(float.IsNaN(vector.Memory.Span[position]));
                }
            }
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

        public static void FailIfOutOfRangeWrite(Vec<Complex> vector)
        {
            for (var position = 0; position < vector.Memory.Length; position++)
            {
                if (position % vector.Stride == 0)
                {
                    Assert.That(!double.IsNaN(vector.Memory.Span[position].Real));
                    Assert.That(!double.IsNaN(vector.Memory.Span[position].Imaginary));
                }
                else
                {
                    Assert.That(double.IsNaN(vector.Memory.Span[position].Real));
                    Assert.That(double.IsNaN(vector.Memory.Span[position].Imaginary));
                }
            }
        }
    }
}
