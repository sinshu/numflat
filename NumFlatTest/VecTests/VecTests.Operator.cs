using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class VecTests_Operator
    {
        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void Add(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.RandomDouble(57, count, yStride);

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var destination = x + y;
                var expected = x.Zip(y, (val1, val2) => val1 + val2).ToArray();
                var actual = destination.ToArray();
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
            }
        }

        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(3, 1)]
        [TestCase(3, 3)]
        [TestCase(5, 1)]
        [TestCase(11, 7)]
        public void Add_Scalar(int count, int xStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = new Random(57).NextDouble();

            using (x.EnsureUnchanged())
            {
                var destination = x + y;
                var expected = x.Select(value => value + y).ToArray();
                var actual = destination.ToArray();
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
            }

            using (x.EnsureUnchanged())
            {
                var destination = y + x;
                var expected = x.Select(value => value + y).ToArray();
                var actual = destination.ToArray();
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void Sub(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.RandomDouble(57, count, yStride);

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var destination = x - y;
                var expected = x.Zip(y, (val1, val2) => val1 - val2).ToArray();
                var actual = destination.ToArray();
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
            }
        }

        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(3, 1)]
        [TestCase(3, 3)]
        [TestCase(5, 1)]
        [TestCase(11, 7)]
        public void Sub_Scalar(int count, int xStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = new Random(57).NextDouble();

            using (x.EnsureUnchanged())
            {
                var destination = x - y;
                var expected = x.Select(value => value - y).ToArray();
                var actual = destination.ToArray();
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
            }

            using (x.EnsureUnchanged())
            {
                var destination = y - x;
                var expected = x.Select(value => value - y).ToArray();
                var actual = destination.ToArray();
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
            }
        }

        [TestCase(1, 1, 1.5)]
        [TestCase(1, 3, 2.3)]
        [TestCase(3, 1, 0.1)]
        [TestCase(3, 3, 0.7)]
        [TestCase(5, 1, 3.5)]
        [TestCase(11, 7, 7.9)]
        public void Mul_Scalar(int count, int xStride, double y)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            var expected = x.Select(value => value * y).ToArray();

            using (x.EnsureUnchanged())
            {
                var destination = x * y;
                var actual = destination.ToArray();
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
            }

            using (x.EnsureUnchanged())
            {
                var destination = y * x;
                var actual = destination.ToArray();
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
            }
        }

        [TestCase(1, 1, 1.5)]
        [TestCase(1, 3, 2.3)]
        [TestCase(3, 1, 0.1)]
        [TestCase(3, 3, 0.7)]
        [TestCase(5, 1, 3.5)]
        [TestCase(11, 7, 7.9)]
        public void Div(int count, int xStride, double y)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            using (x.EnsureUnchanged())
            {
                var destination = x / y;
                var expected = x.Select(value => value / y).ToArray();
                var actual = destination.ToArray();
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void Dot_Single(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomSingle(42, count, xStride);
            var y = TestVector.RandomSingle(57, count, yStride);

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var actual = x * y;
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
        public void Dot_Double(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.RandomDouble(57, count, yStride);

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var actual = x * y;
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
        public void Dot_Complex(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomComplex(42, count, xStride);
            var y = TestVector.RandomComplex(57, count, yStride);

            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                var actual = x * y;
                var expected = x.Zip(y, (val1, val2) => val1 * val2).Aggregate((sum, next) => sum + next);
                Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
                Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
            }
        }
    }
}
