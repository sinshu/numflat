using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class VectorExtensions
    {
        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void PointwiseMul(int count, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);
            var y = Utilities.CreateRandomVectorDouble(57, count, yStride);
            var destination = x.PointwiseMul(y);

            var expected = x.Zip(y, (val1, val2) => val1 * val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void PointwiseDiv(int count, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);
            var y = Utilities.CreateRandomVectorNonZeroDouble(57, count, yStride);
            var destination = x.PointwiseDiv(y);

            var expected = x.Zip(y, (val1, val2) => val1 / val2).ToArray();
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
            var actual = x.DotProduct(y);

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
            var actual = x.DotProduct(y);

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
            var actual = x.DotProduct(y);

            var expected = x.Zip(y, (val1, val2) => val1 * val2).Aggregate((sum, next) => sum + next);
            Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
            Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(1, 3)]
        [TestCase(2, 5)]
        [TestCase(5, 7)]
        public void Conjugate(int count, int xStride)
        {
            var x = Utilities.CreateRandomVectorComplex(42, count, xStride);
            var destination = x.Conjugate();

            for (var i = 0; i < count; i++)
            {
                Assert.That(x[i].Conjugate() == destination[i]);
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void ConjugateDotProduct(int count, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomVectorComplex(42, count, xStride);
            var y = Utilities.CreateRandomVectorComplex(57, count, yStride);
            var actual = x.ConjugateDotProduct(y);

            var expected = x.Zip(y, (val1, val2) => val1.Conjugate() * val2).Aggregate((sum, next) => sum + next);
            Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
            Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
        }
    }
}
