using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class VectorExtension_Test
    {
        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void PointwiseMul(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.RandomDouble(57, count, yStride);

            var expected = x.Zip(y, (val1, val2) => val1 * val2).ToVector();

            Vec<double> actual;
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                actual = x.PointwiseMul(y);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void PointwiseDiv(int count, int xStride, int yStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);
            var y = TestVector.NonZeroRandomDouble(57, count, yStride);

            var expected = x.Zip(y, (val1, val2) => val1 / val2).ToVector();

            Vec<double> actual;
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                actual = x.PointwiseDiv(y);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 2, 1, 3)]
        [TestCase(2, 1, 2, 1)]
        [TestCase(2, 3, 2, 4)]
        [TestCase(3, 1, 3, 1)]
        [TestCase(3, 3, 3, 4)]
        [TestCase(2, 1, 3, 1)]
        [TestCase(2, 3, 3, 4)]
        [TestCase(3, 1, 2, 1)]
        [TestCase(3, 3, 2, 4)]
        public void Outer_Single(int xCount, int xStride, int yCount, int yStride)
        {
            var x = TestVector.RandomSingle(42, xCount, xStride);
            var y = TestVector.RandomSingle(57, yCount, yStride);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var expected = mx.OuterProduct(my);

            Mat<float> actual;
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                actual = x.Outer(y);
            }

            NumAssert.AreSame(expected, actual, 1.0E-6F);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 2, 1, 3)]
        [TestCase(2, 1, 2, 1)]
        [TestCase(2, 3, 2, 4)]
        [TestCase(3, 1, 3, 1)]
        [TestCase(3, 3, 3, 4)]
        [TestCase(2, 1, 3, 1)]
        [TestCase(2, 3, 3, 4)]
        [TestCase(3, 1, 2, 1)]
        [TestCase(3, 3, 2, 4)]
        public void Outer_Double(int xCount, int xStride, int yCount, int yStride)
        {
            var x = TestVector.RandomDouble(42, xCount, xStride);
            var y = TestVector.RandomDouble(57, yCount, yStride);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var expected = mx.OuterProduct(my);

            Mat<double> actual;
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                actual = x.Outer(y);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 2, 1, 3)]
        [TestCase(2, 1, 2, 1)]
        [TestCase(2, 3, 2, 4)]
        [TestCase(3, 1, 3, 1)]
        [TestCase(3, 3, 3, 4)]
        [TestCase(2, 1, 3, 1)]
        [TestCase(2, 3, 3, 4)]
        [TestCase(3, 1, 2, 1)]
        [TestCase(3, 3, 2, 4)]
        public void Outer_Complex_N(int xCount, int xStride, int yCount, int yStride)
        {
            var x = TestVector.RandomComplex(42, xCount, xStride);
            var y = TestVector.RandomComplex(57, yCount, yStride);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var expected = mx.OuterProduct(my);

            Mat<Complex> actual;
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                actual = x.Outer(y, false);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 2, 1, 3)]
        [TestCase(2, 1, 2, 1)]
        [TestCase(2, 3, 2, 4)]
        [TestCase(3, 1, 3, 1)]
        [TestCase(3, 3, 3, 4)]
        [TestCase(2, 1, 3, 1)]
        [TestCase(2, 3, 3, 4)]
        [TestCase(3, 1, 2, 1)]
        [TestCase(3, 3, 2, 4)]
        public void Outer_Complex_C(int xCount, int xStride, int yCount, int yStride)
        {
            var x = TestVector.RandomComplex(42, xCount, xStride);
            var y = TestVector.RandomComplex(57, yCount, yStride);

            var mx = Utilities.ToMathNet(x);
            var my = Utilities.ToMathNet(y);
            var expected = mx.OuterProduct(my.Conjugate());

            Mat<Complex> actual;
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                actual = x.Outer(y, true);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(1, 3)]
        [TestCase(2, 5)]
        [TestCase(5, 7)]
        public void Conjugate(int count, int xStride)
        {
            var x = TestVector.RandomComplex(42, count, xStride);

            var expected = x.Select(value => value.Conjugate()).ToVector();

            Vec<Complex> actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Conjugate();
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(1, 3)]
        [TestCase(2, 5)]
        [TestCase(5, 7)]
        public void ToRowMatrix(int count, int xStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            Mat<double> actual;
            using (x.EnsureUnchanged())
            {
                actual = x.ToRowMatrix();
            }

            Assert.That(actual.RowCount, Is.EqualTo(1));
            Assert.That(actual.ColCount, Is.EqualTo(x.Count));

            NumAssert.AreSame(x, actual.Rows[0], 0);
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(1, 3)]
        [TestCase(2, 5)]
        [TestCase(5, 7)]
        public void ToColMatrix(int count, int xStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            Mat<double> actual;
            using (x.EnsureUnchanged())
            {
                actual = x.ToColMatrix();
            }

            Assert.That(actual.RowCount, Is.EqualTo(x.Count));
            Assert.That(actual.ColCount, Is.EqualTo(1));

            NumAssert.AreSame(x, actual.Cols[0], 0);
        }

        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(3, 1)]
        [TestCase(3, 3)]
        [TestCase(5, 1)]
        [TestCase(11, 7)]
        public void Map(int count, int xStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            var expected = x.Select(value => new Complex(0, -value)).ToVector();

            Vec <Complex> actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Map(value => new Complex(0, -value));
            }

            NumAssert.AreSame(expected, actual, 0);
        }
    }
}
