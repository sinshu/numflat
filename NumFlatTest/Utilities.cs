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
            for (var postion = 0; postion < memory.Length; postion += stride)
            {
                memory[postion] = NextSingle(random);
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
            for (var postion = 0; postion < memory.Length; postion += stride)
            {
                memory[postion] = NextSingleNonZero(random);
            }

            return new Vec<float>(count, stride, memory);
        }

        public static Vec<double> CreateRandomVectorDouble(int seed, int count, int stride)
        {
            var random = new Random(seed);

            var memory = new double[stride * (count - 1) + 1];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = double.NaN;
            }
            for (var postion = 0; postion < memory.Length; postion += stride)
            {
                memory[postion] = NextDouble(random);
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
            for (var postion = 0; postion < memory.Length; postion += stride)
            {
                memory[postion] = NextDoubleNonZero(random);
            }

            return new Vec<double>(count, stride, memory);
        }

        public static Vec<Complex> CreateRandomVectorComplex(int seed, int count, int stride)
        {
            var random = new Random(seed);

            var memory = new Complex[stride * (count - 1) + 1];
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = Complex.NaN;
            }
            for (var postion = 0; postion < memory.Length; postion += stride)
            {
                memory[postion] = NextComplex(random);
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
            for (var postion = 0; postion < memory.Length; postion += stride)
            {
                memory[postion] = NextComplexNonZero(random);
            }

            return new Vec<Complex>(count, stride, memory);
        }
    }
}
