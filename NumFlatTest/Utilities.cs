using System;
using NumFlat;

namespace NumFlatTest
{
    public static class Utilities
    {
        public static double NextDouble(Random random)
        {
            return 2 * random.NextDouble() - 1;
        }

        public static Vec<double> CreateRandomVector(int seed, int count, int stride)
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
    }
}
