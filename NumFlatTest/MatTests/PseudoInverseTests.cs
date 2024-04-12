using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.MatTests
{
    public class PseudoInverseTests
    {
        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 5)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 4, 3)]
        [TestCase(3, 2, 3, 2)]
        [TestCase(3, 2, 5, 4)]
        [TestCase(2, 3, 2, 3)]
        [TestCase(2, 3, 4, 4)]
        [TestCase(6, 3, 7, 4)]
        [TestCase(4, 7, 5, 9)]
        public void PseudoInverseSingle(int rowCount, int colCount, int aStride, int dstStride)
        {
            var a = TestMatrix.RandomSingle(42, rowCount, colCount, aStride);

            var ma = Interop.ToMathNet(a);
            var expected = ma.PseudoInverse();

            var actual = TestMatrix.RandomSingle(0, colCount, rowCount, dstStride);
            using (a.EnsureUnchanged())
            {
                Mat.PseudoInverse(a, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-5F); // 1.0E-6F is too small.

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [Test]
        public void PseudoInverseToleranceSingle()
        {
            var a = new float[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            }
            .ToMatrix();

            var result1 = new float[,]
            {
                { -0.63888889F, -0.16666667F, 0.30555556F },
                { -0.05555556F, 0.00000000F, 0.05555556F },
                { 0.52777778F, 0.16666667F, -0.19444444F },
            }
            .ToMatrix();

            var result2 = new float[,]
            {
                { 0.00611649F, 0.0148213F, 0.02352611F },
                { 0.0072985F, 0.01768552F, 0.02807254F },
                { 0.00848052F, 0.02054974F, 0.03261896F },
            }
            .ToMatrix();

            using (a.EnsureUnchanged())
            {
                NumAssert.AreSame(result1, a.PseudoInverse(0.5F), 1.0E-6F);
                NumAssert.AreSame(result2, a.PseudoInverse(5F), 1.0E-6F);
                NumAssert.AreSame(new Mat<float>(3, 3), a.PseudoInverse(20F), 1.0E-6F);
            }
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 5)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 4, 3)]
        [TestCase(3, 2, 3, 2)]
        [TestCase(3, 2, 5, 4)]
        [TestCase(2, 3, 2, 3)]
        [TestCase(2, 3, 4, 4)]
        [TestCase(6, 3, 7, 4)]
        [TestCase(4, 7, 5, 9)]
        public void PseudoInverseDouble(int rowCount, int colCount, int aStride, int dstStride)
        {
            var a = TestMatrix.RandomDouble(42, rowCount, colCount, aStride);

            var ma = Interop.ToMathNet(a);
            var expected = ma.PseudoInverse();

            var actual = TestMatrix.RandomDouble(0, colCount, rowCount, dstStride);
            using (a.EnsureUnchanged())
            {
                Mat.PseudoInverse(a, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [Test]
        public void PseudoInverseToleranceDouble()
        {
            var a = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            }
            .ToMatrix();

            var result1 = new double[,]
            {
                { -0.63888889, -0.16666667, 0.30555556 },
                { -0.05555556, 0.00000000, 0.05555556 },
                { 0.52777778, 0.16666667, -0.19444444 },
            }
            .ToMatrix();

            var result2 = new double[,]
            {
                { 0.00611649, 0.0148213, 0.02352611 },
                { 0.0072985, 0.01768552, 0.02807254 },
                { 0.00848052, 0.02054974, 0.03261896 },
            }
            .ToMatrix();

            using (a.EnsureUnchanged())
            {
                NumAssert.AreSame(result1, a.PseudoInverse(0.5), 1.0E-6);
                NumAssert.AreSame(result2, a.PseudoInverse(5), 1.0E-6);
                NumAssert.AreSame(new Mat<double>(3, 3), a.PseudoInverse(20), 1.0E-6);
            }
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 5)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 4, 3)]
        [TestCase(3, 2, 3, 2)]
        [TestCase(3, 2, 5, 4)]
        [TestCase(2, 3, 2, 3)]
        [TestCase(2, 3, 4, 4)]
        [TestCase(6, 3, 7, 4)]
        [TestCase(4, 7, 5, 9)]
        public void PseudoInverseComplex(int rowCount, int colCount, int aStride, int dstStride)
        {
            var a = TestMatrix.RandomComplex(42, rowCount, colCount, aStride);

            var ma = Interop.ToMathNet(a);
            var expected = ma.PseudoInverse();

            var actual = TestMatrix.RandomComplex(0, colCount, rowCount, dstStride);
            using (a.EnsureUnchanged())
            {
                Mat.PseudoInverse(a, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [Test]
        public void PseudoInverseToleranceComplex()
        {
            var a = new Complex[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            }
            .ToMatrix();

            var result1 = new Complex[,]
            {
                { -0.63888889, -0.16666667, 0.30555556 },
                { -0.05555556, 0.00000000, 0.05555556 },
                { 0.52777778, 0.16666667, -0.19444444 },
            }
            .ToMatrix();

            var result2 = new Complex[,]
            {
                { 0.00611649, 0.0148213, 0.02352611 },
                { 0.0072985, 0.01768552, 0.02807254 },
                { 0.00848052, 0.02054974, 0.03261896 },
            }
            .ToMatrix();

            using (a.EnsureUnchanged())
            {
                NumAssert.AreSame(result1, a.PseudoInverse(0.5), 1.0E-6);
                NumAssert.AreSame(result2, a.PseudoInverse(5), 1.0E-6);
                NumAssert.AreSame(new Mat<Complex>(3, 3), a.PseudoInverse(20), 1.0E-6);
            }
        }
    }
}
