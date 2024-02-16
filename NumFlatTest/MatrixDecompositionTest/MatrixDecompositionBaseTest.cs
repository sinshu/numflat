using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MatrixDecompositionBaseTest
    {
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
            var a = TestMatrix.RandomDouble(42, m, n, aStride);
            var b = TestVector.RandomDouble(57, a.RowCount, bStride);
            var svd = a.Svd();

            var ma = Interop.ToMathNet(a);
            var mb = Interop.ToMathNet(b);
            var expected = ma.Svd().Solve(mb);

            Vec<double> actual;
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                actual = svd.Solve(b);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 1, 2, 3)]
        [TestCase(2, 3, 3, 10, 4, 4)]
        [TestCase(4, 3, 5, 20, 5, 5)]
        public void MatSolve_Arg2(int m, int n, int aStride, int eqCount, int bStride, int dstStride)
        {
            var a = TestMatrix.RandomDouble(42, m, n, aStride);
            var b = TestMatrix.RandomDouble(57, a.RowCount, eqCount, bStride);
            var destination = TestMatrix.RandomDouble(66, a.ColCount, eqCount, dstStride);
            var svd = a.Svd();

            var ma = Interop.ToMathNet(a);
            var mb = Interop.ToMathNet(b);
            var expected = ma.Svd().Solve(mb);

            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                svd.Solve(b, destination);
            }

            NumAssert.AreSame(expected, destination, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 1, 2)]
        [TestCase(2, 3, 3, 10, 4)]
        [TestCase(4, 3, 5, 20, 5)]
        public void MatSolve_Arg1(int m, int n, int aStride, int eqCount, int bStride)
        {
            var a = TestMatrix.RandomDouble(42, m, n, aStride);
            var b = TestMatrix.RandomDouble(57, a.RowCount, eqCount, bStride);
            var svd = a.Svd();

            var ma = Interop.ToMathNet(a);
            var mb = Interop.ToMathNet(b);
            var expected = ma.Svd().Solve(mb);

            Mat<double> actual;
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                actual = svd.Solve(b);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }
    }
}
