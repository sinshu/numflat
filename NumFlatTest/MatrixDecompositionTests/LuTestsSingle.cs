﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.MatrixDecompositionTests
{
    public class LuTestsSingle
    {
        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 2, 5)]
        [TestCase(2, 2, 2, 2, 2)]
        [TestCase(2, 2, 4, 3, 3)]
        [TestCase(3, 4, 3, 3, 4)]
        [TestCase(3, 4, 5, 7, 6)]
        [TestCase(4, 3, 4, 4, 3)]
        [TestCase(4, 3, 6, 7, 5)]
        public void Decompose(int m, int n, int aStride, int lStride, int uStride)
        {
            var a = TestMatrix.RandomSingle(42, m, n, aStride);
            var l = TestMatrix.RandomSingle(57, m, Math.Min(m, n), lStride);
            var u = TestMatrix.RandomSingle(66, Math.Min(m, n), n, uStride);
            var piv = new int[m];

            using (a.EnsureUnchanged())
            {
                LuDecompositionSingle.Decompose(a, l, u, piv);
            }

            var reconstructed = a.Lu().GetPermutationMatrix() * l * u;
            NumAssert.AreSame(a, reconstructed, 1.0E-6F);

            TestMatrix.FailIfOutOfRangeWrite(l);
            TestMatrix.FailIfOutOfRangeWrite(u);
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
            var a = TestMatrix.RandomSingle(42, m, n, aStride);

            LuDecompositionSingle lu;
            using (a.EnsureUnchanged())
            {
                lu = a.Lu();
            }

            var reconstructed = lu.GetPermutationMatrix() * lu.L * lu.U;
            NumAssert.AreSame(a, reconstructed, 1.0E-6F);
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
        public void Solve(int n, int aStride, int bStride, int dstStride)
        {
            var a = TestMatrix.RandomSingle(42, n, n, aStride);
            var b = TestVector.RandomSingle(57, a.RowCount, bStride);
            var lu = a.Lu();

            var actual = TestVector.RandomSingle(66, a.ColCount, dstStride);
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                lu.Solve(b, actual);
            }

            var expected = a.Svd().Solve(b);

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
            Assert.That(a.Lu().Determinant(), Is.EqualTo(a.Determinant()).Within(1.0E-6F));
        }
    }
}
