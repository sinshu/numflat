using System;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

using MathNetVectorS = MathNet.Numerics.LinearAlgebra.Vector<float>;
using MathNetMatrixS = MathNet.Numerics.LinearAlgebra.Matrix<float>;
using MathNetVectorD = MathNet.Numerics.LinearAlgebra.Vector<double>;
using MathNetMatrixD = MathNet.Numerics.LinearAlgebra.Matrix<double>;
using MathNetVectorZ = MathNet.Numerics.LinearAlgebra.Vector<System.Numerics.Complex>;
using MathNetMatrixZ = MathNet.Numerics.LinearAlgebra.Matrix<System.Numerics.Complex>;

namespace NumFlatTest
{
    public static class NumAssert
    {
        public static void AreSame(Vec<float> expected, Vec<float> actual, float delta)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count));

            var errors = expected.Zip(actual, (x, y) => x - y);

            if (errors.Select(Math.Abs).Max() > delta)
            {
                Assert.Fail();
            }

        }

        public static void AreSame(MathNetVectorS expected, Vec<float> actual, float delta)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count));

            var errors = expected.Zip(actual, (x, y) => x - y);

            if (errors.Select(Math.Abs).Max() > delta)
            {
                Assert.Fail();
            }
        }

        public static void AreSame(Vec<double> expected, Vec<double> actual, double delta)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count));

            var errors = expected.Zip(actual, (x, y) => x - y);

            if (errors.Select(Math.Abs).Max() > delta)
            {
                Assert.Fail();
            }

        }

        public static void AreSame(MathNetVectorD expected, Vec<double> actual, double delta)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count));

            var errors = expected.Zip(actual, (x, y) => x - y);

            if (errors.Select(Math.Abs).Max() > delta)
            {
                Assert.Fail();
            }
        }

        public static void AreSame(Vec<Complex> expected, Vec<Complex> actual, double delta)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count));

            var errors = expected.Zip(actual, (x, y) => x - y);

            if (errors.Select(c => Math.Max(Math.Abs(c.Real), Math.Abs(c.Imaginary))).Max() > delta)
            {
                Assert.Fail();
            }

        }

        public static void AreSame(MathNetVectorZ expected, Vec<Complex> actual, double delta)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count));

            var errors = expected.Zip(actual, (x, y) => x - y);

            if (errors.Select(c => Math.Max(Math.Abs(c.Real), Math.Abs(c.Imaginary))).Max() > delta)
            {
                Assert.Fail();
            }
        }

        public static void AreSame(Mat<float> expected, Mat<float> actual, float delta)
        {
            Assert.That(actual.RowCount, Is.EqualTo(expected.RowCount));
            Assert.That(actual.ColCount, Is.EqualTo(expected.ColCount));

            var xs = expected.Cols.SelectMany(col => col);
            var ys = actual.Cols.SelectMany(col => col);
            var errors = xs.Zip(ys, (x, y) => x - y);

            if (errors.Select(Math.Abs).Max() > delta)
            {
                Assert.Fail();
            }
        }

        public static void AreSame(MathNetMatrixS expected, Mat<float> actual, float delta)
        {
            Assert.That(actual.RowCount, Is.EqualTo(expected.RowCount));
            Assert.That(actual.ColCount, Is.EqualTo(expected.ColumnCount));

            var xs = expected.EnumerateColumns().SelectMany(col => col);
            var ys = actual.Cols.SelectMany(col => col);
            var errors = xs.Zip(ys, (x, y) => x - y);

            if (errors.Select(Math.Abs).Max() > delta)
            {
                Assert.Fail();
            }
        }

        public static void AreSame(Mat<double> expected, Mat<double> actual, double delta)
        {
            Assert.That(actual.RowCount, Is.EqualTo(expected.RowCount));
            Assert.That(actual.ColCount, Is.EqualTo(expected.ColCount));

            var xs = expected.Cols.SelectMany(col => col);
            var ys = actual.Cols.SelectMany(col => col);
            var errors = xs.Zip(ys, (x, y) => x - y);

            if (errors.Select(Math.Abs).Max() > delta)
            {
                Assert.Fail();
            }
        }

        public static void AreSame(MathNetMatrixD expected, Mat<double> actual, double delta)
        {
            Assert.That(actual.RowCount, Is.EqualTo(expected.RowCount));
            Assert.That(actual.ColCount, Is.EqualTo(expected.ColumnCount));

            var xs = expected.EnumerateColumns().SelectMany(col => col);
            var ys = actual.Cols.SelectMany(col => col);
            var errors = xs.Zip(ys, (x, y) => x - y);

            if (errors.Select(Math.Abs).Max() > delta)
            {
                Assert.Fail();
            }
        }

        public static void AreSame(Mat<Complex> expected, Mat<Complex> actual, double delta)
        {
            Assert.That(actual.RowCount, Is.EqualTo(expected.RowCount));
            Assert.That(actual.ColCount, Is.EqualTo(expected.ColCount));

            var xs = expected.Cols.SelectMany(col => col);
            var ys = actual.Cols.SelectMany(col => col);
            var errors = xs.Zip(ys, (x, y) => x - y);

            if (errors.Select(c => Math.Max(Math.Abs(c.Real), Math.Abs(c.Imaginary))).Max() > delta)
            {
                Assert.Fail();
            }
        }

        public static void AreSame(MathNetMatrixZ expected, Mat<Complex> actual, double delta)
        {
            Assert.That(actual.RowCount, Is.EqualTo(expected.RowCount));
            Assert.That(actual.ColCount, Is.EqualTo(expected.ColumnCount));
            var xs = expected.EnumerateColumns().SelectMany(col => col);
            var ys = actual.Cols.SelectMany(col => col);
            var errors = xs.Zip(ys, (x, y) => x - y);

            if (errors.Select(c => Math.Max(Math.Abs(c.Real), Math.Abs(c.Imaginary))).Max() > delta)
            {
                Assert.Fail();
            }
        }
    }
}
