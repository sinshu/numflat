﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.MatrixDecompositionTests
{
    public class LuTestsComplex
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
            var a = TestMatrix.RandomComplex(42, m, n, aStride);
            var l = TestMatrix.RandomComplex(57, m, Math.Min(m, n), lStride);
            var u = TestMatrix.RandomComplex(66, Math.Min(m, n), n, uStride);
            var piv = new int[m];

            using (a.EnsureUnchanged())
            {
                LuDecompositionComplex.Decompose(a, l, u, piv);
            }

            var reconstructed = a.Lu().GetPermutationMatrix() * l * u;
            NumAssert.AreSame(a, reconstructed, 1.0E-12);

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
            var a = TestMatrix.RandomComplex(42, m, n, aStride);

            LuDecompositionComplex lu;
            using (a.EnsureUnchanged())
            {
                lu = a.Lu();
            }

            var reconstructed = lu.GetPermutationMatrix() * lu.L * lu.U;
            NumAssert.AreSame(a, reconstructed, 1.0E-12);
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
            var a = TestMatrix.RandomComplex(42, n, n, aStride);
            var b = TestVector.RandomComplex(57, a.RowCount, bStride);
            var lu = a.Lu();

            var actual = TestVector.RandomComplex(66, a.ColCount, dstStride);
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                lu.Solve(b, actual);
            }

            var expected = a.Svd().Solve(b);

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Determinant(int n)
        {
            var a = TestMatrix.RandomComplex(42, n, n, n);
            var actual = a.Lu().Determinant();
            var expected = a.Determinant();
            Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
            Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
        }
    }
}
