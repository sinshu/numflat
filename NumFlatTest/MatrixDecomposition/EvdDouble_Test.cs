using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class EvdDouble_Test
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
            var v = TestMatrix.RandomDouble(0, n, n, vStride);
            using (a.EnsureUnchanged())
            {
                EigenValueDecompositionDouble.Decompose(a, d, v);
            }

            var reconstructed = v * d.ToDiagonalMatrix() * v.Transpose();
            NumAssert.AreSame(a, reconstructed, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(d);
            TestMatrix.FailIfOutOfRangeWrite(v);
        }

        private static Mat<double> CreateHermitianMatrix(int seed, int n, int stride)
        {
            var mat = TestMatrix.RandomDouble(seed, n, n, stride);
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
                        mat[row, col] = mat[col, row];
                    }
                }
            }
            return mat;
        }
    }
}
