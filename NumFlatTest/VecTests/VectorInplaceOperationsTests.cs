using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class VectorInplaceOperationsTests
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
        public void AddInplace_Scalar()
        {
            Vec<double> x = [1, 2, 3];
            var y = 0.5;

            var expected = x + y * VectorBuilder.Fill(3, 1.0);

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
        public void SubInplace_Scalar()
        {
            Vec<double> x = [1, 2, 3];
            var y = 0.5;

            var expected = x - y * VectorBuilder.Fill(3, 1.0);

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

        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 1)]
        [TestCase(3, 4)]
        [TestCase(4, 1)]
        [TestCase(4, 2)]
        [TestCase(5, 1)]
        [TestCase(5, 3)]
        public void Reverse(int count, int xStride)
        {
            var x = TestVector.RandomDouble(42, count, xStride);

            var expected = x.ToArray().Reverse().ToVector();

            x.ReverseInplace();

            NumAssert.AreSame(expected, x, 0);

            TestVector.FailIfOutOfRangeWrite(x);
        }

        [Test]
        public void MapInplace()
        {
            Vec<double> x = [1, 2, 3];

            var expected = x.Map(value => value * value);

            x.MapInplace(value => value * value);

            NumAssert.AreSame(expected, x, 1.0E-12);
        }

        [Test]
        public void Conjugate()
        {
            Vec<Complex> x = [new Complex(1, 2), new Complex(3, 4), new Complex(5, 6)];

            var expected = x.Conjugate();

            x.ConjugateInplace();

            NumAssert.AreSame(expected, x, 0);
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
