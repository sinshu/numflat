using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class LuDouble_Test
    {
        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4, 2)]
        [TestCase(2, 2, 2, 2, 2)]
        [TestCase(2, 2, 3, 4, 3)]
        [TestCase(3, 3, 3, 3, 3)]
        [TestCase(3, 3, 4, 4, 5)]
        [TestCase(4, 4, 4, 4, 4)]
        [TestCase(4, 4, 5, 5, 5)]
        [TestCase(5, 5, 5, 5, 5)]
        [TestCase(5, 5, 7, 6, 6)]
        [TestCase(3, 7, 3, 3, 3)]
        [TestCase(3, 7, 4, 5, 4)]
        [TestCase(6, 4, 6, 6, 6)]
        [TestCase(6, 4, 7, 7, 7)]
        public void Decompose(int m, int n, int aStride, int lStride, int uStride)
        {
            var a = Utilities.CreateRandomMatrixDouble(42, m, n, aStride);
            var min = Math.Min(m, n);
            var l = Utilities.CreateRandomMatrixDouble(0, m, min, lStride);
            var u = Utilities.CreateRandomMatrixDouble(0, min, n, uStride);
            var permutation = new int[m];
            LuDouble.Decompose(a, l, u, permutation);
            var p = LuDouble.GetP(permutation);

            var reconstructed = p * l * u;
            for (var row = 0; row < reconstructed.RowCount; row++)
            {
                for (var col = 0; col < reconstructed.ColCount; col++)
                {
                    var actual = reconstructed[row, col];
                    var expected = a[row, col];
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(l);
            Utilities.FailIfOutOfRangeWrite(u);
        }

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

            var reconstructed = lu.GetP() * lu.L * lu.U;
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
    }
}
