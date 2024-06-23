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
    public class IcaTests
    {
        [Test]
        public void In2_Out2()
        {
            var n = 100;

            Mat<double> ys =
            [
                CreateTestSignal1(n),
                CreateTestSignal2(n),
            ];

            Mat<double> mixing =
            [
                [-2.3, -1.5],
                [ 0.9,  3.1],
            ];

            var xs = mixing * ys;

            var ica = xs.Cols.Ica(2, new Random(42));

            var transformed = xs.Cols.Select(x => ica.Transform(x).AsEnumerable()).ColsToMatrix();

            Assert.That(GetError(transformed.Rows[0], ys), Is.LessThan(0.03));
            Assert.That(GetError(transformed.Rows[1], ys), Is.LessThan(0.03));

            var reconstructed = transformed.Cols.Select(x => ica.InverseTransform(x).AsEnumerable()).ColsToMatrix();

            Assert.That(GetError(reconstructed.Rows[0], xs.Rows[0]), Is.LessThan(0.01));
            Assert.That(GetError(reconstructed.Rows[1], xs.Rows[1]), Is.LessThan(0.01));
        }

        [Test]
        public void In3_Out3()
        {
            var n = 300;

            Mat<double> ys =
            [
                CreateTestSignal1(n),
                CreateTestSignal2(n),
                CreateTestSignal3(n),
            ];

            Mat<double> mixing =
            [
                [ 2.3, -1.5,  1.2],
                [-0.9, -3.1,  0.8],
                [ 1.1,  0.3, -3.9],
            ];

            var xs = mixing * ys;

            var ica = xs.Cols.Ica(3, new Random(57));

            var transformed = xs.Cols.Select(x => ica.Transform(x).AsEnumerable()).ColsToMatrix();

            Assert.That(GetError(transformed.Rows[0], ys), Is.LessThan(0.03));
            Assert.That(GetError(transformed.Rows[1], ys), Is.LessThan(0.03));
            Assert.That(GetError(transformed.Rows[2], ys), Is.LessThan(0.03));

            var reconstructed = transformed.Cols.Select(x => ica.InverseTransform(x).AsEnumerable()).ColsToMatrix();

            Assert.That(GetError(reconstructed.Rows[0], xs.Rows[0]), Is.LessThan(0.01));
            Assert.That(GetError(reconstructed.Rows[1], xs.Rows[1]), Is.LessThan(0.01));
            Assert.That(GetError(reconstructed.Rows[2], xs.Rows[2]), Is.LessThan(0.01));
        }

        [Test]
        public void In4_Out3()
        {
            var n = 300;

            Mat<double> ys =
            [
                CreateTestSignal1(n),
                CreateTestSignal2(n),
                CreateTestSignal3(n),
            ];

            Mat<double> mixing =
            [
                [ 2.3, -1.5,  1.2],
                [-0.9, -3.1,  0.8],
                [ 1.1,  0.3, -3.9],
                [ 0.5, -1.2,  1.6],
            ];

            var xs = mixing * ys;

            var ica = xs.Cols.Ica(3, new Random(66));

            var transformed = xs.Cols.Select(x => ica.Transform(x).AsEnumerable()).ColsToMatrix();

            Assert.That(GetError(transformed.Rows[0], ys), Is.LessThan(0.03));
            Assert.That(GetError(transformed.Rows[1], ys), Is.LessThan(0.03));
            Assert.That(GetError(transformed.Rows[2], ys), Is.LessThan(0.03));

            var reconstructed = transformed.Cols.Select(x => ica.InverseTransform(x).AsEnumerable()).ColsToMatrix();

            Assert.That(GetError(reconstructed.Rows[0], xs.Rows[0]), Is.LessThan(0.01));
            Assert.That(GetError(reconstructed.Rows[1], xs.Rows[1]), Is.LessThan(0.01));
            Assert.That(GetError(reconstructed.Rows[2], xs.Rows[2]), Is.LessThan(0.01));
            Assert.That(GetError(reconstructed.Rows[3], xs.Rows[3]), Is.LessThan(0.01));
        }

        [Test]
        public void In5_Out2()
        {
            var n = 100;

            Mat<double> ys =
            [
                CreateTestSignal1(n),
                CreateTestSignal2(n),
            ];

            Mat<double> mixing =
            [
                [-2.3, -1.5],
                [ 0.9,  3.1],
                [ 2.0, -3.5],
                [ 0.0,  0.0],
                [-1.5,  1.0],
            ];

            var xs = mixing * ys;

            var ica = xs.Cols.Ica(2, new Random(77));

            var transformed = xs.Cols.Select(x => ica.Transform(x).AsEnumerable()).ColsToMatrix();

            Assert.That(GetError(transformed.Rows[0], ys), Is.LessThan(0.03));
            Assert.That(GetError(transformed.Rows[1], ys), Is.LessThan(0.03));

            var reconstructed = transformed.Cols.Select(x => ica.InverseTransform(x).AsEnumerable()).ColsToMatrix();

            Assert.That(GetError(reconstructed.Rows[0], xs.Rows[0]), Is.LessThan(0.01));
            Assert.That(GetError(reconstructed.Rows[1], xs.Rows[1]), Is.LessThan(0.01));
            Assert.That(GetError(reconstructed.Rows[2], xs.Rows[2]), Is.LessThan(0.01));
            Assert.That(GetError(reconstructed.Rows[3], xs.Rows[3]), Is.LessThan(0.01));
            Assert.That(GetError(reconstructed.Rows[4], xs.Rows[4]), Is.LessThan(0.01));
        }

        private static Vec<double> CreateTestSignal1(int n)
        {
            var vec = new Vec<double>(n);
            for (var i = 0; i < n; i++)
            {
                vec[i] = Math.Sin(2 * Math.PI * i / n * 17) + 1;
            }
            return vec;
        }

        private static Vec<double> CreateTestSignal2(int n)
        {
            var vec = new Vec<double>(n);
            for (var i = 0; i < n; i++)
            {
                vec[i] = Math.Sign(Math.Cos(2 * Math.PI * i / n * 19)) - 2;
            }
            return vec;
        }

        private static Vec<double> CreateTestSignal3(int n)
        {
            var vec = new Vec<double>(n);
            for (var i = 0; i < n; i++)
            {
                vec[i] = (i % 13) / 13.0 * 2 - 1 + 3;
            }
            return vec;
        }

        private static double GetError(Vec<double> x, Mat<double> y)
        {
            return y.Rows.Select(row => GetError(x, row)).Min();
        }

        private static double GetError(Vec<double> x, Vec<double> y)
        {
            var nx = Normalize(x);
            var ny = Normalize(y);

            var e1 = (nx - ny).Select(Math.Abs).Average();
            var e2 = (nx + ny).Select(Math.Abs).Average();

            return Math.Min(e1, e2);
        }

        private static Vec<double> Normalize(Vec<double> x)
        {
            var mean = x.Average();
            var sd = x.StandardDeviation();
            return (x - mean) / sd;
        }
    }
}
