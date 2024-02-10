using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class VectorInplaceOperationsTest
    {
        [Test]
        public void AddInplace()
        {
            Vec<double> x = [1, 2, 3];
            Vec<double> y = [4, 5, 6];

            var expected = x + y;

            x.AddInplace(y);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void SubInplace()
        {
            Vec<double> x = [1, 2, 3];
            Vec<double> y = [4, 5, 6];

            var expected = x - y;

            x.SubInplace(y);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void MulInplace()
        {
            Vec<double> x = [1, 2, 3];
            double y = 4;

            var expected = x * y;

            x.MulInplace(y);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void DivInplace()
        {
            Vec<double> x = [1, 2, 3];
            double y = 4;

            var expected = x / y;

            x.DivInplace(y);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void PointwiseMulInplace()
        {
            Vec<double> x = [1, 2, 3];
            Vec<double> y = [4, 5, 6];

            var expected = x.PointwiseMul(y);

            x.PointwiseMulInplace(y);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void PointwiseDivInplace()
        {
            Vec<double> x = [1, 2, 3];
            Vec<double> y = [4, 5, 6];

            var expected = x.PointwiseDiv(y);

            x.PointwiseDivInplace(y);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void NormalizeSingle_Arg0()
        {
            Vec<float> x = [1, 2, 3];

            var expected = x.Normalize();

            x.NormalizeInplace();

            NumAssert.AreSame(expected, x, 1.0E-6F);
        }

        [Test]
        public void NormalizeSingle_Arg1()
        {
            Vec<float> x = [1, 2, 3];

            var expected = x.Normalize(3);

            x.NormalizeInplace(3);

            NumAssert.AreSame(expected, x, 1.0E-6F);
        }

        [Test]
        public void NormalizeDouble_Arg0()
        {
            Vec<double> x = [1, 2, 3];

            var expected = x.Normalize();

            x.NormalizeInplace();

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void NormalizeDouble_Arg1()
        {
            Vec<double> x = [1, 2, 3];

            var expected = x.Normalize(3);

            x.NormalizeInplace(3);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void NormalizeComplex_Arg0()
        {
            Vec<Complex> x = [new Complex(1, 4), new Complex(2, 5), new Complex(3, 6)];

            var expected = x.Normalize();

            x.NormalizeInplace();

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void NormalizeComplex_Arg1()
        {
            Vec<Complex> x = [new Complex(1, 4), new Complex(2, 5), new Complex(3, 6)];

            var expected = x.Normalize(3);

            x.NormalizeInplace(3);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }
    }
}
