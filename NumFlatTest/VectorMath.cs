﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

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
            var x = Utilities.CreateRandomVectorDouble(xSeed, count, xStride);
            var y = Utilities.CreateRandomVectorDouble(ySeed, count, yStride);
            var destination = Utilities.CreateRandomVectorDouble(0, count, dstStride);
            Vec.Add(x, y, destination);

            var expected = x.Zip(y, (val1, val2) => val1 + val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 42, 1, 57, 1, 1)]
        [TestCase(1, 42, 3, 57, 2, 4)]
        [TestCase(3, 42, 1, 57, 1, 1)]
        [TestCase(3, 42, 3, 57, 2, 4)]
        [TestCase(5, 42, 1, 57, 2, 3)]
        [TestCase(11, 42, 7, 57, 2, 5)]
        public void Sub(int count, int xSeed, int xStride, int ySeed, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomVectorDouble(xSeed, count, xStride);
            var y = Utilities.CreateRandomVectorDouble(ySeed, count, yStride);
            var destination = Utilities.CreateRandomVectorDouble(0, count, dstStride);
            Vec.Sub(x, y, destination);

            var expected = x.Zip(y, (val1, val2) => val1 - val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 42, 1, 1.5, 1)]
        [TestCase(1, 42, 3, 2.3, 4)]
        [TestCase(3, 42, 1, 0.1, 1)]
        [TestCase(3, 42, 3, 0.7, 4)]
        [TestCase(5, 42, 1, 3.5, 3)]
        [TestCase(11, 42, 7, 7.9, 5)]
        public void Mul(int count, int xSeed, int xStride, double y, int dstStride)
        {
            var x = Utilities.CreateRandomVectorDouble(xSeed, count, xStride);
            var destination = Utilities.CreateRandomVectorDouble(0, count, dstStride);
            Vec.Mul(x, y, destination);

            var expected = x.Select(value => value * y).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 42, 1, 57, 1, 1)]
        [TestCase(1, 42, 3, 57, 2, 4)]
        [TestCase(3, 42, 1, 57, 1, 1)]
        [TestCase(3, 42, 3, 57, 2, 4)]
        [TestCase(5, 42, 1, 57, 2, 3)]
        [TestCase(11, 42, 7, 57, 2, 5)]
        public void PointwiseMul(int count, int xSeed, int xStride, int ySeed, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomVectorDouble(xSeed, count, xStride);
            var y = Utilities.CreateRandomVectorDouble(ySeed, count, yStride);
            var destination = Utilities.CreateRandomVectorDouble(0, count, dstStride);
            Vec.PointwiseMul(x, y, destination);

            var expected = x.Zip(y, (val1, val2) => val1 * val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 42, 1, 57, 1, 1)]
        [TestCase(1, 42, 3, 57, 2, 4)]
        [TestCase(3, 42, 1, 57, 1, 1)]
        [TestCase(3, 42, 3, 57, 2, 4)]
        [TestCase(5, 42, 1, 57, 2, 3)]
        [TestCase(11, 42, 7, 57, 2, 5)]
        public void PointwiseDiv(int count, int xSeed, int xStride, int ySeed, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomVectorDouble(xSeed, count, xStride);
            var y = Utilities.CreateRandomVectorNonZeroDouble(ySeed, count, yStride);
            var destination = Utilities.CreateRandomVectorDouble(0, count, dstStride);
            Vec.PointwiseDiv(x, y, destination);

            var expected = x.Zip(y, (val1, val2) => val1 / val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 42, 1, 57, 1, 1)]
        [TestCase(1, 42, 3, 57, 2, 4)]
        [TestCase(3, 42, 1, 57, 1, 1)]
        [TestCase(3, 42, 3, 57, 2, 4)]
        [TestCase(5, 42, 1, 57, 2, 3)]
        [TestCase(11, 42, 7, 57, 2, 5)]
        public void DotProductSingle(int count, int xSeed, int xStride, int ySeed, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomVectorSingle(xSeed, count, xStride);
            var y = Utilities.CreateRandomVectorSingle(ySeed, count, yStride);
            var actual = Vec.InnerProduct(x, y);

            var expected = x.Zip(y, (val1, val2) => val1 * val2).Sum();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
        }

        [TestCase(1, 42, 1, 57, 1, 1)]
        [TestCase(1, 42, 3, 57, 2, 4)]
        [TestCase(3, 42, 1, 57, 1, 1)]
        [TestCase(3, 42, 3, 57, 2, 4)]
        [TestCase(5, 42, 1, 57, 2, 3)]
        [TestCase(11, 42, 7, 57, 2, 5)]
        public void DotProductDouble(int count, int xSeed, int xStride, int ySeed, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomVectorDouble(xSeed, count, xStride);
            var y = Utilities.CreateRandomVectorDouble(ySeed, count, yStride);
            var actual = Vec.InnerProduct(x, y);

            var expected = x.Zip(y, (val1, val2) => val1 * val2).Sum();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 42, 1, 57, 1, 1)]
        [TestCase(1, 42, 3, 57, 2, 4)]
        [TestCase(3, 42, 1, 57, 1, 1)]
        [TestCase(3, 42, 3, 57, 2, 4)]
        [TestCase(5, 42, 1, 57, 2, 3)]
        [TestCase(11, 42, 7, 57, 2, 5)]
        public void DotProductComplex(int count, int xSeed, int xStride, int ySeed, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomVectorComplex(xSeed, count, xStride);
            var y = Utilities.CreateRandomVectorComplex(ySeed, count, yStride);
            var actual = Vec.InnerProduct(x, y);

            var expected = x.Zip(y, (val1, val2) => val1 * val2).Aggregate((sum, next) => sum + next);
            Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
            Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
        }
    }
}
