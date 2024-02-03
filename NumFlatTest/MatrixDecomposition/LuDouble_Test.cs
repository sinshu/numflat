using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class LuDouble_Test
    {
        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 3, 4)]
        [TestCase(4, 4, 4)]
        [TestCase(4, 4, 5)]
        [TestCase(5, 5, 5)]
        [TestCase(5, 5, 7)]
        [TestCase(3, 7, 3)]
        [TestCase(3, 7, 4)]
        [TestCase(6, 4, 6)]
        [TestCase(6, 4, 7)]
        public void ExtensionMethod(int m, int n, int aStride)
        {
            var a = Utilities.CreateRandomMatrixDouble(42, m, n, aStride);
            var lu = a.Lu();

            var reconstructed = lu.GetP() * lu.GetL() * lu.GetU();
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
            var a = Utilities.CreateRandomMatrixDouble(42, n, n, aStride);
            var b = Utilities.CreateRandomVectorDouble(57, n, bStride);
            var lu = a.Lu();
            var destination = Utilities.CreateRandomVectorDouble(66, n, dstStride);
            lu.Solve(b, destination);

            var expected = a.Svd().Solve(b);

            for (var i = 0; i < n; i++)
            {
                Assert.That(destination[i], Is.EqualTo(expected[i]).Within(1.0E-12));
            }

            Utilities.FailIfOutOfRangeWrite(destination);
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
            var a = Utilities.CreateRandomMatrixDouble(42, n, n, aStride);
            var b = Utilities.CreateRandomVectorDouble(57, n, bStride);
            var lu = a.Lu();
            var destination = lu.Solve(b);

            var expected = a.Svd().Solve(b);

            for (var i = 0; i < n; i++)
            {
                Assert.That(destination[i], Is.EqualTo(expected[i]).Within(1.0E-12));
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }
    }
}
