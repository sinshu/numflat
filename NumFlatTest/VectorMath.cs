using System;
using NUnit.Framework;
using NumFlat;
using System.Linq;

namespace NumFlatTest
{
    public class VectorMath
    {
        [TestCase(1, 42, 1, 57, 1, 1)]
        [TestCase(1, 42, 3, 57, 2, 4)]
        [TestCase(3, 42, 1, 57, 1, 1)]
        [TestCase(3, 42, 3, 57, 2, 4)]
        [TestCase(5, 42, 1, 57, 2, 3)]
        [TestCase(11, 42, 7, 57, 2, 5)]
        public void Add(int count, int xSeed, int xStride, int ySeed, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomVector(xSeed, count, xStride);
            var y = Utilities.CreateRandomVector(ySeed, count, yStride);
            var destination = new Vec<double>(count, dstStride, new double[dstStride * (count - 1) + 1]);
            Vec.Add(x, y, destination);

            var expected = x.Zip(y, (val1, val2) => val1 + val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 42, 1, 57, 1, 1)]
        [TestCase(1, 42, 3, 57, 2, 4)]
        [TestCase(3, 42, 1, 57, 1, 1)]
        [TestCase(3, 42, 3, 57, 2, 4)]
        [TestCase(5, 42, 1, 57, 2, 3)]
        [TestCase(11, 42, 7, 57, 2, 5)]
        public void Sub(int count, int xSeed, int xStride, int ySeed, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomVector(xSeed, count, xStride);
            var y = Utilities.CreateRandomVector(ySeed, count, yStride);
            var destination = new Vec<double>(count, dstStride, new double[dstStride * (count - 1) + 1]);
            Vec.Sub(x, y, destination);

            var expected = x.Zip(y, (val1, val2) => val1 - val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 42, 1, 57, 1, 1)]
        [TestCase(1, 42, 3, 57, 2, 4)]
        [TestCase(3, 42, 1, 57, 1, 1)]
        [TestCase(3, 42, 3, 57, 2, 4)]
        [TestCase(5, 42, 1, 57, 2, 3)]
        [TestCase(11, 42, 7, 57, 2, 5)]
        public void ElemMul(int count, int xSeed, int xStride, int ySeed, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomVector(xSeed, count, xStride);
            var y = Utilities.CreateRandomVector(ySeed, count, yStride);
            var destination = new Vec<double>(count, dstStride, new double[dstStride * (count - 1) + 1]);
            Vec.PointwiseMul(x, y, destination);

            var expected = x.Zip(y, (val1, val2) => val1 * val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 42, 1, 57, 1, 1)]
        [TestCase(1, 42, 3, 57, 2, 4)]
        [TestCase(3, 42, 1, 57, 1, 1)]
        [TestCase(3, 42, 3, 57, 2, 4)]
        [TestCase(5, 42, 1, 57, 2, 3)]
        [TestCase(11, 42, 7, 57, 2, 5)]
        public void ElemDiv(int count, int xSeed, int xStride, int ySeed, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomVector(xSeed, count, xStride);
            var y = Utilities.CreateRandomVectorNonZero(ySeed, count, yStride);
            var destination = new Vec<double>(count, dstStride, new double[dstStride * (count - 1) + 1]);
            Vec.PointwiseDiv(x, y, destination);

            var expected = x.Zip(y, (val1, val2) => val1 / val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }
    }
}
