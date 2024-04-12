using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.VecTests
{
    public class NormTests
    {
        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void Single(int count, int stride)
        {
            var x = TestVector.RandomSingle(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.L2Norm();

            float actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Norm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6F));
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void Double(int count, int stride)
        {
            var x = TestVector.RandomDouble(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.L2Norm();

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Norm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 2)]
        [TestCase(7, 3)]
        public void Complex(int count, int stride)
        {
            var x = TestVector.RandomComplex(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.L2Norm();

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Norm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 2)]
        [TestCase(1, 2, 3)]
        [TestCase(2, 1, 4)]
        [TestCase(2, 2, 2)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void Single(int count, int stride, float p)
        {
            var x = TestVector.RandomSingle(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Norm(p);

            float actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Norm(p);
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6F));
        }

        [TestCase(1, 1, 2)]
        [TestCase(1, 2, 3)]
        [TestCase(2, 1, 4)]
        [TestCase(2, 2, 2)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void Double(int count, int stride, double p)
        {
            var x = TestVector.RandomDouble(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Norm(p);

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Norm(p);
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 2)]
        [TestCase(1, 2, 3)]
        [TestCase(2, 1, 4)]
        [TestCase(2, 2, 2)]
        [TestCase(5, 2, 3)]
        [TestCase(7, 3, 4)]
        public void Complex(int count, int stride, double p)
        {
            var x = TestVector.RandomComplex(42, count, stride);

            var mx = Interop.ToMathNet(x);
            var expected = mx.Norm(p);

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.Norm(p);
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }
    }
}
