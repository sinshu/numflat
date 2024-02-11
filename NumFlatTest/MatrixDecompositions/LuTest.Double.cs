using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class LuTest_Double
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
            var a = TestMatrix.RandomDouble(42, m, n, aStride);
            var l = TestMatrix.RandomDouble(57, m, Math.Min(m, n), lStride);
            var u = TestMatrix.RandomDouble(66, Math.Min(m, n), n, uStride);
            var piv = new int[Math.Min(m, n)];

            using (a.EnsureUnchanged())
            {
                LuDecompositionDouble.Decompose(a, l, u, piv);
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
            var a = TestMatrix.RandomDouble(42, m, n, aStride);

            LuDecompositionDouble lu;
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
            var a = TestMatrix.RandomDouble(42, n, n, aStride);
            var b = TestVector.RandomDouble(57, a.RowCount, bStride);
            var lu = a.Lu();

            var actual = TestVector.RandomDouble(66, a.ColCount, dstStride);
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
            var a = TestMatrix.RandomDouble(42, n, n, n);
            Assert.That(a.Lu().Determinant(), Is.EqualTo(a.Determinant()).Within(1.0E-12));
        }
    }
}
