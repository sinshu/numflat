using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class Vec_Math_Test
    {
        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 3, 2, 4)]
        [TestCase(3, 1, 1, 1)]
        [TestCase(3, 3, 2, 4)]
        [TestCase(5, 1, 2, 3)]
        [TestCase(11, 7, 2, 5)]
        public void Add(int count, int xStride, int yStride, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.RandomDouble(57, count, yStride);

            var expected = x.Zip(y, (val1, val2) => val1 + val2).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.Add(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 3, 2, 4)]
        [TestCase(3, 1, 1, 1)]
        [TestCase(3, 3, 2, 4)]
        [TestCase(5, 1, 2, 3)]
        [TestCase(11, 7, 2, 5)]
        public void Sub(int count, int xStride, int yStride, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.RandomDouble(57, count, yStride);

            var expected = x.Zip(y, (val1, val2) => val1 - val2).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.Sub(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1.5, 1)]
        [TestCase(1, 3, 2.3, 4)]
        [TestCase(3, 1, 0.1, 1)]
        [TestCase(3, 3, 0.7, 4)]
        [TestCase(5, 1, 3.5, 3)]
        [TestCase(11, 7, 7.9, 5)]
        public void Mul(int count, int xStride, double y, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            var expected = x.Select(value => value * y).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Mul(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1.5, 1)]
        [TestCase(1, 3, 2.3, 4)]
        [TestCase(3, 1, 0.1, 1)]
        [TestCase(3, 3, 0.7, 4)]
        [TestCase(5, 1, 3.5, 3)]
        [TestCase(11, 7, 7.9, 5)]
        public void Div(int count, int xStride, double y, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            var expected = x.Select(value => value / y).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Div(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 3, 2, 4)]
        [TestCase(3, 1, 1, 1)]
        [TestCase(3, 3, 2, 4)]
        [TestCase(5, 1, 2, 3)]
        [TestCase(11, 7, 2, 5)]
        public void PointwiseMul(int count, int xStride, int yStride, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.RandomDouble(57, count, yStride);

            var expected = x.Zip(y, (val1, val2) => val1 * val2).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.PointwiseMul(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 3, 2, 4)]
        [TestCase(3, 1, 1, 1)]
        [TestCase(3, 3, 2, 4)]
        [TestCase(5, 1, 2, 3)]
        [TestCase(11, 7, 2, 5)]
        public void PointwiseDiv(int count, int xStride, int yStride, int dstStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.NonZeroRandomDouble(57, count, yStride);

            var expected = x.Zip(y, (val1, val2) => val1 / val2).ToVector();

            var actual = TestVector.RandomDouble(0, count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.PointwiseDiv(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
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
        public void Dot_Double(int count, int xStride, int yStride)
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
        public void Dot_Complex_N(int count, int xStride, int yStride)
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
        public void Dot_Complex_C(int count, int xStride, int yStride)
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

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 2, 1, 3, 4)]
        [TestCase(2, 1, 2, 1, 2)]
        [TestCase(2, 3, 2, 4, 5)]
        [TestCase(3, 1, 3, 1, 3)]
        [TestCase(3, 3, 3, 4, 5)]
        [TestCase(2, 1, 3, 1, 4)]
        [TestCase(2, 3, 3, 4, 5)]
        [TestCase(3, 1, 2, 1, 3)]
        [TestCase(3, 3, 2, 4, 5)]
        public void Outer_Single(int xCount, int xStride, int yCount, int yStride, int dstStride)
        {
            var x = TestVector.RandomSingle(42, xCount, xStride);
            var y = TestVector.RandomSingle(57, yCount, yStride);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var expected = mx.OuterProduct(my);

            var actual = TestMatrix.RandomSingle(0, x.Count, y.Count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.Outer(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-6F);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 2, 1, 3, 4)]
        [TestCase(2, 1, 2, 1, 2)]
        [TestCase(2, 3, 2, 4, 5)]
        [TestCase(3, 1, 3, 1, 3)]
        [TestCase(3, 3, 3, 4, 5)]
        [TestCase(2, 1, 3, 1, 4)]
        [TestCase(2, 3, 3, 4, 5)]
        [TestCase(3, 1, 2, 1, 3)]
        [TestCase(3, 3, 2, 4, 5)]
        public void Outer_Double(int xCount, int xStride, int yCount, int yStride, int dstStride)
        {
            var x = TestVector.RandomDouble(42, xCount, xStride);
            var y = TestVector.RandomDouble(57, yCount, yStride);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var expected = mx.OuterProduct(my);

            var actual = TestMatrix.RandomDouble(0, x.Count, y.Count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.Outer(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 2, 1, 3, 4)]
        [TestCase(2, 1, 2, 1, 2)]
        [TestCase(2, 3, 2, 4, 5)]
        [TestCase(3, 1, 3, 1, 3)]
        [TestCase(3, 3, 3, 4, 5)]
        [TestCase(2, 1, 3, 1, 4)]
        [TestCase(2, 3, 3, 4, 5)]
        [TestCase(3, 1, 2, 1, 3)]
        [TestCase(3, 3, 2, 4, 5)]
        public void Outer_Complex_N(int xCount, int xStride, int yCount, int yStride, int dstStride)
        {
            var x = TestVector.RandomComplex(42, xCount, xStride);
            var y = TestVector.RandomComplex(57, yCount, yStride);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var expected = mx.OuterProduct(my);

            var actual = TestMatrix.RandomComplex(0, x.Count, y.Count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.Outer(x, y, actual, false);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 2, 1, 3, 4)]
        [TestCase(2, 1, 2, 1, 2)]
        [TestCase(2, 3, 2, 4, 5)]
        [TestCase(3, 1, 3, 1, 3)]
        [TestCase(3, 3, 3, 4, 5)]
        [TestCase(2, 1, 3, 1, 4)]
        [TestCase(2, 3, 3, 4, 5)]
        [TestCase(3, 1, 2, 1, 3)]
        [TestCase(3, 3, 2, 4, 5)]
        public void Outer_Complex_C(int xCount, int xStride, int yCount, int yStride, int dstStride)
        {
            var x = TestVector.RandomComplex(42, xCount, xStride);
            var y = TestVector.RandomComplex(57, yCount, yStride);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var expected = mx.OuterProduct(my.Conjugate());

            var actual = TestMatrix.RandomComplex(0, x.Count, y.Count, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Vec.Outer(x, y, actual, true);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(1, 3, 4)]
        [TestCase(2, 5, 4)]
        [TestCase(5, 7, 6)]
        public void Conjugate(int count, int xStride, int dstStride)
        {
            var x = Utilities.CreateRandomVectorComplex(42, count, xStride);

            var expected = x.Select(c => c.Conjugate()).ToVector();

            var actual = Utilities.CreateRandomVectorComplex(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Conjugate(x, actual);
            }

            NumAssert.AreSame(expected, actual, 0);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 4)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 4)]
        [TestCase(5, 1, 3)]
        [TestCase(11, 7, 5)]
        public void Map(int count, int xStride, int dstStride)
        {
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);

            var expected = x.Select(value => new Complex(0, -value)).ToVector();

            var actual = Utilities.CreateRandomVectorComplex(0, count, dstStride);
            using (x.EnsureUnchanged())
            {
                Vec.Map(x, value => new Complex(0, -value), actual);
            }

            NumAssert.AreSame(expected, actual, 0);

            TestVector.FailIfOutOfRangeWrite(actual);
        }
    }
}
