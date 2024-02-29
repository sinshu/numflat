using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.SignalProcessing;

namespace NumFlatTest.SignalProcessingTests
{
    public class WindowFunctionTests
    {
        [TestCase(64)]
        [TestCase(100)]
        public void Hann(int length)
        {
            var window = WindowFunctions.Hann(length);

            Assert.That(window.First(), Is.EqualTo(0.0).Within(1.0E-12));
            Assert.That(window[length / 2], Is.EqualTo(1.0).Within(1.0E-12));
            Assert.That(window.Last(), Is.EqualTo(0.0).Within(0.05));
        }

        [TestCase(64)]
        [TestCase(100)]
        public void HannSquared(int length)
        {
            var window = WindowFunctions.HannSquared(length);

            Assert.That(window.First(), Is.EqualTo(0.0).Within(1.0E-12));
            Assert.That(window[length / 2], Is.EqualTo(1.0).Within(1.0E-12));
            Assert.That(window.Last(), Is.EqualTo(0.0).Within(0.05));
        }

        [TestCase(64)]
        [TestCase(100)]
        public void Hamming(int length)
        {
            var window = WindowFunctions.Hamming(length);

            Assert.That(window.First(), Is.EqualTo(0.0).Within(0.1));
            Assert.That(window[length / 2], Is.EqualTo(1.0).Within(0.1));
            Assert.That(window.Last(), Is.EqualTo(0.0).Within(0.1));
        }
    }
}
