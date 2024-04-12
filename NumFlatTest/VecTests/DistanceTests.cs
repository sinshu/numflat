using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.VecTests
{
    public class DistanceTests
    {
        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 2)]
        [TestCase(2, 1, 1)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 2, 3)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void Single(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomSingle(42, count, xStride);
            var y = TestVector.RandomSingle(57, count, yStride);

            var expected1 = (x - y).Norm();
            var expected2 = expected1 * expected1;

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var actual1 = x.Distance(y);
                var actual2 = x.DistanceSquared(y);
                Assert.That(actual1, Is.EqualTo(expected1).Within(1.0E-6F));
                Assert.That(actual2, Is.EqualTo(expected2).Within(1.0E-6F));
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 2)]
        [TestCase(2, 1, 1)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 2, 3)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void Double(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.RandomDouble(57, count, yStride);

            var expected1 = (x - y).Norm();
            var expected2 = expected1 * expected1;

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var actual1 = x.Distance(y);
                var actual2 = x.DistanceSquared(y);
                Assert.That(actual1, Is.EqualTo(expected1).Within(1.0E-12));
                Assert.That(actual2, Is.EqualTo(expected2).Within(1.0E-12));
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 2)]
        [TestCase(2, 1, 1)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 2, 3)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void Complex(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomComplex(42, count, xStride);
            var y = TestVector.RandomComplex(57, count, yStride);

            var expected1 = (x - y).Norm();
            var expected2 = expected1 * expected1;

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var actual1 = x.Distance(y);
                var actual2 = x.DistanceSquared(y);
                Assert.That(actual1, Is.EqualTo(expected1).Within(1.0E-12));
                Assert.That(actual2, Is.EqualTo(expected2).Within(1.0E-12));
            }
        }
    }
}
