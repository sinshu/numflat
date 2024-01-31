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
        public void DotSingle(int count, int xStride, int yStride)
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
        public void DotDouble(int count, int xStride, int yStride)
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
        public void DotComplex_N(int count, int xStride, int yStride)
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
        public void DotComplex_C(int count, int xStride, int yStride)
        {
            var x = Utilities.CreateRandomVectorComplex(42, count, xStride);
            var y = Utilities.CreateRandomVectorComplex(57, count, yStride);
            var actual = x.Dot(y, true);

            var expected = x.Zip(y, (val1, val2) => val1.Conjugate() * val2).Aggregate((sum, next) => sum + next);
            Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
            Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
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
        public void OuterSingle(int xCount, int xStride, int yCount, int yStride)
        {
            var x = Utilities.CreateRandomVectorSingle(42, xCount, xStride);
            var y = Utilities.CreateRandomVectorSingle(57, yCount, yStride);
            var destination = x.Outer(y);

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
        public void OuterDouble(int xCount, int xStride, int yCount, int yStride)
        {
            var x = Utilities.CreateRandomVectorDouble(42, xCount, xStride);
            var y = Utilities.CreateRandomVectorDouble(57, yCount, yStride);
            var destination = x.Outer(y);

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
        public void OuterComplex_N(int xCount, int xStride, int yCount, int yStride)
        {
            var x = Utilities.CreateRandomVectorComplex(42, xCount, xStride);
            var y = Utilities.CreateRandomVectorComplex(57, yCount, yStride);
            var destination = x.Outer(y, false);

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
        public void OuterComplex_C(int xCount, int xStride, int yCount, int yStride)
        {
            var x = Utilities.CreateRandomVectorComplex(42, xCount, xStride);
            var y = Utilities.CreateRandomVectorComplex(57, yCount, yStride);
            var destination = x.Outer(y, true);

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

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(1, 3)]
        [TestCase(2, 5)]
        [TestCase(5, 7)]
        public void ToRowMatrix(int count, int xStride)
        {
            var x = Utilities.CreateRandomVectorComplex(42, count, xStride);
            var destination = x.ToRowMatrix();

            Assert.That(destination.RowCount == 1);
            Assert.That(destination.ColCount == x.Count);

            for (var i = 0; i < x.Count; i++)
            {
                Assert.That(destination[0, i] == x[i]);
            }
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(1, 3)]
        [TestCase(2, 5)]
        [TestCase(5, 7)]
        public void ToColMatrix(int count, int xStride)
        {
            var x = Utilities.CreateRandomVectorComplex(42, count, xStride);
            var destination = x.ToColMatrix();

            Assert.That(destination.RowCount == x.Count);
            Assert.That(destination.ColCount == 1);

            for (var i = 0; i < x.Count; i++)
            {
                Assert.That(destination[i, 0] == x[i]);
            }
        }
    }
}
