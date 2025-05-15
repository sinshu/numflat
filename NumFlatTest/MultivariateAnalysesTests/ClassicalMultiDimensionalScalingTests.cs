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
    public class ClassicalMultiDimensionalScalingTests
    {
        [Test]
        public void Test()
        {
            var positions = new List<Vec<double>>();
            var random = new Random(42);
            var n = 20;

            for (var i = 0; i < n; i++)
            {
                Vec<double> pos = [3 * random.NextGaussian(), 5 * random.NextGaussian()];
                positions.Add(pos);
            }

            var distanceMatrix = new Mat<double>(n, n);
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    distanceMatrix[i, j] = positions[i].Distance(positions[j]);
                }
            }

            var x = ClassicalMultiDimensionalScaling.Fit(distanceMatrix);

            var reconstructed = new Mat<double>(n, n);
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    reconstructed[i, j] = x.Rows[i].Distance(x.Rows[j]);
                }
            }

            NumAssert.AreSame(distanceMatrix, reconstructed, 1.0E-6);
        }
    }
}
