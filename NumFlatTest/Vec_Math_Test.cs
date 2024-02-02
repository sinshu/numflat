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
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);
            var y = Utilities.CreateRandomVectorDouble(57, count, yStride);
            var destination = Utilities.CreateRandomVectorDouble(0, count, dstStride);
            Vec.Add(x, y, destination);

            var expected = x.Zip(y, (val1, val2) => val1 + val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 3, 2, 4)]
        [TestCase(3, 1, 1, 1)]
        [TestCase(3, 3, 2, 4)]
        [TestCase(5, 1, 2, 3)]
        [TestCase(11, 7, 2, 5)]
        public void Sub(int count, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);
            var y = Utilities.CreateRandomVectorDouble(57, count, yStride);
            var destination = Utilities.CreateRandomVectorDouble(0, count, dstStride);
            Vec.Sub(x, y, destination);

            var expected = x.Zip(y, (val1, val2) => val1 - val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1.5, 1)]
        [TestCase(1, 3, 2.3, 4)]
        [TestCase(3, 1, 0.1, 1)]
        [TestCase(3, 3, 0.7, 4)]
        [TestCase(5, 1, 3.5, 3)]
        [TestCase(11, 7, 7.9, 5)]
        public void Mul(int count, int xStride, double y, int dstStride)
        {
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);
            var destination = Utilities.CreateRandomVectorDouble(0, count, dstStride);
            Vec.Mul(x, y, destination);

            var expected = x.Select(value => value * y).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1.5, 1)]
        [TestCase(1, 3, 2.3, 4)]
        [TestCase(3, 1, 0.1, 1)]
        [TestCase(3, 3, 0.7, 4)]
        [TestCase(5, 1, 3.5, 3)]
        [TestCase(11, 7, 7.9, 5)]
        public void Div(int count, int xStride, double y, int dstStride)
        {
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);
            var destination = Utilities.CreateRandomVectorDouble(0, count, dstStride);
            Vec.Div(x, y, destination);

            var expected = x.Select(value => value / y).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 3, 2, 4)]
        [TestCase(3, 1, 1, 1)]
        [TestCase(3, 3, 2, 4)]
        [TestCase(5, 1, 2, 3)]
        [TestCase(11, 7, 2, 5)]
        public void PointwiseMul(int count, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);
            var y = Utilities.CreateRandomVectorDouble(57, count, yStride);
            var destination = Utilities.CreateRandomVectorDouble(0, count, dstStride);
            Vec.PointwiseMul(x, y, destination);

            var expected = x.Zip(y, (val1, val2) => val1 * val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 3, 2, 4)]
        [TestCase(3, 1, 1, 1)]
        [TestCase(3, 3, 2, 4)]
        [TestCase(5, 1, 2, 3)]
        [TestCase(11, 7, 2, 5)]
        public void PointwiseDiv(int count, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);
            var y = Utilities.CreateRandomVectorNonZeroDouble(57, count, yStride);
            var destination = Utilities.CreateRandomVectorDouble(0, count, dstStride);
            Vec.PointwiseDiv(x, y, destination);

            var expected = x.Zip(y, (val1, val2) => val1 / val2).ToArray();
            var actual = destination.ToArray();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void Dot_Single(int count, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomVectorSingle(42, count, xStride);
            var y = Utilities.CreateRandomVectorSingle(57, count, yStride);
            var actual = x.Dot(y);

            var expected = x.Zip(y, (val1, val2) => val1 * val2).Sum();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void Dot_Double(int count, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomVectorDouble(42, count, xStride);
            var y = Utilities.CreateRandomVectorDouble(57, count, yStride);
            var actual = x.Dot(y);

            var expected = x.Zip(y, (val1, val2) => val1 * val2).Sum();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void Dot_Complex_N(int count, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomVectorComplex(42, count, xStride);
            var y = Utilities.CreateRandomVectorComplex(57, count, yStride);
            var actual = x.Dot(y, false);

            var expected = x.Zip(y, (val1, val2) => val1 * val2).Aggregate((sum, next) => sum + next);
            Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
            Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 1, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(5, 1, 2)]
        [TestCase(11, 7, 2)]
        public void Dot_Complex_C(int count, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomVectorComplex(42, count, xStride);
            var y = Utilities.CreateRandomVectorComplex(57, count, yStride);
            var actual = x.Dot(y, true);

            var expected = x.Zip(y, (val1, val2) => val1.Conjugate() * val2).Aggregate((sum, next) => sum + next);
            Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
            Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
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
            var x = Utilities.CreateRandomVectorSingle(42, xCount, xStride);
            var y = Utilities.CreateRandomVectorSingle(57, yCount, yStride);
            var destination = Utilities.CreateRandomMatrixSingle(0, x.Count, y.Count, dstStride);
            Vec.Outer(x, y, destination);

            var mx = new MathNet.Numerics.LinearAlgebra.Single.DenseVector(x.ToArray());
            var my = new MathNet.Numerics.LinearAlgebra.Single.DenseVector(y.ToArray());
            var md = mx.OuterProduct(my);

            for (var row = 0; row < md.RowCount; row++)
            {
                for (var col = 0; col < md.ColumnCount; col++)
                {
                    var expected = md[row, col];
                    var actual = destination[row, col];
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
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
            var x = Utilities.CreateRandomVectorDouble(42, xCount, xStride);
            var y = Utilities.CreateRandomVectorDouble(57, yCount, yStride);
            var destination = Utilities.CreateRandomMatrixDouble(0, x.Count, y.Count, dstStride);
            Vec.Outer(x, y, destination);

            var mx = new MathNet.Numerics.LinearAlgebra.Double.DenseVector(x.ToArray());
            var my = new MathNet.Numerics.LinearAlgebra.Double.DenseVector(y.ToArray());
            var md = mx.OuterProduct(my);

            for (var row = 0; row < md.RowCount; row++)
            {
                for (var col = 0; col < md.ColumnCount; col++)
                {
                    var expected = md[row, col];
                    var actual = destination[row, col];
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
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
            var x = Utilities.CreateRandomVectorComplex(42, xCount, xStride);
            var y = Utilities.CreateRandomVectorComplex(57, yCount, yStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, x.Count, y.Count, dstStride);
            Vec.Outer(x, y, destination, false);

            var mx = new MathNet.Numerics.LinearAlgebra.Complex.DenseVector(x.ToArray());
            var my = new MathNet.Numerics.LinearAlgebra.Complex.DenseVector(y.ToArray());
            var md = mx.OuterProduct(my);

            for (var row = 0; row < md.RowCount; row++)
            {
                for (var col = 0; col < md.ColumnCount; col++)
                {
                    var expected = md[row, col];
                    var actual = destination[row, col];
                    Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
                    Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
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
            var x = Utilities.CreateRandomVectorComplex(42, xCount, xStride);
            var y = Utilities.CreateRandomVectorComplex(57, yCount, yStride);
            var destination = Utilities.CreateRandomMatrixComplex(0, x.Count, y.Count, dstStride);
            Vec.Outer(x, y, destination, true);

            var mx = new MathNet.Numerics.LinearAlgebra.Complex.DenseVector(x.ToArray());
            var my = new MathNet.Numerics.LinearAlgebra.Complex.DenseVector(y.ToArray()).Conjugate();
            var md = mx.OuterProduct(my);

            for (var row = 0; row < md.RowCount; row++)
            {
                for (var col = 0; col < md.ColumnCount; col++)
                {
                    var expected = md[row, col];
                    var actual = destination[row, col];
                    Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
                    Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
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
            var destination = Utilities.CreateRandomVectorComplex(0, count, dstStride);
            Vec.Conjugate(x, destination);

            for (var i = 0; i < count; i++)
            {
                Assert.That(x[i].Conjugate() == destination[i]);
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }
    }
}
