﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class SvdTest_Single
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
            var a = TestMatrix.RandomSingle(42, m, n, aStride);

            var actual = TestVector.RandomSingle(57, Math.Min(m, n), sStride);
            using (a.EnsureUnchanged())
            {
                SingularValueDecompositionSingle.GetSingularValues(a, actual);
            }

            var expected = Interop.ToMathNet(a).Svd().S;

            NumAssert.AreSame(expected, actual, 1.0E-6F);

            TestVector.FailIfOutOfRangeWrite(actual);
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
            var a = TestMatrix.RandomSingle(42, m, n, aStride);

            Vec<float> actual;
            using (a.EnsureUnchanged())
            {
                actual = a.GetSingularValues();
            }

            var expected = Interop.ToMathNet(a).Svd().S;

            NumAssert.AreSame(expected, actual, 1.0E-6F);
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
            var a = TestMatrix.RandomSingle(42, m, n, aStride);
            var s = TestVector.RandomSingle(57, Math.Min(m, n), sStride);
            var u = TestMatrix.RandomSingle(66, m, m, uStride);
            var vt = TestMatrix.RandomSingle(77, n, n, vtStride);

            using (a.EnsureUnchanged())
            {
                SingularValueDecompositionSingle.Decompose(a, s, u, vt);
            }

            var reconstructed = u * s.ToDiagonalMatrix(m, n) * vt;
            NumAssert.AreSame(a, reconstructed, 1.0E-6F);

            CheckUnitary(u);
            CheckUnitary(vt);

            TestVector.FailIfOutOfRangeWrite(s);
            TestMatrix.FailIfOutOfRangeWrite(u);
            TestMatrix.FailIfOutOfRangeWrite(vt);
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
            var a = TestMatrix.RandomSingle(42, m, n, aStride);

            SingularValueDecompositionSingle svd;
            using (a.EnsureUnchanged())
            {
                svd = a.Svd();
            }

            var reconstructed = svd.U * svd.S.ToDiagonalMatrix(m, n) * svd.VT;
            NumAssert.AreSame(a, reconstructed, 1.0E-6F);

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
        public void Solve(int m, int n, int aStride, int bStride, int dstStride)
        {
            var a = TestMatrix.RandomSingle(42, m, n, aStride);
            var b = TestVector.RandomSingle(57, a.RowCount, bStride);
            var svd = a.Svd();

            var ma = Interop.ToMathNet(a);
            var mb = Interop.ToMathNet(b);
            var expected = ma.Svd().Solve(mb);

            var actual = TestVector.RandomSingle(66, a.ColCount, dstStride);
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                svd.Solve(b, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-5F); // 1.0E-6F is too small.

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Determinant(int n)
        {
            var a = TestMatrix.RandomSingle(42, n, n, n);
            var svd = a.Svd();
            var expected = Math.Abs(a.Determinant());
            Assert.That(svd.Determinant(), Is.EqualTo(expected).Within(1.0E-6F));
            Assert.That(svd.LogDeterminant(), Is.EqualTo(Math.Log(expected)).Within(1.0E-5F)); // 1.0E-6F is too small.
        }

        private static void CheckUnitary(Mat<float> mat)
        {
            var actual = mat * mat.Transpose();
            var expected = MatrixBuilder.Identity<float>(actual.RowCount);
            NumAssert.AreSame(expected, actual, 1.0E-6F);
        }
    }
}
