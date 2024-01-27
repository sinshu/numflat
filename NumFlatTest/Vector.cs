using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class Vector
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

            for (var position = 0; position < memory.Length; position++)
            {
                if (position % stride == 0)
                {
                    Assert.That(memory[position] == -1);
                }
                else
                {
                    Assert.That(memory[position] != -1);
                }
            }
        }

        [TestCase(1, 1, new int[] { 1 })]
        [TestCase(3, 1, new int[] { 1, 2, 3 })]
        [TestCase(3, 2, new int[] { 1, 2, 3, 4, 5 })]
        [TestCase(3, 3, new int[] { 1, 2, 3, 4, 5, 6, 7 })]
        [TestCase(2, 3, new int[] { 1, 2, 3, 4 })]
        [TestCase(1, 3, new int[] { 1 })]
        public void Fill(int count, int stride, int[] memory)
        {
            var vector = new Vec<int>(count, stride, memory);
            vector.Fill(-1);
            Assert.That(vector.All(value => value == -1));

            for (var position = 0; position < memory.Length; position++)
            {
                if (position % stride == 0)
                {
                    Assert.That(memory[position] == -1);
                }
                else
                {
                    Assert.That(memory[position] != -1);
                }
            }
        }

        [TestCase(1, 1, new int[] { 1 })]
        [TestCase(3, 1, new int[] { 1, 2, 3 })]
        [TestCase(3, 2, new int[] { 1, 2, 3, 4, 5 })]
        [TestCase(3, 3, new int[] { 1, 2, 3, 4, 5, 6, 7 })]
        [TestCase(2, 3, new int[] { 1, 2, 3, 4 })]
        [TestCase(1, 3, new int[] { 1 })]
        public void Clear(int count, int stride, int[] memory)
        {
            var vector = new Vec<int>(count, stride, memory);
            vector.Clear();
            Assert.That(vector.All(value => value == 0));

            for (var position = 0; position < memory.Length; position++)
            {
                if (position % stride == 0)
                {
                    Assert.That(memory[position] == 0);
                }
                else
                {
                    Assert.That(memory[position] != 0);
                }
            }
        }

        [TestCase(42, 1, 1)]
        [TestCase(42, 1, 3)]
        [TestCase(42, 3, 1)]
        [TestCase(42, 10, 2)]
        [TestCase(42, 11, 7)]
        public void Enumerate(int seed, int count, int stride)
        {
            var vector = Utilities.CreateRandomVectorDouble(seed, count, stride);

            var expected = new double[vector.Count];
            for (var i = 0; i < vector.Count; i++)
            {
                expected[i] = vector[i];
            }

            var actual = vector.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestCase(42, 1, 1, 0, 1)]
        [TestCase(42, 1, 3, 0, 1)]
        [TestCase(42, 3, 1, 0, 2)]
        [TestCase(42, 3, 1, 1, 2)]
        [TestCase(42, 3, 1, 0, 3)]
        [TestCase(42, 3, 3, 0, 2)]
        [TestCase(42, 3, 3, 1, 2)]
        [TestCase(42, 3, 3, 0, 3)]
        [TestCase(42, 11, 7, 3, 5)]
        public void Subvector(int seed, int srcCount, int srcStride, int dstIndex, int dstCount)
        {
            var vector = Utilities.CreateRandomVectorDouble(seed, srcCount, srcStride);

            var expected = new double[dstCount];
            for (var i = 0; i < dstCount; i++)
            {
                expected[i] = vector[dstIndex + i];
            }

            var actual = vector.Subvector(dstIndex, dstCount).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
