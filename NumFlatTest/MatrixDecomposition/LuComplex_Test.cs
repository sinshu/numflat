using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class LuComplex_Test
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
            var a = TestMatrix.RandomComplex(42, m, n, aStride);

            LuDecompositionComplex lu;
            using (a.EnsureUnchanged())
            {
                lu = a.Lu();
            }

            var reconstructed = lu.GetP() * lu.GetL() * lu.GetU();
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
        public void Solve_Arg2(int n, int aStride, int bStride, int dstStride)
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
            var a = TestMatrix.RandomComplex(42, n, n, aStride);
            var b = TestVector.RandomComplex(57, a.RowCount, bStride);
            var lu = a.Lu();

            Vec<Complex> actual;
            using (a.EnsureUnchanged())
            using (b.EnsureUnchanged())
            {
                actual = lu.Solve(b);
            }

            var expected = a.Svd().Solve(b);

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }
    }
}
