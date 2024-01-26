using System;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class VectorTests
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void New(int count)
        {
            var vector = new Vec<int>(count);

            Assert.That(vector.Count == count);
            Assert.That(vector.Stride == 1);
            Assert.IsTrue(vector.Memory.Length == count);

            for (var vectorIndex = 0; vectorIndex < count; vectorIndex++)
            {
                Assert.That(vector[vectorIndex] == 0);
            }
        }

        [TestCase(1, 1, new int[] { 1 })]
        [TestCase(3, 1, new int[] { 1, 2, 3 })]
        [TestCase(3, 2, new int[] { 1, 2, 3, 4, 5 })]
        [TestCase(3, 3, new int[] { 1, 2, 3, 4, 5, 6, 7 })]
        [TestCase(2, 3, new int[] { 1, 2, 3, 4 })]
        [TestCase(1, 3, new int[] { 1 })]
        public void Stride(int count, int stride, int[] memory)
        {
            var vector = new Vec<int>(count, stride, memory);

            var memoryIndex = 0;
            for (var vectorIndex = 0; vectorIndex < count; vectorIndex++)
            {
                Assert.That(vector[vectorIndex] == memory[memoryIndex]);
                memoryIndex += stride;
            }
        }

        [TestCase(1, 1, new int[] { 1 })]
        [TestCase(3, 1, new int[] { 1, 2, 3 })]
        [TestCase(3, 2, new int[] { 1, 2, 3, 4, 5 })]
        [TestCase(3, 3, new int[] { 1, 2, 3, 4, 5, 6, 7 })]
        [TestCase(2, 3, new int[] { 1, 2, 3, 4 })]
        [TestCase(1, 3, new int[] { 1 })]
        public void Set(int count, int stride, int[] memory)
        {
            var vector = new Vec<int>(count, stride, memory);

            for (var vectorIndex = 0; vectorIndex < count; vectorIndex++)
            {
                vector[vectorIndex] = -1;
                Assert.That(vector[vectorIndex] == -1);
            }
        }
    }
}
