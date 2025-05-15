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
        public void Test1()
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

        [Test]
        public void Test2()
        {
            var positions = new List<Vec<double>>();
            var random = new Random(57);
            var n = 30;

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

            var x = ClassicalMultiDimensionalScaling.Fit(distanceMatrix, 2);

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

        [Test]
        public void EuroDist()
        {
            Mat<double> expected =
            [
                [-2290.2746796314427, -1798.8029280852895],
                [825.3827903533338, -546.8114799819325],
                [-59.18334054586861, 367.08135246404635],
                [82.84597289698863, 429.91465818461427],
                [352.4994348881581, 290.90843282618215],
                [-293.6896331438711, 405.31194480518917],
                [-681.9315445294149, 1108.6447775310014],
                [9.423363810420726, -240.40599900079366],
                [2048.449112865862, -642.4585438589065],
                [-561.1089699422771, 773.3692895561531],
                [-164.9217994920027, 549.3670405243699],
                [1935.0408105660604, -49.125135804933905],
                [226.42323642764796, -187.0877902287921],
                [1423.3536965978371, -305.87512979117474],
                [299.49871000071585, -388.8072564773425],
                [-260.8780456660394, -416.67380908914595],
                [-587.6756789484731, -81.18224195198505],
                [156.83625680196076, 211.13911235079624],
                [-709.4132816619809, -1109.3666474677393],
                [-839.4459111695453, 1836.7905503932175],
                [-911.2305004780735, -205.9301968975312],
            ];

            var d = File
                .ReadLines(Path.Combine("dataset", "eurodist.csv"))
                .Skip(1)
                .Select(line => line.Split(',').Skip(1).Select(value => double.Parse(value)).ToArray())
                .RowsToMatrix();

            var actual = ClassicalMultiDimensionalScaling.Fit(d, 2);

            if (Math.Sign(actual[0, 0]) != Math.Sign(expected[0, 0]))
            {
                actual.Cols[0].MulInplace(-1);
            }

            if (Math.Sign(actual[0, 1]) != Math.Sign(expected[0, 1]))
            {
                actual.Cols[1].MulInplace(-1);
            }

            NumAssert.AreSame(expected, actual, 1.0E-6);
        }
    }
}
