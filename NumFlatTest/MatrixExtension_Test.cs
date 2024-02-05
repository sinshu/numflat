using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MatrixExtension_Test
    {
        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 3, 5)]
        [TestCase(3, 3, 3, 3)]
        [TestCase(3, 3, 5, 4)]
        [TestCase(1, 3, 1, 1)]
        [TestCase(1, 3, 5, 2)]
        [TestCase(3, 1, 3, 4)]
        [TestCase(3, 1, 7, 7)]
        [TestCase(2, 3, 2, 4)]
        [TestCase(2, 3, 4, 3)]
        [TestCase(3, 2, 3, 4)]
        [TestCase(3, 2, 6, 4)]
        public void PointwiseMul(int rowCount, int colCount, int xStride, int yStride)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);
            var y = TestMatrix.RandomDouble(57, rowCount, colCount, yStride);

            var expected = x.Cols.Zip(y.Cols, (xCol, yCol) => xCol.PointwiseMul(yCol).AsEnumerable()).ColsToMatrix();

            Mat<double> actual;
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                actual = x.PointwiseMul(y);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 3, 5)]
        [TestCase(3, 3, 3, 3)]
        [TestCase(3, 3, 5, 4)]
        [TestCase(1, 3, 1, 1)]
        [TestCase(1, 3, 5, 2)]
        [TestCase(3, 1, 3, 4)]
        [TestCase(3, 1, 7, 7)]
        [TestCase(2, 3, 2, 4)]
        [TestCase(2, 3, 4, 3)]
        [TestCase(3, 2, 3, 4)]
        [TestCase(3, 2, 6, 4)]
        public void PointwiseDiv(int rowCount, int colCount, int xStride, int yStride)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);
            var y = TestMatrix.NonZeroRandomDouble(57, rowCount, colCount, yStride);

            var expected = x.Cols.Zip(y.Cols, (xCol, yCol) => xCol.PointwiseDiv(yCol).AsEnumerable()).ColsToMatrix();

            Mat<double> actual;
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                actual = x.PointwiseDiv(y);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 1, 3)]
        [TestCase(1, 3, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 2, 3)]
        [TestCase(2, 3, 2)]
        [TestCase(4, 5, 8)]
        [TestCase(9, 6, 11)]
        public void Transpose(int rowCount, int colCount, int xStride)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);

            Mat<double> actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Transpose();
            }

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(actual[col, row], Is.EqualTo(x[row, col]));
                }
            }
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 1, 3)]
        [TestCase(1, 3, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 2, 3)]
        [TestCase(2, 3, 2)]
        [TestCase(4, 5, 8)]
        [TestCase(9, 6, 11)]
        public void Conjugate(int rowCount, int colCount, int xStride)
        {
            var x = TestMatrix.RandomComplex(42, rowCount, colCount, xStride);

            var expected = x.Map(value => value.Conjugate());

            Mat<Complex> actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Conjugate();
            }

            NumAssert.AreSame(expected, actual, 0);
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 1, 3)]
        [TestCase(1, 3, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(3, 2, 3)]
        [TestCase(2, 3, 2)]
        [TestCase(4, 5, 8)]
        [TestCase(9, 6, 11)]
        public void ConjugateTranspose(int rowCount, int colCount, int xStride)
        {
            var x = TestMatrix.RandomComplex(42, rowCount, colCount, xStride);

            Mat<Complex> actual;
            using (x.EnsureUnchanged())
            {
                actual = x.ConjugateTranspose();
            }

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(actual[col, row], Is.EqualTo(x[row, col].Conjugate()));
                }
            }
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        public void Inverse_Single(int n, int xStride)
        {
            var x = TestMatrix.RandomSingle(42, n, n, xStride);

            Mat<float> inverse;
            using (x.EnsureUnchanged())
            {
                inverse = x.Inverse();
            }

            var expected = MatrixBuilder.Identity<float>(n);
            var actual = inverse * x;
            NumAssert.AreSame(expected, actual, 1.0E-6F);
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        public void Inverse_Double(int n, int xStride)
        {
            var x = TestMatrix.RandomDouble(42, n, n, xStride);

            Mat<double> inverse;
            using (x.EnsureUnchanged())
            {
                inverse = x.Inverse();
            }

            var expected = MatrixBuilder.Identity<double>(n);
            var actual = inverse * x;
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        public void Inverse_Complex(int n, int xStride)
        {
            var x = TestMatrix.RandomComplex(42, n, n, xStride);

            Mat<Complex> inverse;
            using (x.EnsureUnchanged())
            {
                inverse = x.Inverse();
            }

            var expected = MatrixBuilder.Identity<Complex>(n);
            var actual = inverse * x;
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 2, 3)]
        [TestCase(3, 2, 5)]
        [TestCase(2, 3, 2)]
        [TestCase(2, 3, 4)]
        [TestCase(6, 3, 7)]
        [TestCase(4, 7, 5)]
        public void PseudoInverse_Single(int rowCount, int colCount, int aStride)
        {
            var a = TestMatrix.RandomSingle(42, rowCount, colCount, aStride);

            var ma = Utilities.ToMathNet(a);
            var expected = ma.PseudoInverse();

            Mat<float> actual;
            using (a.EnsureUnchanged())
            {
                actual = a.PseudoInverse();
            }

            NumAssert.AreSame(expected, actual, 1.0E-5F); // 1.0E-6F is too small.
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 2, 3)]
        [TestCase(3, 2, 5)]
        [TestCase(2, 3, 2)]
        [TestCase(2, 3, 4)]
        [TestCase(6, 3, 7)]
        [TestCase(4, 7, 5)]
        public void PseudoInverse_Double(int rowCount, int colCount, int aStride)
        {
            var a = TestMatrix.RandomDouble(42, rowCount, colCount, aStride);

            var ma = Utilities.ToMathNet(a);
            var expected = ma.PseudoInverse();

            Mat<double> actual;
            using (a.EnsureUnchanged())
            {
                actual = a.PseudoInverse();
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 2, 3)]
        [TestCase(3, 2, 5)]
        [TestCase(2, 3, 2)]
        [TestCase(2, 3, 4)]
        [TestCase(6, 3, 7)]
        [TestCase(4, 7, 5)]
        public void PseudoInverse_Complex(int rowCount, int colCount, int aStride)
        {
            var a = TestMatrix.RandomComplex(42, rowCount, colCount, aStride);

            var ma = Utilities.ToMathNet(a);
            var expected = ma.PseudoInverse();

            Mat<Complex> actual;
            using (a.EnsureUnchanged())
            {
                actual = a.PseudoInverse();
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 5)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 3, 7)]
        [TestCase(3, 3, 3, 3)]
        [TestCase(3, 3, 5, 8)]
        [TestCase(1, 3, 1, 1)]
        [TestCase(1, 3, 5, 1)]
        [TestCase(3, 1, 3, 3)]
        [TestCase(3, 1, 7, 7)]
        [TestCase(2, 3, 2, 3)]
        [TestCase(2, 3, 4, 5)]
        [TestCase(3, 2, 3, 6)]
        [TestCase(3, 2, 6, 3)]
        public void Map(int rowCount, int colCount, int xStride, int dstStride)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);

            var expected = x.Cols.Select(col => col.Select(value => new Complex(0, -value))).ColsToMatrix();

            Mat <Complex> actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Map(value => new Complex(0, -value));
            }

            NumAssert.AreSame(expected, actual, 0);
        }
    }
}
