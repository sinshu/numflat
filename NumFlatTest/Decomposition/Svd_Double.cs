using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class Svd_Double
    {
        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4, 2, 5)]
        [TestCase(2, 2, 2, 1, 2, 2)]
        [TestCase(2, 2, 4, 2, 3, 3)]
        [TestCase(3, 4, 3, 1, 3, 4)]
        [TestCase(3, 4, 5, 5, 7, 6)]
        [TestCase(4, 3, 4, 1, 4, 3)]
        [TestCase(4, 3, 6, 3, 7, 5)]
        public void Test(int m, int n, int aStride, int sStride, int uStride, int vtStride)
        {
            var a = Utilities.CreateRandomMatrixDouble(42, m, n, aStride);
            var s = Utilities.CreateRandomVectorDouble(57, Math.Min(m, n), sStride);
            var u = Utilities.CreateRandomMatrixDouble(66, m, m, uStride);
            var vt = Utilities.CreateRandomMatrixDouble(77, n, n, vtStride);

            SingularValueDecompositionDouble.Decompose(a, s, u, vt);

            var reconstructed = u * s.ToDiagonalMatrix(m, n) * vt;
            for (var row = 0; row < reconstructed.RowCount; row++)
            {
                for (var col = 0; col < reconstructed.ColCount; col++)
                {
                    var expected = a[row, col];
                    var actual = reconstructed[row, col];
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));
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
            var a = Utilities.CreateRandomMatrixDouble(42, m, n, aStride);
            var svd = a.Svd();

            var reconstructed = svd.U * svd.S.ToDiagonalMatrix(m, n) * svd.VT;
            for (var row = 0; row < reconstructed.RowCount; row++)
            {
                for (var col = 0; col < reconstructed.ColCount; col++)
                {
                    var expected = a[row, col];
                    var actual = reconstructed[row, col];
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));
                }
            }

            CheckUnitary(svd.U);
            CheckUnitary(svd.VT);
        }

        private static void CheckUnitary(Mat<double> mat)
        {
            var identity = mat * mat.Transpose();
            for (var row = 0; row < identity.RowCount; row++)
            {
                for (var col = 0; col < identity.ColCount; col++)
                {
                    if (row == col)
                    {
                        Assert.That(identity[row, col], Is.EqualTo(1.0).Within(1.0E-12));
                    }
                    else
                    {
                        Assert.That(identity[row, col], Is.EqualTo(0.0).Within(1.0E-12));
                    }
                }
            }
        }
    }
}
