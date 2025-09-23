using MathNet.Numerics.LinearAlgebra.Factorization;
using NumFlat;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace NumFlatTest.MatrixDecompositionTests
{
    public class EvdTestsDouble
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

            EigenValueDecompositionDouble evd;
            using (a.EnsureUnchanged())
            {
                evd = a.Evd();
            }

            var reconstructed = evd.V * evd.D.ToDiagonalMatrix() * evd.V.Transpose();
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
        public void Solve(int n, int aStride, int bStride, int dstStride)
        {
            var a = CreateHermitianMatrix(42, n, aStride);
            var b = TestVector.RandomDouble(57, a.RowCount, bStride);
            var evd = a.Evd();

            var actual = TestVector.RandomDouble(66, a.ColCount, dstStride);
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                evd.Solve(b, actual);
            }

            var expected = a.Svd().Solve(b);

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Determinant(int n)
        {
            var a = CreateHermitianMatrix(42, n, n);
            var evd = a.Evd();
            var expected = a.Determinant();
            Assert.That(evd.Determinant(), Is.EqualTo(expected).Within(1.0E-12));
            Assert.That(evd.LogDeterminant(), Is.EqualTo(Math.Log(expected)).Within(1.0E-12));
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

        [Test]
        public void Rank()
        {
            Mat<double> rank0 =
            [
                [0, 0, 0],
                [0, 0, 0],
                [0, 0, 0],
            ];

            Mat<double> rank1 =
            [
                [0, 0, 0],
                [0, 1, 0],
                [0, 0, 0],
            ];

            Mat<double> rank3 =
            [
                [1, 0, 0],
                [0, 1, 0],
                [0, 0, 2],
            ];

            Assert.That(rank0.Evd().Rank(), Is.EqualTo(0));
            Assert.That(rank1.Evd().Rank(), Is.EqualTo(1));
            Assert.That(rank3.Evd().Rank(), Is.EqualTo(3));
            Assert.That(rank3.Evd().Rank(0.999), Is.EqualTo(3));
            Assert.That(rank3.Evd().Rank(1.001), Is.EqualTo(1));
            Assert.That(rank3.Evd().Rank(2.001), Is.EqualTo(0));
        }
    }
}
