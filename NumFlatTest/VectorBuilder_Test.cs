using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class VectorBuilder_Test
    {
        [Test]
        public void Create()
        {
            var expected = new int[] { 1, 2, 3 };
            var actual = VectorBuilder.Create<int>(expected).ToArray();
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
