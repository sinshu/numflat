using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class EvdComplex_Test
    {
        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 3, 3, 3)]
        [TestCase(2, 2, 1, 2)]
        [TestCase(2, 4, 4, 3)]
        [TestCase(3, 3, 1, 3)]
        [TestCase(3, 5, 3, 4)]
        [TestCase(4, 4, 1, 4)]
        [TestCase(4, 5, 2, 5)]
        [TestCase(5, 5, 1, 5)]
        [TestCase(5, 7, 3, 8)]
        public void Decompose(int n, int aStride, int dStride, int vStride)
        {
            var a = CreateHermitianMatrix(42, n, aStride);

            var d = TestVector.RandomDouble(0, n, dStride);
            var v = TestMatrix.RandomComplex(0, n, n, vStride);
            using (a.EnsureUnchanged())
            {
                EigenValueDecompositionComplex.Decompose(a, d, v);
            }

            var reconstructed = v * d.Select(value => (Complex)value).ToDiagonalMatrix() * v.ConjugateTranspose();
            NumAssert.AreSame(a, reconstructed, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(d);
            TestMatrix.FailIfOutOfRangeWrite(v);
        }

        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(2, 2)]
        [TestCase(2, 4)]
        [TestCase(3, 3)]
        [TestCase(3, 5)]
        [TestCase(4, 4)]
        [TestCase(4, 5)]
        [TestCase(5, 5)]
        [TestCase(5, 7)]
        public void ExtensionMethod(int n, int aStride)
        {
            var a = CreateHermitianMatrix(42, n, aStride);

            EigenValueDecompositionComplex evd;
            using (a.EnsureUnchanged())
            {
                evd = a.Evd();
            }

            var reconstructed = evd.V * evd.D.Select(value => (Complex)value).ToDiagonalMatrix() * evd.V.ConjugateTranspose();
            NumAssert.AreSame(a, reconstructed, 1.0E-12);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 2, 3)]
        [TestCase(2, 2, 1, 1)]
        [TestCase(2, 2, 4, 3)]
        [TestCase(3, 3, 1, 1)]
        [TestCase(3, 3, 2, 4)]
        [TestCase(4, 4, 1, 1)]
        [TestCase(4, 4, 3, 2)]
        [TestCase(5, 5, 1, 1)]
        [TestCase(5, 5, 8, 7)]
        public void Solve_Arg2(int n, int aStride, int bStride, int dstStride)
        {
            var a = CreateHermitianMatrix(42, n, aStride);
            var b = TestVector.RandomComplex(57, a.RowCount, bStride);
            var evd = a.Evd();

            var actual = TestVector.RandomComplex(66, a.ColCount, dstStride);
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                evd.Solve(b, actual);
            }

            var expected = a.Svd().Solve(b);

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 2)]
        [TestCase(2, 2, 1)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 3, 1)]
        [TestCase(3, 3, 2)]
        [TestCase(4, 4, 1)]
        [TestCase(4, 4, 3)]
        [TestCase(5, 5, 1)]
        [TestCase(5, 5, 8)]
        public void Solve_Arg1(int n, int aStride, int bStride)
        {
            var a = CreateHermitianMatrix(42, n, aStride);
            var b = TestVector.RandomComplex(57, a.RowCount, bStride);
            var evd = a.Evd();

            var expected = a.Svd().Solve(b);

            Vec<Complex> actual;
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                actual = evd.Solve(b);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        private static Mat<Complex> CreateHermitianMatrix(int seed, int n, int stride)
        {
            var mat = TestMatrix.RandomComplex(seed, n, n, stride);
            var random = new Random(seed);
            for (var col = 0; col < n; col++)
            {
                for (var row = col; row < n; row++)
                {
                    if (row == col)
                    {
                        mat[row, col] = 2 + 2 * random.NextDouble();
                    }
                    else
                    {
                        mat[row, col] = mat[col, row].Conjugate();
                    }
                }
            }
            return mat;
        }
    }
}
