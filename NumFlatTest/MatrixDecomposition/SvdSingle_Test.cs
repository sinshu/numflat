using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class SvdSingle_Test
    {
        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4)]
        [TestCase(2, 2, 2, 1)]
        [TestCase(2, 2, 4, 2)]
        [TestCase(3, 4, 3, 1)]
        [TestCase(3, 4, 5, 5)]
        [TestCase(4, 3, 4, 1)]
        [TestCase(4, 3, 6, 3)]
        public void GetSingularValues(int m, int n, int aStride, int sStride)
        {
            var a = Utilities.CreateRandomMatrixSingle(42, m, n, aStride);
            var actual = Utilities.CreateRandomVectorSingle(57, Math.Min(m, n), sStride);
            SvdSingle.GetSingularValues(a, actual);

            var expected = a.Svd().S;

            for (var i = 0; i < expected.Count; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]).Within(1.0E-6));
            }

            Utilities.FailIfOutOfRangeWrite(a);
            Utilities.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(4, 3, 4)]
        [TestCase(4, 3, 6)]
        public void GetSingularValues_ExtensionMethod(int m, int n, int aStride)
        {
            var a = Utilities.CreateRandomMatrixSingle(42, m, n, aStride);
            var actual = a.GetSingularValues();

            var expected = a.Svd().S;

            for (var i = 0; i < expected.Count; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]).Within(1.0E-6));
            }

            Utilities.FailIfOutOfRangeWrite(a);
            Utilities.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4, 2, 5)]
        [TestCase(2, 2, 2, 1, 2, 2)]
        [TestCase(2, 2, 4, 2, 3, 3)]
        [TestCase(3, 4, 3, 1, 3, 4)]
        [TestCase(3, 4, 5, 5, 7, 6)]
        [TestCase(4, 3, 4, 1, 4, 3)]
        [TestCase(4, 3, 6, 3, 7, 5)]
        public void Decompose(int m, int n, int aStride, int sStride, int uStride, int vtStride)
        {
            var a = Utilities.CreateRandomMatrixSingle(42, m, n, aStride);
            var s = Utilities.CreateRandomVectorSingle(57, Math.Min(m, n), sStride);
            var u = Utilities.CreateRandomMatrixSingle(66, m, m, uStride);
            var vt = Utilities.CreateRandomMatrixSingle(77, n, n, vtStride);

            SvdSingle.Decompose(a, s, u, vt);

            var reconstructed = u * s.ToDiagonalMatrix(m, n) * vt;
            for (var row = 0; row < reconstructed.RowCount; row++)
            {
                for (var col = 0; col < reconstructed.ColCount; col++)
                {
                    var actual = reconstructed[row, col];
                    var expected = a[row, col];
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
                }
            }

            CheckUnitary(u);
            CheckUnitary(vt);

            Utilities.FailIfOutOfRangeWrite(a);
            Utilities.FailIfOutOfRangeWrite(s);
            Utilities.FailIfOutOfRangeWrite(u);
            Utilities.FailIfOutOfRangeWrite(vt);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 4, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(4, 3, 4)]
        [TestCase(4, 3, 6)]
        public void ExtensionMethod(int m, int n, int aStride)
        {
            var a = Utilities.CreateRandomMatrixSingle(42, m, n, aStride);
            var svd = a.Svd();

            var reconstructed = svd.U * svd.S.ToDiagonalMatrix(m, n) * svd.VT;
            for (var row = 0; row < reconstructed.RowCount; row++)
            {
                for (var col = 0; col < reconstructed.ColCount; col++)
                {
                    var actual = reconstructed[row, col];
                    var expected = a[row, col];
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
                }
            }

            CheckUnitary(svd.U);
            CheckUnitary(svd.VT);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 2, 4, 3)]
        [TestCase(2, 2, 2, 1, 1)]
        [TestCase(2, 2, 4, 9, 3)]
        [TestCase(3, 3, 3, 1, 1)]
        [TestCase(3, 3, 4, 2, 2)]
        [TestCase(3, 2, 3, 1, 1)]
        [TestCase(3, 2, 4, 5, 2)]
        [TestCase(2, 3, 2, 1, 1)]
        [TestCase(2, 3, 5, 2, 3)]
        [TestCase(6, 3, 7, 4, 2)]
        [TestCase(3, 7, 4, 3, 3)]
        public void Solve_Arg2(int m, int n, int aStride, int bStride, int dstStride)
        {
            var a = Utilities.CreateRandomMatrixSingle(42, m, n, aStride);
            var b = Utilities.CreateRandomVectorSingle(57, a.RowCount, bStride);
            var svd = a.Svd();
            var destination = Utilities.CreateRandomVectorSingle(66, a.ColCount, dstStride);
            svd.Solve(b, destination);

            var ma = Utilities.ToMathNet(a);
            var mb = Utilities.ToMathNet(b);
            var msvd = ma.Svd();
            var expected = msvd.Solve(mb);

            for (var i = 0; i < expected.Count; i++)
            {
                Assert.That(destination[i], Is.EqualTo(expected[i]).Within(1.0E-5));
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 2, 4)]
        [TestCase(2, 2, 2, 1)]
        [TestCase(2, 2, 4, 9)]
        [TestCase(3, 3, 3, 1)]
        [TestCase(3, 3, 4, 2)]
        [TestCase(3, 2, 3, 1)]
        [TestCase(3, 2, 4, 5)]
        [TestCase(2, 3, 2, 1)]
        [TestCase(2, 3, 5, 2)]
        [TestCase(6, 3, 7, 4)]
        [TestCase(3, 7, 4, 3)]
        public void Solve_Arg1(int m, int n, int aStride, int bStride)
        {
            var a = Utilities.CreateRandomMatrixSingle(42, m, n, aStride);
            var b = Utilities.CreateRandomVectorSingle(57, a.RowCount, bStride);
            var svd = a.Svd();
            var actual = svd.Solve(b);

            var ma = Utilities.ToMathNet(a);
            var mb = Utilities.ToMathNet(b);
            var msvd = ma.Svd();
            var expected = msvd.Solve(mb);

            for (var i = 0; i < expected.Count; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]).Within(1.0E-5));
            }
        }

        private static void CheckUnitary(Mat<float> mat)
        {
            var identity = mat * mat.Transpose();
            for (var row = 0; row < identity.RowCount; row++)
            {
                for (var col = 0; col < identity.ColCount; col++)
                {
                    if (row == col)
                    {
                        Assert.That(identity[row, col], Is.EqualTo(1.0).Within(1.0E-6));
                    }
                    else
                    {
                        Assert.That(identity[row, col], Is.EqualTo(0.0).Within(1.0E-6));
                    }
                }
            }
        }
    }
}
