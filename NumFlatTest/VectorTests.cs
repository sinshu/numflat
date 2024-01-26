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
        [TestCase(4)]
        [TestCase(5)]
        public void New(int count)
        {
            var vector = new Vec<int>(count);
            Assert.That(vector.Count == count);
            Assert.That(vector.Stride == 1);
            Assert.IsTrue(vector.Memory.Length == count);
        }
    }
}
