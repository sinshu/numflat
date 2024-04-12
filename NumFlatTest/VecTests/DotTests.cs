using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.VecTests
{
    public class DotTests
    {
        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void Single(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomSingle(42, count, xStride);
            var y = TestVector.RandomSingle(57, count, yStride);

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var actual = x.Dot(y);
                var expected = x.Zip(y, (val1, val2) => val1 * val2).Sum();
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void Double(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.RandomDouble(57, count, yStride);

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var actual = x.Dot(y);
                var expected = x.Zip(y, (val1, val2) => val1 * val2).Sum();
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void Complex(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomComplex(42, count, xStride);
            var y = TestVector.RandomComplex(57, count, yStride);

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var actual = x.Dot(y, false);
                var expected = x.Zip(y, (val1, val2) => val1 * val2).Aggregate((sum, next) => sum + next);
                Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
                Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void ComplexConj(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomComplex(42, count, xStride);
            var y = TestVector.RandomComplex(57, count, yStride);

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var actual = x.Dot(y, true);
                var expected = x.Zip(y, (val1, val2) => val1.Conjugate() * val2).Aggregate((sum, next) => sum + next);
                Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
                Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
            }
        }
    }
}
