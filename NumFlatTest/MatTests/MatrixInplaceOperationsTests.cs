using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.MatTests
{
    public class MatrixInplaceOperationsTests
    {
        [Test]
        public void AddInplace()
        {
            var x = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            }
            .ToMatrix();

            var y = new double[,]
            {
                { 4, 5, 6 },
                { 7, 8, 9 },
            }
            .ToMatrix();

            var expected = x + y;

            x.AddInplace(y);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void AddInplaceScalar()
        {
            var x = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            }
            .ToMatrix();

            var y = 0.5;

            var expected = x + MatrixBuilder.Fill(x.RowCount, x.ColCount, y);

            x.AddInplace(y);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void SubInplace()
        {
            var x = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            }
            .ToMatrix();

            var y = new double[,]
            {
                { 4, 5, 6 },
                { 7, 8, 9 },
            }
            .ToMatrix();

            var expected = x - y;

            x.SubInplace(y);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void SubInplaceScalar()
        {
            var x = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            }
            .ToMatrix();

            var y = 0.5;

            var expected = x - MatrixBuilder.Fill(x.RowCount, x.ColCount, y);

            x.SubInplace(y);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void MulInplace()
        {
            var x = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            }
            .ToMatrix();

            double y = 42;

            var expected = x * y;

            x.MulInplace(y);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void DivInplace()
        {
            var x = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            }
            .ToMatrix();

            double y = 42;

            var expected = x / y;

            x.DivInplace(y);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void PointwiseMulInplace()
        {
            var x = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            }
            .ToMatrix();

            var y = new double[,]
            {
                { 4, 5, 6 },
                { 7, 8, 9 },
            }
            .ToMatrix();

            var expected = x.PointwiseMul(y);

            x.PointwiseMulInplace(y);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void PointwiseDivInplace()
        {
            var x = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            }
            .ToMatrix();

            var y = new double[,]
            {
                { 4, 5, 6 },
                { 7, 8, 9 },
            }
            .ToMatrix();

            var expected = x.PointwiseDiv(y);

            x.PointwiseDivInplace(y);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(2, 4)]
        [TestCase(3, 3)]
        [TestCase(3, 5)]
        public void TransposeInplace(int n, int xStride)
        {
            var x = TestMatrix.RandomDouble(42, n, n, xStride);

            var expected = x.Transpose();

            x.TransposeInplace();

            NumAssert.AreSame(expected, x, 0);

            TestMatrix.FailIfOutOfRangeWrite(x);
        }

        [Test]
        public void MapInplace()
        {
            var x = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            }
            .ToMatrix();

            var expected = x.Map(value => value * value);

            x.MapInplace(value => value * value);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void ConjugateInplace()
        {
            var x = TestMatrix.RandomComplex(42, 10, 5, 11);

            var expected = x.Conjugate();

            x.ConjugateInplace();

            NumAssert.AreSame(expected, x, 0);

            TestMatrix.FailIfOutOfRangeWrite(x);
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(2, 4)]
        [TestCase(3, 3)]
        [TestCase(3, 5)]
        public void ConjugateTransposeInplace(int n, int xStride)
        {
            var x = TestMatrix.RandomComplex(42, n, n, xStride);

            var expected = x.ConjugateTranspose();

            x.ConjugateTransposeInplace();

            NumAssert.AreSame(expected, x, 0);

            TestMatrix.FailIfOutOfRangeWrite(x);
        }

        [Test]
        public void InverseInplaceSingle()
        {
            var x = TestMatrix.RandomSingle(42, 3, 3, 3);

            var expected = x.Inverse();

            x.InverseInplace();

            NumAssert.AreSame(expected, x, 1.0E-6F);
        }

        [Test]
        public void InverseInplaceDouble()
        {
            var x = TestMatrix.RandomDouble(42, 3, 3, 3);

            var expected = x.Inverse();

            x.InverseInplace();

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void InverseInplaceComplex()
        {
            var x = TestMatrix.RandomComplex(42, 3, 3, 3);

            var expected = x.Inverse();

            x.InverseInplace();

            NumAssert.AreSame(expected, x, 1.0E-12);
        }
    }
}
