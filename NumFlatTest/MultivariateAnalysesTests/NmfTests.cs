using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.MultivariateAnalyses;

namespace NumFlatTest.MultivariateAnalysesTests
{
    public class NmfTests
    {
        [Test]
        public void PerfectDecompositionDoesNotChange()
        {
            var random = new Random(42);

            var w = new Mat<double>(5, 3);
            var h = new Mat<double>(3, 100);
            w.MapInplace(x => random.NextDouble());
            h.MapInplace(x => random.NextDouble());

            // Now W and H are the perfect decomposition of V.
            var v = w * h;

            var w2 = new Mat<double>(5, 3);
            var h2 = new Mat<double>(3, 100);
            NonnegativeMatrixFactorization.Update(v.Cols, w, h, w2, h2);

            NumAssert.AreSame(w, w2, 1.0E-15);
            NumAssert.AreSame(h, h2, 1.0E-15);
        }

        [TestCase(42, 5, 3, 100)]
        [TestCase(57, 6, 4, 88)]
        public void ErrorDecreases1(int seed, int dimension, int componentCount, int dataCount)
        {
            var random = new Random(seed);

            var w = new Mat<double>(dimension, componentCount);
            var h = new Mat<double>(componentCount, dataCount);
            w.MapInplace(x => random.NextDouble());
            h.MapInplace(x => random.NextDouble());

            var v = new Mat<double>(dimension, dataCount);
            v.MapInplace(x => random.NextDouble());

            var error = (v - w * h).FrobeniusNorm();
            for (var i = 0; i < 100; i++)
            {
                var w2 = new Mat<double>(dimension, componentCount);
                var h2 = new Mat<double>(componentCount, dataCount);
                NonnegativeMatrixFactorization.Update(v.Cols, w, h, w2, h2);
                w2.CopyTo(w);
                h2.CopyTo(h);

                var newError = (v - w * h).FrobeniusNorm();
                Assert.That(newError < error);
                error = newError;
            }
        }
    }
}
