using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.VecTests
{
    public class VectorBuilderTests
    {
        [Test]
        public void Create()
        {
            var expected = new int[] { 1, 2, 3 };
            var actual = VectorBuilder.Create<int>(expected).ToArray();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Fill()
        {
            var expected = new int[] { 42, 42, 42 };
            var actual = VectorBuilder.Fill(3, 42);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void FromFunc()
        {
            var expected = new int[] { 1, 4, 9 };
            var actual = VectorBuilder.FromFunc(3, i => (i + 1) * (i + 1));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ToVector()
        {
            var expected = new int[] { 1, 2, 3 };
            var actual = expected.ToVector().ToArray();
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
