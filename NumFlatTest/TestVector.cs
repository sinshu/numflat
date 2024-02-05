using System;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public static class TestVector
    {
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
                memory[position] = Utilities.NextDouble(random);
            }

            return new Vec<double>(count, stride, memory);
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
    }
}
