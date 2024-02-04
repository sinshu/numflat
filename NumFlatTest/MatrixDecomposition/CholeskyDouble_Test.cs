using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class CholeskyDouble_Test
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
            var destination = Utilities.CreateRandomMatrixDouble(0, n, n, lStride);

            CholeskyDouble.Decompose(a, destination);

            var reconstructed = destination * destination.Transpose();

            for (var row = 0; row < reconstructed.RowCount; row++)
            {
                for (var col = 0; col < reconstructed.ColCount; col++)
                {
                    var actual = reconstructed[row, col];
                    var expected = a[row, col];
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
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
            var chol = a.Cholesky();

            var reconstructed = chol.L * chol.L.Transpose();

            for (var row = 0; row < reconstructed.RowCount; row++)
            {
                for (var col = 0; col < reconstructed.ColCount; col++)
                {
                    var actual = reconstructed[row, col];
                    var expected = a[row, col];
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
                }
            }
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
            var b = Utilities.CreateRandomVectorDouble(57, n, bStride);
            var chol = a.Cholesky();
            var destination = Utilities.CreateRandomVectorDouble(66, n, dstStride);
            chol.Solve(b, destination);

            var expected = a.Svd().Solve(b);

            for (var i = 0; i < n; i++)
            {
                Assert.That(destination[i], Is.EqualTo(expected[i]).Within(1.0E-12));
            }

            Utilities.FailIfOutOfRangeWrite(destination);
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
        public void Solve_Arg1(int n, int aStride, int bStride, int dstStride)
        {
            var a = CreateHermitianMatrix(42, n, aStride);
            var b = Utilities.CreateRandomVectorDouble(57, n, bStride);
            var chol = a.Cholesky();
            var destination = chol.Solve(b);

            var expected = a.Svd().Solve(b);

            for (var i = 0; i < n; i++)
            {
                Assert.That(destination[i], Is.EqualTo(expected[i]).Within(1.0E-12));
            }
        }

        private static Mat<double> CreateHermitianMatrix(int seed, int n, int stride)
        {
            var mat = Utilities.CreateRandomMatrixDouble(seed, n, n, stride);
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
                        mat[row, col] = Utilities.NextDouble(random);
                        mat[col, row] = mat[row, col];
                    }
                }
            }
            return mat;
        }
    }
}
