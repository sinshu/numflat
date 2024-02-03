using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class QrDouble_Test
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
            var a = Utilities.CreateRandomMatrixDouble(42, m, n, aStride);
            var q = Utilities.CreateRandomMatrixDouble(57, m, n, qStride);
            var r = Utilities.CreateRandomMatrixDouble(66, n, n, rStride);

            QrDouble.Decompose(a, q, r);

            var reconstructed = q * r;
            for (var row = 0; row < reconstructed.RowCount; row++)
            {
                for (var col = 0; col < reconstructed.ColCount; col++)
                {
                    var actual = reconstructed[row, col];
                    var expected = a[row, col];
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(q);
            Utilities.FailIfOutOfRangeWrite(r);
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
            var a = Utilities.CreateRandomMatrixDouble(42, m, n, aStride);
            var qr = a.Qr();

            var reconstructed = qr.Q * qr.R;
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
