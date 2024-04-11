using System;
using System.Numerics;
using MatFlat;

namespace NumFlat
{
    public static partial class Mat
    {
        /// <summary>
        /// Computes a matrix-and-vector multiplication, X * y.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// This vector is interpreted as a column vector.
        /// </param>
        /// <param name="destination">
        /// The destination of the matrix-and-vector multiplication.
        /// </param>
        /// <param name="transposeX">
        /// If true, the matrix X is treated as transposed.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Mul(in Mat<float> x, in Vec<float> y, in Vec<float> destination, bool transposeX)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (transposeX)
            {
                if (y.Count != x.RowCount)
                {
                    throw new ArgumentException("'y.Count' must match 'x.RowCount'.");
                }

                if (destination.Count != x.ColCount)
                {
                    throw new ArgumentException("'destination.Count' must match 'x.ColCount'.");
                }
            }
            else
            {
                if (y.Count != x.ColCount)
                {
                    throw new ArgumentException("'y.Count' must match 'x.ColCount'.");
                }

                if (destination.Count != x.RowCount)
                {
                    throw new ArgumentException("'destination.Count' must match 'x.RowCount'.");
                }
            }

            var transX = transposeX ? MatFlat.Transpose.Trans : MatFlat.Transpose.NoTrans;

            fixed (float* px = x.Memory.Span)
            fixed (float* py = y.Memory.Span)
            fixed (float* pd = destination.Memory.Span)
            {
                Blas.MulMatVec(transX, x.RowCount, x.ColCount, px, x.Stride, py, y.Stride, pd, destination.Stride);
            }
        }

        /// <summary>
        /// Computes a matrix multiplication, X * Y.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <param name="destination">
        /// The destination the matrix multiplication.
        /// </param>
        /// <param name="transposeX">
        /// If true, the matrix X is treated as transposed.
        /// </param>
        /// <param name="transposeY">
        /// If true, the matrix Y is treated as transposed.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Mul(in Mat<float> x, in Mat<float> y, in Mat<float> destination, bool transposeX, bool transposeY)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            var transX = transposeX ? MatFlat.Transpose.Trans : MatFlat.Transpose.NoTrans;
            var transY = transposeY ? MatFlat.Transpose.Trans : MatFlat.Transpose.NoTrans;
            var (m, n, k) = GetMulArgs(x, transposeX, y, transposeY, destination);

            fixed (float* px = x.Memory.Span)
            fixed (float* py = y.Memory.Span)
            fixed (float* pd = destination.Memory.Span)
            {
                Blas.MulMatMat(transX, transY, m, n, k, px, x.Stride, py, y.Stride, pd, destination.Stride);
            }
        }

        /// <summary>
        /// Computes a matrix-and-vector multiplication, X * y.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// This vector is interpreted as a column vector.
        /// </param>
        /// <param name="destination">
        /// The destination of the matrix-and-vector multiplication.
        /// </param>
        /// <param name="transposeX">
        /// If true, the matrix X is treated as transposed.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Mul(in Mat<double> x, in Vec<double> y, in Vec<double> destination, bool transposeX)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (transposeX)
            {
                if (y.Count != x.RowCount)
                {
                    throw new ArgumentException("'y.Count' must match 'x.RowCount'.");
                }

                if (destination.Count != x.ColCount)
                {
                    throw new ArgumentException("'destination.Count' must match 'x.ColCount'.");
                }
            }
            else
            {
                if (y.Count != x.ColCount)
                {
                    throw new ArgumentException("'y.Count' must match 'x.ColCount'.");
                }

                if (destination.Count != x.RowCount)
                {
                    throw new ArgumentException("'destination.Count' must match 'x.RowCount'.");
                }
            }

            var transX = transposeX ? MatFlat.Transpose.Trans : MatFlat.Transpose.NoTrans;

            fixed (double* px = x.Memory.Span)
            fixed (double* py = y.Memory.Span)
            fixed (double* pd = destination.Memory.Span)
            {
                Blas.MulMatVec(transX, x.RowCount, x.ColCount, px, x.Stride, py, y.Stride, pd, destination.Stride);
            }
        }

        /// <summary>
        /// Computes a matrix multiplication, X * Y.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <param name="destination">
        /// The destination the matrix multiplication.
        /// </param>
        /// <param name="transposeX">
        /// If true, the matrix X is treated as transposed.
        /// </param>
        /// <param name="transposeY">
        /// If true, the matrix Y is treated as transposed.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Mul(in Mat<double> x, in Mat<double> y, in Mat<double> destination, bool transposeX, bool transposeY)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            var transX = transposeX ? MatFlat.Transpose.Trans : MatFlat.Transpose.NoTrans;
            var transY = transposeY ? MatFlat.Transpose.Trans : MatFlat.Transpose.NoTrans;
            var (m, n, k) = GetMulArgs(x, transposeX, y, transposeY, destination);

            fixed (double* px = x.Memory.Span)
            fixed (double* py = y.Memory.Span)
            fixed (double* pd = destination.Memory.Span)
            {
                Blas.MulMatMat(transX, transY, m, n, k, px, x.Stride, py, y.Stride, pd, destination.Stride);
            }
        }

        /// <summary>
        /// Computes a matrix-and-vector multiplication, X * y.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// This vector is interpreted as a column vector.
        /// </param>
        /// <param name="destination">
        /// The destination of the matrix-and-vector multiplication.
        /// </param>
        /// <param name="transposeX">
        /// If true, the matrix X is treated as transposed.
        /// </param>
        /// <param name="conjugateX">
        /// If true, the matrix X is treated as conjugated.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Mul(in Mat<Complex> x, in Vec<Complex> y, in Vec<Complex> destination, bool transposeX, bool conjugateX)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (transposeX)
            {
                if (y.Count != x.RowCount)
                {
                    throw new ArgumentException("'y.Count' must match 'x.RowCount'.");
                }

                if (destination.Count != x.ColCount)
                {
                    throw new ArgumentException("'destination.Count' must match 'x.ColCount'.");
                }
            }
            else
            {
                if (y.Count != x.ColCount)
                {
                    throw new ArgumentException("'y.Count' must match 'x.ColCount'.");
                }

                if (destination.Count != x.RowCount)
                {
                    throw new ArgumentException("'destination.Count' must match 'x.RowCount'.");
                }
            }

            var transX = transposeX ? MatFlat.Transpose.Trans : MatFlat.Transpose.NoTrans;
            if (conjugateX)
            {
                transX = transX == MatFlat.Transpose.Trans ? MatFlat.Transpose.ConjTrans : MatFlat.Transpose.ConjNoTrans;
            }

            var one = Complex.One;
            var zero = Complex.Zero;

            fixed (Complex* px = x.Memory.Span)
            fixed (Complex* py = y.Memory.Span)
            fixed (Complex* pd = destination.Memory.Span)
            {
                Blas.MulMatVec(transX, x.RowCount, x.ColCount, px, x.Stride, py, y.Stride, pd, destination.Stride);
            }
        }

        /// <summary>
        /// Computes a matrix multiplication, X * Y.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <param name="destination">
        /// The destination of the matrix multiplication.
        /// </param>
        /// <param name="transposeX">
        /// If true, the matrix X is treated as transposed.
        /// </param>
        /// <param name="conjugateX">
        /// If true, the matrix X is treated as conjugated.
        /// </param>
        /// <param name="transposeY">
        /// If true, the matrix Y is treated as transposed.
        /// </param>
        /// <param name="conjugateY">
        /// If true, the matrix Y is treated as conjugated.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Mul(in Mat<Complex> x, in Mat<Complex> y, in Mat<Complex> destination, bool transposeX, bool conjugateX, bool transposeY, bool conjugateY)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            var transX = transposeX ? MatFlat.Transpose.Trans : MatFlat.Transpose.NoTrans;
            var transY = transposeY ? MatFlat.Transpose.Trans : MatFlat.Transpose.NoTrans;
            var (m, n, k) = GetMulArgs(x, transposeX, y, transposeY, destination);
            if (conjugateX)
            {
                transX = transX == MatFlat.Transpose.Trans ? MatFlat.Transpose.ConjTrans : MatFlat.Transpose.ConjNoTrans;
            }
            if (conjugateY)
            {
                transY = transY == MatFlat.Transpose.Trans ? MatFlat.Transpose.ConjTrans : MatFlat.Transpose.ConjNoTrans;
            }

            var one = Complex.One;
            var zero = Complex.Zero;

            fixed (Complex* px = x.Memory.Span)
            fixed (Complex* py = y.Memory.Span)
            fixed (Complex* pd = destination.Memory.Span)
            {
                Blas.MulMatMat(transX, transY, m, n, k, px, x.Stride, py, y.Stride, pd, destination.Stride);
            }
        }

        private static (int m, int n, int k) GetMulArgs<T>(in Mat<T> x, bool transposeX, in Mat<T> y, bool transposeY, in Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            var xRowCount = (count: x.RowCount, name: "x.RowCount");
            var xColCount = (count: x.ColCount, name: "x.ColCount");
            var yRowCount = (count: y.RowCount, name: "y.RowCount");
            var yColCount = (count: y.ColCount, name: "y.ColCount");

            if (transposeX)
            {
                (xRowCount, xColCount) = (xColCount, xRowCount);
            }

            if (transposeY)
            {
                (yRowCount, yColCount) = (yColCount, yRowCount);
            }

            if (yRowCount.count != xColCount.count)
            {
                throw new ArgumentException($"'{yRowCount.name}' must match '{xColCount.name}'.");
            }

            var m = xRowCount;
            var n = yColCount;
            var k = xColCount;

            if (destination.RowCount != m.count)
            {
                throw new ArgumentException($"'destination.RowCount' must match '{m.name}'.");
            }

            if (destination.ColCount != n.count)
            {
                throw new ArgumentException($"'destination.ColCount' must match '{n.name}'.");
            }

            return (m.count, n.count, k.count);
        }
    }
}
