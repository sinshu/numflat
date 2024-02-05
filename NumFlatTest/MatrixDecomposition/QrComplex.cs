using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class QrComplex_Test
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
            var a = TestMatrix.RandomComplex(42, m, n, aStride);
            var q = TestMatrix.RandomComplex(57, m, n, qStride);
            var r = TestMatrix.RandomComplex(66, n, n, rStride);

            using (a.EnsureUnchanged())
            {
                QrComplex.Decompose(a, q, r);
            }

            var reconstructed = q * r;
            NumAssert.AreSame(a, reconstructed, 1.0E-12);

            CheckUnitary(q);

            TestMatrix.FailIfOutOfRangeWrite(q);
            TestMatrix.FailIfOutOfRangeWrite(r);
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
            var a = TestMatrix.RandomComplex(42, m, n, aStride);

            QrComplex qr;
            using (a.EnsureUnchanged())
            {
                qr = a.Qr();
            }

            var reconstructed = qr.Q * qr.R;
            NumAssert.AreSame(a, reconstructed, 1.0E-12);

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
            var a = TestMatrix.RandomComplex(42, m, n, aStride);
            var b = TestVector.RandomComplex(57, a.RowCount, bStride);
            var qr = a.Qr();

            var actual = TestVector.RandomComplex(66, a.ColCount, dstStride);
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                qr.Solve(b, actual);
            }

            var expected = a.Svd().Solve(b);

            NumAssert.AreSame(expected, actual, 1.0E-12);

            Utilities.FailIfOutOfRangeWrite(actual);
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
            var a = TestMatrix.RandomComplex(42, m, n, aStride);
            var b = TestVector.RandomComplex(57, a.RowCount, bStride);
            var qr = a.Qr();

            Vec<Complex> actual;
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                actual = qr.Solve(b);
            }

            var expected = a.Svd().Solve(b);

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        private static void CheckUnitary(Mat<Complex> mat)
        {
            var actual = mat.ConjugateTranspose() * mat;
            var expected = MatrixBuilder.Identity<Complex>(actual.RowCount);
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }
    }
}
