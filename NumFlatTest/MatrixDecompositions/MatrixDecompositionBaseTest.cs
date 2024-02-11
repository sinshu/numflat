using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using MathNet.Numerics.LinearAlgebra.Factorization;

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
    }
}
