using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NumFlat;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace NumFlatTest
{
    public class QrDouble_Test
    {
        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 2, 3, 2)]
        [TestCase(2, 2, 2, 2, 2)]
        [TestCase(2, 2, 3, 3, 3)]
        [TestCase(3, 3, 3, 3, 3)]
        [TestCase(3, 3, 4, 5, 6)]
        [TestCase(7, 4, 7, 7, 7)]
        [TestCase(7, 4, 8, 8, 8)]
        [TestCase(8, 3, 8, 8, 8)]
        [TestCase(8, 3, 9, 9, 9)]
        public void Decompose(int m, int n, int aStride, int qStride, int rStride)
        {
            var a = Utilities.CreateRandomMatrixDouble(42, m, n, aStride);
            var q = Utilities.CreateRandomMatrixDouble(57, m, n, qStride);
            var r = Utilities.CreateRandomMatrixDouble(66, n, n, rStride);

            QrDouble.Decompose(a, q, r);

            var reconstructed = q * r;
            for (var row = 0; row < reconstructed.RowCount; row++)
            {
                for (var col = 0; col < reconstructed.ColCount; col++)
                {
                    var actual = reconstructed[row, col];
                    var expected = a[row, col];
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
                }
            }

            CheckUnitary(q);

            Utilities.FailIfOutOfRangeWrite(q);
            Utilities.FailIfOutOfRangeWrite(r);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 2, 3, 2)]
        [TestCase(2, 2, 2, 2, 2)]
        [TestCase(2, 2, 3, 3, 3)]
        [TestCase(3, 3, 3, 3, 3)]
        [TestCase(3, 3, 4, 5, 6)]
        [TestCase(7, 4, 7, 7, 7)]
        [TestCase(7, 4, 8, 8, 8)]
        [TestCase(8, 3, 8, 8, 8)]
        [TestCase(8, 3, 9, 9, 9)]
        public void ExtensionMethod(int m, int n, int aStride, int qStride, int rStride)
        {
            var a = Utilities.CreateRandomMatrixDouble(42, m, n, aStride);
            var qr = a.Qr();

            var reconstructed = qr.Q * qr.R;
            for (var row = 0; row < reconstructed.RowCount; row++)
            {
                for (var col = 0; col < reconstructed.ColCount; col++)
                {
                    var actual = reconstructed[row, col];
                    var expected = a[row, col];
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
                }
            }

            CheckUnitary(qr.Q);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 2, 4, 3)]
        [TestCase(2, 2, 2, 1, 1)]
        [TestCase(2, 2, 4, 9, 3)]
        [TestCase(3, 3, 3, 1, 1)]
        [TestCase(3, 3, 4, 2, 2)]
        [TestCase(3, 2, 3, 1, 1)]
        [TestCase(3, 2, 4, 5, 2)]
        [TestCase(6, 3, 7, 4, 2)]
        public void Solve_Arg2(int m, int n, int aStride, int bStride, int dstStride)
        {
            var a = Utilities.CreateRandomMatrixDouble(42, m, n, aStride);
            var b = Utilities.CreateRandomVectorDouble(57, a.RowCount, bStride);
            var qr = a.Qr();
            var destination = Utilities.CreateRandomVectorDouble(66, a.ColCount, dstStride);
            qr.Solve(b, destination);

            var expected = a.Svd().Solve(b);

            for (var i = 0; i < expected.Count; i++)
            {
                Assert.That(destination[i], Is.EqualTo(expected[i]).Within(1.0E-12));
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 2, 4)]
        [TestCase(2, 2, 2, 1)]
        [TestCase(2, 2, 4, 9)]
        [TestCase(3, 3, 3, 1)]
        [TestCase(3, 3, 4, 2)]
        [TestCase(3, 2, 3, 1)]
        [TestCase(3, 2, 4, 5)]
        [TestCase(6, 3, 7, 4)]
        public void Solve_Arg1(int m, int n, int aStride, int bStride)
        {
            var a = Utilities.CreateRandomMatrixDouble(42, m, n, aStride);
            var b = Utilities.CreateRandomVectorDouble(57, a.RowCount, bStride);
            var qr = a.Qr();
            var destination = qr.Solve(b);

            var expected = a.Svd().Solve(b);

            for (var i = 0; i < expected.Count; i++)
            {
                Assert.That(destination[i], Is.EqualTo(expected[i]).Within(1.0E-12));
            }
        }

        private static void CheckUnitary(Mat<double> mat)
        {
            var identity = mat.Transpose() * mat;
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
