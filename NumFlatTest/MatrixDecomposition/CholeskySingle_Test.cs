using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class CholeskySingle_Test
    {
        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 3, 3)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 5, 3)]
        [TestCase(5, 5, 5)]
        [TestCase(5, 8, 6)]
        public void Decompose(int n, int aStride, int lStride)
        {
            var a = CreateHermitianMatrix(42, n, aStride);

            var l = TestMatrix.RandomSingle(0, n, n, lStride);
            using (a.EnsureUnchanged())
            {
                CholeskyDecompositionSingle.Decompose(a, l);
            }

            var reconstructed = l * l.Transpose();
            NumAssert.AreSame(a, reconstructed, 1.0E-6F);

            TestMatrix.FailIfOutOfRangeWrite(l);
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 3)]
        [TestCase(3, 5)]
        [TestCase(5, 5)]
        [TestCase(5, 8)]
        public void ExtensionMethod(int n, int aStride)
        {
            var a = CreateHermitianMatrix(42, n, aStride);

            CholeskyDecompositionSingle chol;
            using (a.EnsureUnchanged())
            {
                chol = a.Cholesky();
            }

            var reconstructed = chol.L * chol.L.Transpose();
            NumAssert.AreSame(a, reconstructed, 1.0E-6F);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 2, 3)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 4, 3)]
        [TestCase(3, 3, 3, 3)]
        [TestCase(3, 3, 4, 4)]
        [TestCase(4, 4, 4, 4)]
        [TestCase(4, 4, 6, 5)]
        [TestCase(5, 5, 5, 5)]
        [TestCase(5, 5, 8, 7)]
        public void Solve_Arg2(int n, int aStride, int bStride, int dstStride)
        {
            var a = CreateHermitianMatrix(42, n, aStride);
            var b = TestVector.RandomSingle(57, a.RowCount, bStride);
            var chol = a.Cholesky();

            var actual = TestVector.RandomSingle(66, a.ColCount, dstStride);
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                chol.Solve(b, actual);
            }

            var expected = a.Svd().Solve(b);

            NumAssert.AreSame(expected, actual, 1.0E-6F);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 2)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 3, 4)]
        [TestCase(4, 4, 4)]
        [TestCase(4, 4, 6)]
        [TestCase(5, 5, 5)]
        [TestCase(5, 5, 8)]
        public void Solve_Arg1(int n, int aStride, int bStride)
        {
            var a = CreateHermitianMatrix(42, n, aStride);
            var b = TestVector.RandomSingle(57, a.RowCount, bStride);
            var chol = a.Cholesky();

            var ma = Interop.ToMathNet(a);
            var mb = Interop.ToMathNet(b);
            var expected = ma.Svd().Solve(mb);

            Vec<float> actual;
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                actual = chol.Solve(b);
            }

            NumAssert.AreSame(expected, actual, 1.0E-6F);
        }

        private static Mat<float> CreateHermitianMatrix(int seed, int n, int stride)
        {
            var mat = TestMatrix.RandomSingle(seed, n, n, stride);
            var random = new Random(seed);
            for (var col = 0; col < n; col++)
            {
                for (var row = col; row < n; row++)
                {
                    if (row == col)
                    {
                        mat[row, col] = 2 + 2 * random.NextSingle();
                    }
                    else
                    {
                        mat[row, col] = mat[col, row];
                    }
                }
            }
            return mat;
        }
    }
}
