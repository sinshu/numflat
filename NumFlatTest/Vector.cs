using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            Assert.That(vector.Memory.Length == count);

            for (var i = 0; i < count; i++)
            {
                Assert.That(vector[i] == 0);
            }
        }

        [TestCase(1, 1, new int[] { 1 })]
        [TestCase(3, 1, new int[] { 1, 2, 3 })]
        [TestCase(3, 2, new int[] { 1, -1, 2, -1, 3 })]
        [TestCase(3, 3, new int[] { 1, -1, -1, 2, -1, -1, 3 })]
        [TestCase(2, 3, new int[] { 1, -1, -1, 2 })]
        [TestCase(1, 3, new int[] { 1 })]
        public void Stride(int count, int stride, int[] memory)
        {
            var vector = new Vec<int>(count, stride, memory);

            {
                var expected = 1;
                for (var i = 0; i < count; i++)
                {
                    Assert.That(vector[i] == expected);
                    expected++;
                }
            }

            {
                var expected = 1;
                for (var position = 0; position < memory.Length; position++)
                {
                    if (position % stride == 0)
                    {
                        Assert.That(vector.Memory.Span[position] == expected);
                        expected++;
                    }
                    else
                    {
                        Assert.That(vector.Memory.Span[position] == -1);
                    }
                }
            }
        }

        [TestCase(1, 1, new int[] { 1 })]
        [TestCase(3, 1, new int[] { 1, 2, 3 })]
        [TestCase(3, 2, new int[] { 1, -1, 2, -1, 3 })]
        [TestCase(3, 3, new int[] { 1, -1, -1, 2, -1, -1, 3 })]
        [TestCase(2, 3, new int[] { 1, -1, -1, 2 })]
        [TestCase(1, 3, new int[] { 1 })]
        public void Set(int count, int stride, int[] memory)
        {
            var vector = new Vec<int>(count, stride, memory);

            for (var i = 0; i < count; i++)
            {
                vector[i] = int.MaxValue;
                Assert.That(vector[i] == int.MaxValue);
            }

            for (var position = 0; position < memory.Length; position++)
            {
                if (position % stride == 0)
                {
                    Assert.That(memory[position] == int.MaxValue);
                }
                else
                {
                    Assert.That(memory[position] == -1);
                }
            }
        }

        [TestCase(1, 1, new int[] { 1 })]
        [TestCase(3, 1, new int[] { 1, 2, 3 })]
        [TestCase(3, 2, new int[] { 1, -1, 2, -1, 3 })]
        [TestCase(3, 3, new int[] { 1, -1, -1, 2, -1, -1, 3 })]
        [TestCase(2, 3, new int[] { 1, -1, -1, 2 })]
        [TestCase(1, 3, new int[] { 1 })]
        public void Fill(int count, int stride, int[] memory)
        {
            var vector = new Vec<int>(count, stride, memory);
            vector.Fill(int.MaxValue);
            Assert.That(vector.All(value => value == int.MaxValue));

            for (var position = 0; position < memory.Length; position++)
            {
                if (position % stride == 0)
                {
                    Assert.That(memory[position] == int.MaxValue);
                }
                else
                {
                    Assert.That(memory[position] == -1);
                }
            }
        }

        [TestCase(1, 1, new int[] { 1 })]
        [TestCase(3, 1, new int[] { 1, 2, 3 })]
        [TestCase(3, 2, new int[] { 1, -1, 2, -1, 3 })]
        [TestCase(3, 3, new int[] { 1, -1, -1, 2, -1, -1, 3 })]
        [TestCase(2, 3, new int[] { 1, -1, -1, 2 })]
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
                    Assert.That(memory[position] == -1);
                }
            }
        }

        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(3, 1)]
        [TestCase(10, 2)]
        [TestCase(11, 7)]
        public void Enumerate(int count, int stride)
        {
            var vector = Utilities.CreateRandomVectorDouble(42, count, stride);

            var expected = new double[vector.Count];
            for (var i = 0; i < vector.Count; i++)
            {
                expected[i] = vector[i];
            }

            var actual = vector.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestCase(1, 1, 0, 1)]
        [TestCase(1, 3, 0, 1)]
        [TestCase(3, 1, 0, 2)]
        [TestCase(3, 1, 1, 2)]
        [TestCase(3, 1, 0, 3)]
        [TestCase(3, 3, 0, 2)]
        [TestCase(3, 3, 1, 2)]
        [TestCase(3, 3, 0, 3)]
        [TestCase(11, 7, 3, 5)]
        public void Subvector(int srcCount, int srcStride, int dstIndex, int dstCount)
        {
            var vector = Utilities.CreateRandomVectorDouble(42, srcCount, srcStride);

            var expected = new double[dstCount];
            for (var i = 0; i < dstCount; i++)
            {
                expected[i] = vector[dstIndex + i];
            }

            var actual = vector.Subvector(dstIndex, dstCount).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestCase(1, 1, 0, 1, 1)]
        [TestCase(1, 3, 0, 1, 3)]
        [TestCase(3, 1, 0, 2, 1)]
        [TestCase(3, 1, 1, 2, 1)]
        [TestCase(3, 1, 0, 3, 1)]
        [TestCase(3, 3, 0, 2, 5)]
        [TestCase(3, 3, 1, 2, 3)]
        [TestCase(3, 3, 0, 3, 2)]
        [TestCase(11, 7, 3, 5, 4)]
        public void CopyTo(int dstCount, int dstStride, int subIndex, int subCount, int srcStride)
        {
            var vector = Utilities.CreateRandomVectorDouble(42, dstCount, dstStride);
            var expected = vector.ToArray();

            var subvector = vector.Subvector(subIndex, subCount);
            var source = Utilities.CreateRandomVectorDouble(57, subCount, srcStride);
            source.CopyTo(subvector);
            for (var i = 0; i < subCount; i++)
            {
                expected[subIndex + i] = source[i];
            }
            var actual = vector.ToArray();

            CollectionAssert.AreEqual(subvector, source);
            CollectionAssert.AreEqual(expected, actual);

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
