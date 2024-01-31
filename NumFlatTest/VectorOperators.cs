using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class VectorOperators
    {
        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void Add(int count, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);
            var y = Utilities.CreateRandomVectorDouble(57, count, yStride);
            var destination = x + y;

            var expected = x.Zip(y, (val1, val2) => val1 + val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void Sub(int count, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);
            var y = Utilities.CreateRandomVectorDouble(57, count, yStride);
            var destination = x - y;

            var expected = x.Zip(y, (val1, val2) => val1 - val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1.5)]
        [TestCase(1, 3, 2.3)]
        [TestCase(3, 1, 0.1)]
        [TestCase(3, 3, 0.7)]
        [TestCase(5, 1, 3.5)]
        [TestCase(11, 7, 7.9)]
        public void Mul1(int count, int xStride, double y)
        {
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);
            var destination = x * y;

            var expected = x.Select(value => value * y).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1.5)]
        [TestCase(1, 3, 2.3)]
        [TestCase(3, 1, 0.1)]
        [TestCase(3, 3, 0.7)]
        [TestCase(5, 1, 3.5)]
        [TestCase(11, 7, 7.9)]
        public void Mul2(int count, int xStride, double y)
        {
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);
            var destination = y * x;

            var expected = x.Select(value => value * y).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1.5)]
        [TestCase(1, 3, 2.3)]
        [TestCase(3, 1, 0.1)]
        [TestCase(3, 3, 0.7)]
        [TestCase(5, 1, 3.5)]
        [TestCase(11, 7, 7.9)]
        public void Div(int count, int xStride, double y)
        {
            var x = Utilities.CreateRandomVectorNonZeroDouble(42, count, xStride);
            var destination = x / y;

            var expected = x.Select(value => value / y).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void DotProductSingle(int count, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomVectorSingle(42, count, xStride);
            var y = Utilities.CreateRandomVectorSingle(57, count, yStride);
            var actual = x * y;

            var expected = x.Zip(y, (val1, val2) => val1 * val2).Sum();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void DotProductDouble(int count, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);
            var y = Utilities.CreateRandomVectorDouble(57, count, yStride);
            var actual = x * y;

            var expected = x.Zip(y, (val1, val2) => val1 * val2).Sum();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void DotProductComplex(int count, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomVectorComplex(42, count, xStride);
            var y = Utilities.CreateRandomVectorComplex(57, count, yStride);
            var actual = x * y;

            var expected = x.Zip(y, (val1, val2) => val1 * val2).Aggregate((sum, next) => sum + next);
            Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
            Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
        }
    }
}
