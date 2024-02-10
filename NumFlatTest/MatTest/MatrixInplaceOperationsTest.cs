using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MatrixInplaceOperationTest
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
    }
}
