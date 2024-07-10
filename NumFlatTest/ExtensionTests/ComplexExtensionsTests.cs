using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.ExtensionTests
{
    public class ComplexExtensionsTests
    {
        [TestCase(0.0, 0.0)]
        [TestCase(1.0, 0.0)]
        [TestCase(1.0, 2.0)]
        [TestCase(-2.0, -3.0)]
        [TestCase(4.0, -5.0)]
        public void Conjugate(double real, double imaginary)
        {
            var value = new Complex(real, imaginary);
            var conjugated = value.Conjugate();
            Assert.That(conjugated.Real, Is.EqualTo(real).Within(1.0E-12));
            Assert.That(conjugated.Imaginary, Is.EqualTo(-imaginary).Within(1.0E-12));
        }

        [TestCase(0.0, 0.0)]
        [TestCase(1.0, 0.0)]
        [TestCase(1.0, 2.0)]
        [TestCase(-2.0, -3.0)]
        [TestCase(4.0, -5.0)]
        public void MagnitudeSquared(double real, double imaginary)
        {
            var value = new Complex(real, imaginary);
            var actual = ComplexExtensions.MagnitudeSquared(value);
            var expected = real * real + imaginary * imaginary;
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }
    }
}
