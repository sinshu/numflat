using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.VecTests
{
    public class BasicTests
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void New(int count)
        {
            var vector = new Vec<int>(count);

            Assert.That(vector.Count, Is.EqualTo(count));
            Assert.That(vector.Stride, Is.EqualTo(1));
            Assert.That(vector.Memory.Length, Is.EqualTo(count));

            for (var i = 0; i < count; i++)
            {
                Assert.That(vector[i], Is.EqualTo(0));
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

            Assert.That(vector.Count, Is.EqualTo(count));
            Assert.That(vector.Stride, Is.EqualTo(stride));
            Assert.That(vector.Memory.Length, Is.EqualTo(stride * (count - 1) + 1));

            var expected = 1;
            for (var i = 0; i < count; i++)
            {
                Assert.That(vector[i], Is.EqualTo(expected));
                expected++;
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
                Assert.That(vector[i], Is.EqualTo(int.MaxValue));
            }

            for (var position = 0; position < memory.Length; position++)
            {
                if (position % stride == 0)
                {
                    Assert.That(memory[position], Is.EqualTo(int.MaxValue));
                }
                else
                {
                    Assert.That(memory[position], Is.EqualTo(-1));
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
                    Assert.That(memory[position], Is.EqualTo(int.MaxValue));
                }
                else
                {
                    Assert.That(memory[position], Is.EqualTo(-1));
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
                    Assert.That(memory[position], Is.EqualTo(0));
                }
                else
                {
                    Assert.That(memory[position], Is.EqualTo(-1));
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
            var vector = TestVector.RandomDouble(42, count, stride);

            var actual = vector.ToArray();

            var expected = new double[vector.Count];
            for (var i = 0; i < vector.Count; i++)
            {
                expected[i] = vector[i];
            }

            Assert.That(actual, Is.EqualTo(expected));
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
            var vector = TestVector.RandomDouble(42, srcCount, srcStride);

            var actual = vector.Subvector(dstIndex, dstCount).ToArray();

            var expected = new double[dstCount];
            for (var i = 0; i < dstCount; i++)
            {
                expected[i] = vector[dstIndex + i];
            }

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 4, 3)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 4, 4)]
        [TestCase(5, 9, 7)]
        [TestCase(7, 9, 8)]
        public void CopyTo(int count, int srcStride, int dstStride)
        {
            var source = TestVector.RandomDouble(42, count, srcStride);
            var destination = TestVector.RandomDouble(0, count, dstStride);

            using (source.EnsureUnchanged())
            {
                source.CopyTo(destination);
            }

            NumAssert.AreSame(source, destination, 0);

            TestVector.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 2)]
        [TestCase(2, 4)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(5, 9)]
        [TestCase(7, 9)]
        public void CopyToSpan(int count, int srcStride)
        {
            var source = TestVector.RandomDouble(42, count, srcStride);
            var destination = new double[source.Count];

            using (source.EnsureUnchanged())
            {
                source.CopyTo(destination);
            }

            NumAssert.AreSame(source, destination.ToVector(), 0);
        }
    }
}
