using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class GevdTest_Complex
    {
        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 2, 3, 2, 4)]
        [TestCase(2, 2, 2, 1, 2)]
        [TestCase(2, 3, 3, 2, 4)]
        [TestCase(3, 3, 3, 1, 3)]
        [TestCase(3, 5, 4, 3, 4)]
        [TestCase(4, 4, 4, 1, 4)]
        [TestCase(4, 6, 5, 3, 5)]
        [TestCase(5, 5, 5, 1, 5)]
        [TestCase(5, 6, 7, 3, 8)]
        public void Decompose(int n, int aStride, int bStride, int dStride, int vStride)
        {
            var a = CreateHermitianMatrix(42, n, aStride);
            var b = CreateHermitianMatrix(57, n, bStride);

            var d = TestVector.RandomDouble(0, n, dStride);
            var v = TestMatrix.RandomComplex(0, n, n, vStride);
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                GeneralizedEigenValueDecompositionComplex.Decompose(a, b, d, v);
            }

            for (var i = 0; i < n; i++)
            {
                var left = a * v.Cols[i];
                var right = d[i] * b * v.Cols[i];
                NumAssert.AreSame(left, right, 1.0E-12);
            }

            TestVector.FailIfOutOfRangeWrite(d);
            TestMatrix.FailIfOutOfRangeWrite(v);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 3, 3)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 5, 4)]
        [TestCase(4, 4, 4)]
        [TestCase(4, 6, 5)]
        [TestCase(5, 5, 5)]
        [TestCase(5, 6, 7)]
        public void ExtensionMethod(int n, int aStride, int bStride)
        {
            var a = CreateHermitianMatrix(42, n, aStride);
            var b = CreateHermitianMatrix(57, n, bStride);

            GeneralizedEigenValueDecompositionComplex gevd;
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                gevd = a.Gevd(b);
            }

            for (var i = 0; i < n; i++)
            {
                var left = a * gevd.V.Cols[i];
                var right = gevd.D[i] * b * gevd.V.Cols[i];
                NumAssert.AreSame(left, right, 1.0E-12);
            }
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
