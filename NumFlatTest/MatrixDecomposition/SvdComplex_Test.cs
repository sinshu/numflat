using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class SvdComplex_Test
    {
        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4)]
        [TestCase(2, 2, 2, 1)]
        [TestCase(2, 2, 4, 2)]
        [TestCase(3, 4, 3, 1)]
        [TestCase(3, 4, 5, 5)]
        [TestCase(4, 3, 4, 1)]
        [TestCase(4, 3, 6, 3)]
        public void GetSingularValues(int m, int n, int aStride, int sStride)
        {
            var a = Utilities.CreateRandomMatrixComplex(42, m, n, aStride);
            var actual = Utilities.CreateRandomVectorDouble(57, Math.Min(m, n), sStride);
            SvdComplex.GetSingularValues(a, actual);

            var expected = a.Svd().S;

            for (var i = 0; i < expected.Count; i++)
            {
                Assert.That(expected[i], Is.EqualTo(actual[i]).Within(1.0E-12));
            }

            Utilities.FailIfOutOfRangeWrite(a);
            Utilities.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4, 2, 5)]
        [TestCase(2, 2, 2, 1, 2, 2)]
        [TestCase(2, 2, 4, 2, 3, 3)]
        [TestCase(3, 4, 3, 1, 3, 4)]
        [TestCase(3, 4, 5, 5, 7, 6)]
        [TestCase(4, 3, 4, 1, 4, 3)]
        [TestCase(4, 3, 6, 3, 7, 5)]
        public void Decompose(int m, int n, int aStride, int sStride, int uStride, int vtStride)
        {
            var a = Utilities.CreateRandomMatrixComplex(42, m, n, aStride);
            var s = Utilities.CreateRandomVectorDouble(57, Math.Min(m, n), sStride);
            var u = Utilities.CreateRandomMatrixComplex(66, m, m, uStride);
            var vt = Utilities.CreateRandomMatrixComplex(77, n, n, vtStride);

            SvdComplex.Decompose(a, s, u, vt);

            var reconstructed = u * s.Select(e => (Complex)e).ToDiagonalMatrix(m, n) * vt;
            for (var row = 0; row < reconstructed.RowCount; row++)
            {
                for (var col = 0; col < reconstructed.ColCount; col++)
                {
                    var expected = a[row, col];
                    var actual = reconstructed[row, col];
                    Assert.That(expected.Real, Is.EqualTo(actual.Real).Within(1.0E-12));
                    Assert.That(expected.Imaginary, Is.EqualTo(actual.Imaginary).Within(1.0E-12));
                }
            }

            CheckUnitary(u);
            CheckUnitary(vt);

            Utilities.FailIfOutOfRangeWrite(a);
            Utilities.FailIfOutOfRangeWrite(s);
            Utilities.FailIfOutOfRangeWrite(u);
            Utilities.FailIfOutOfRangeWrite(vt);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(4, 3, 4)]
        [TestCase(4, 3, 6)]
        public void ExtensionMethod(int m, int n, int aStride)
        {
            var a = Utilities.CreateRandomMatrixComplex(42, m, n, aStride);
            var svd = a.Svd();

            var reconstructed = svd.U * svd.S.Select(e => (Complex)e).ToDiagonalMatrix(m, n) * svd.VT;
            for (var row = 0; row < reconstructed.RowCount; row++)
            {
                for (var col = 0; col < reconstructed.ColCount; col++)
                {
                    var expected = a[row, col];
                    var actual = reconstructed[row, col];
                    Assert.That(expected.Real, Is.EqualTo(actual.Real).Within(1.0E-12));
                    Assert.That(expected.Imaginary, Is.EqualTo(actual.Imaginary).Within(1.0E-12));
                }
            }

            CheckUnitary(svd.U);
            CheckUnitary(svd.VT);
        }

        private static void CheckUnitary(Mat<Complex> mat)
        {
            var identity = mat * mat.ConjugateTranspose();
            for (var row = 0; row < identity.RowCount; row++)
            {
                for (var col = 0; col < identity.ColCount; col++)
                {
                    if (row == col)
                    {
                        Assert.That(identity[row, col].Real, Is.EqualTo(1.0).Within(1.0E-12));
                        Assert.That(identity[row, col].Imaginary, Is.EqualTo(0.0).Within(1.0E-12));
                    }
                    else
                    {
                        Assert.That(identity[row, col].Real, Is.EqualTo(0.0).Within(1.0E-12));
                        Assert.That(identity[row, col].Imaginary, Is.EqualTo(0.0).Within(1.0E-12));
                    }
                }
            }
        }
    }
}
