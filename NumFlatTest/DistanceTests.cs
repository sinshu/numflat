using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class DistanceTests
    {
        [Test]
        public void Euclidean()
        {
            Vec<double> x = [1, 2, 3];
            Vec<double> y = [1, 3, 7];

            var expected = (x - y).Norm();
            var actual = Distance.Euclidean(x, y);
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [Test]
        public void Manhattan()
        {
            Vec<double> x = [1, 2, 3];
            Vec<double> y = [1, 3, 7];

            var expected = (x - y).L1Norm();
            var actual = Distance.Manhattan(x, y);
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }
    }
}
