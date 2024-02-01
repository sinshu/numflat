using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class Mat_Math_SingleTest
    {
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 6)]
        [TestCase(5, 6)]
        public void Determinant(int n, int xStride)
        {
            var x = Utilities.CreateRandomMatrixSingle(42, n, n, xStride);
            var actual = Mat.Determinant(x);

            var mathNet = new DenseMatrix(n, n);
            for (var row = 0; row < n; row++)
            {
                for (var col = 0; col < n; col++)
                {
                    mathNet[row, col] = x[row, col];
                }
            }
            var expected = mathNet.Determinant();

            Assert.That(expected, Is.EqualTo(actual).Within(1.0E-6));
        }

        [TestCase(1, 1, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 3)]
        [TestCase(3, 4, 5)]
        [TestCase(4, 6, 5)]
        public void Inverse(int n, int xStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixSingle(42, n, n, xStride);
            var destination = Utilities.CreateRandomMatrixSingle(0, n, n, dstStride);
            Mat.Inverse(x, destination);

            var identity = destination * x;
            for (var row = 0; row < n; row++)
            {
                for (var col = 0; col < n; col++)
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

            Utilities.FailIfOutOfRangeWrite(destination);
        }
    }
}
