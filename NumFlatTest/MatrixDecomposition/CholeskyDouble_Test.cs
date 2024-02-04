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
            var source = CreateHermitianMatrix(42, n);
            var a = Utilities.CreateRandomMatrixDouble(0, n, n, aStride);
            source.CopyTo(a);
            var destination = Utilities.CreateRandomMatrixDouble(0, n, n, lStride);

            CholeskyDouble.Decompose(a, destination);

            Console.WriteLine(a);

            var reconstructed = destination * destination.Transpose();

            Console.WriteLine(reconstructed);

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

        private static Mat<double> CreateHermitianMatrix(int seed, int n)
        {
            var random = new Random(seed);
            var mat = new Mat<double>(n, n);
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
