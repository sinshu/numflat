using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.MatTests
{
    public class RankTests
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void ZeroSingle(int n)
        {
            var x = new Mat<float>(n, n);

            int rank;
            using (x.EnsureUnchanged())
            {
                rank = x.Rank();
            }

            Assert.That(rank, Is.EqualTo(0));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void OneOrMoreSingle(int n)
        {
            for (var expected = 1; expected <= n; expected++)
            {
                var src = TestMatrix.RandomSingle(42, expected, n, expected);
                var x = src * src.Transpose();

                int rank;
                using (x.EnsureUnchanged())
                {
                    rank = x.Rank();
                }

                Assert.That(rank, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ToleranceSingle()
        {
            var a = new float[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            }
            .ToMatrix();

            using (a.EnsureUnchanged())
            {
                Assert.That(a.Rank(), Is.EqualTo(2));
                Assert.That(a.Rank(5), Is.EqualTo(1));
                Assert.That(a.Rank(10), Is.EqualTo(1));
                Assert.That(a.Rank(15), Is.EqualTo(1));
                Assert.That(a.Rank(20), Is.EqualTo(0));
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void ZeroDouble(int n)
        {
            var x = new Mat<double>(n, n);

            int rank;
            using (x.EnsureUnchanged())
            {
                rank = x.Rank();
            }

            Assert.That(rank, Is.EqualTo(0));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void OneOrMoreDouble(int n)
        {
            for (var expected = 1; expected <= n; expected++)
            {
                var src = TestMatrix.RandomDouble(42, expected, n, expected);
                var x = src * src.Transpose();

                int rank;
                using (x.EnsureUnchanged())
                {
                    rank = x.Rank();
                }

                Assert.That(rank, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ToleranceDouble()
        {
            var a = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            }
            .ToMatrix();

            using (a.EnsureUnchanged())
            {
                Assert.That(a.Rank(), Is.EqualTo(2));
                Assert.That(a.Rank(5), Is.EqualTo(1));
                Assert.That(a.Rank(10), Is.EqualTo(1));
                Assert.That(a.Rank(15), Is.EqualTo(1));
                Assert.That(a.Rank(20), Is.EqualTo(0));
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void ZeroComplex(int n)
        {
            var x = new Mat<Complex>(n, n);

            int rank;
            using (x.EnsureUnchanged())
            {
                rank = x.Rank();
            }

            Assert.That(rank, Is.EqualTo(0));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void OneOrMoreComplex(int n)
        {
            for (var expected = 1; expected <= n; expected++)
            {
                var src = TestMatrix.RandomComplex(42, expected, n, expected);
                var x = src * src.Transpose();

                int rank;
                using (x.EnsureUnchanged())
                {
                    rank = x.Rank();
                }

                Assert.That(rank, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ToleranceComplex()
        {
            var a = new Complex[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
            }
            .ToMatrix();

            using (a.EnsureUnchanged())
            {
                Assert.That(a.Rank(), Is.EqualTo(2));
                Assert.That(a.Rank(5), Is.EqualTo(1));
                Assert.That(a.Rank(10), Is.EqualTo(1));
                Assert.That(a.Rank(15), Is.EqualTo(1));
                Assert.That(a.Rank(20), Is.EqualTo(0));
            }
        }
    }
}
