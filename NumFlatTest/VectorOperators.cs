using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class VectorOperators
    {
        [TestCase(1, 42, 1, 57, 1)]
        [TestCase(1, 42, 3, 57, 2)]
        [TestCase(3, 42, 1, 57, 1)]
        [TestCase(3, 42, 3, 57, 2)]
        [TestCase(5, 42, 1, 57, 2)]
        [TestCase(11, 42, 7, 57, 2)]
        public void Add(int count, int xSeed, int xStride, int ySeed, int yStride)
        {
            var x = Utilities.CreateRandomVector(xSeed, count, xStride);
            var y = Utilities.CreateRandomVector(ySeed, count, yStride);
            var destination = x + y;

            var expected = x.Zip(y, (val1, val2) => val1 + val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 42, 1, 57, 1)]
        [TestCase(1, 42, 3, 57, 2)]
        [TestCase(3, 42, 1, 57, 1)]
        [TestCase(3, 42, 3, 57, 2)]
        [TestCase(5, 42, 1, 57, 2)]
        [TestCase(11, 42, 7, 57, 2)]
        public void Sub(int count, int xSeed, int xStride, int ySeed, int yStride)
        {
            var x = Utilities.CreateRandomVector(xSeed, count, xStride);
            var y = Utilities.CreateRandomVector(ySeed, count, yStride);
            var destination = x - y;

            var expected = x.Zip(y, (val1, val2) => val1 - val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }
    }
}
