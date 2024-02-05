using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

using MMat = MathNet.Numerics.LinearAlgebra.Matrix<float>;

namespace NumFlatTest
{
    public class Mat_Math_Single_Mul_Test
    {
        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(1, 1, 1, 2, 3, 4)]
        [TestCase(2, 2, 2, 2, 2, 2)]
        [TestCase(2, 2, 2, 5, 4, 3)]
        [TestCase(2, 4, 3, 5, 6, 5)]
        [TestCase(5, 4, 3, 5, 5, 5)]
        [TestCase(8, 7, 9, 8, 9, 8)]
        [TestCase(7, 8, 9, 10, 10, 10)]
        public void Mul_MatMat(int m, int n, int k, int xStride, int yStride, int dstStride)
        {
            Func<MMat, MMat> none = mat => mat;
            Func<MMat, MMat> transpose = mat => mat.Transpose();

            bool[] xTransposeConditions = [false, true];
            bool[] yTransposeConditions = [false, true];

            foreach (var xTranspose in xTransposeConditions)
            {
                foreach (var yTranspose in yTransposeConditions)
                {
                    var xArgs = xTranspose ? (k, m, yStride) : (m, k, xStride);
                    var yArgs = yTranspose ? (n, k, xStride) : (k, n, yStride);

                    var mxt = xTranspose ? transpose : none;
                    var myt = yTranspose ? transpose : none;

                    var x = TestMatrix.RandomSingle(42, xArgs.Item1, xArgs.Item2, xArgs.Item3);
                    var y = TestMatrix.RandomSingle(57, yArgs.Item1, yArgs.Item2, yArgs.Item3);

                    var mx = Interop.ToMathNet(x);
                    var my = Interop.ToMathNet(y);
                    var expected = mxt(mx) * myt(my);

                    var actual = TestMatrix.RandomSingle(0, m, n, dstStride);
                    using (x.EnsureUnchanged())
                    using (y.EnsureUnchanged())
                    {
                        Mat.Mul(x, y, actual, xTranspose, yTranspose);
                    }

                    NumAssert.AreSame(expected, actual, 1.0E-6F);

                    TestMatrix.FailIfOutOfRangeWrite(actual);
                }
            }
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4, 5)]
        [TestCase(2, 2, 2, 1, 1)]
        [TestCase(2, 2, 3, 3, 2)]
        [TestCase(3, 3, 3, 1, 1)]
        [TestCase(3, 3, 4, 7, 6)]
        [TestCase(2, 5, 2, 1, 1)]
        [TestCase(2, 5, 3, 2, 2)]
        [TestCase(7, 3, 7, 1, 1)]
        [TestCase(7, 4, 7, 2, 5)]
        public void Mul_MatVec(int rowCount, int colCount, int xStride, int yStride, int dstStride)
        {
            Func<MMat, MMat> none = mat => mat;
            Func<MMat, MMat> transpose = mat => mat.Transpose();

            bool[] xTransposeConditions = [false, true];

            foreach (var xTranspose in xTransposeConditions)
            {
                var xArgs = xTranspose ? (colCount, rowCount, colCount + xStride - rowCount) : (rowCount, colCount, xStride);

                var mxt = xTranspose ? transpose : none;

                var x = TestMatrix.RandomSingle(42, xArgs.Item1, xArgs.Item2, xArgs.Item3);
                var y = TestVector.RandomSingle(57, colCount, yStride);

                var mx = Interop.ToMathNet(x);
                var my = Interop.ToMathNet(y);
                var expected = mxt(mx) * my;

                var actual = TestVector.RandomSingle(0, rowCount, dstStride);
                using (x.EnsureUnchanged())
                using (y.EnsureUnchanged())
                {
                    Mat.Mul(x, y, actual, xTranspose);
                }

                NumAssert.AreSame(expected, actual, 1.0E-6F);

                TestVector.FailIfOutOfRangeWrite(actual);
            }
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 1, 2, 3)]
        [TestCase(2, 2, 2, 2, 2)]
        [TestCase(2, 2, 2, 5, 4)]
        [TestCase(2, 4, 3, 5, 6)]
        [TestCase(5, 4, 3, 5, 5)]
        [TestCase(8, 7, 9, 8, 9)]
        [TestCase(7, 8, 9, 10, 10)]
        public void Operator_MatMat(int m, int n, int k, int xStride, int yStride)
        {
            var x = TestMatrix.RandomSingle(42, m, k, xStride);
            var y = TestMatrix.RandomSingle(57, k, n, yStride);

            var mx = Interop.ToMathNet(x);
            var my = Interop.ToMathNet(y);

            var expected = mx * my;
            var actual = x * y;
            NumAssert.AreSame(expected, actual, 1.0E-6F);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4)]
        [TestCase(2, 2, 2, 1)]
        [TestCase(3, 3, 3, 7)]
        [TestCase(2, 3, 3, 1)]
        [TestCase(7, 3, 7, 1)]
        [TestCase(7, 4, 7, 2)]
        public void Operator_MatVec(int rowCount, int colCount, int xStride, int yStride)
        {
            var x = TestMatrix.RandomSingle(42, rowCount, colCount, xStride);
            var y = TestVector.RandomSingle(57, colCount, yStride);

            var mx = Interop.ToMathNet(x);
            var my = Interop.ToMathNet(y);

            var expected = mx * my;
            var actual = x * y;
            NumAssert.AreSame(expected, actual, 1.0E-6F);
        }
    }
}
