using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.VecTests
{
    public class L1NormTests
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
            var expected = mx.L1Norm();

            float actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L1Norm();
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
            var expected = mx.L1Norm();

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L1Norm();
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
            var expected = mx.L1Norm();

            double actual;
            using (x.EnsureUnchanged())
            {
                actual = x.L1Norm();
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }
    }
}
