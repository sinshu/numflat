using System;
using System.Buffers;
using System.Numerics;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides low-level matrix operations.
    /// </summary>
    public static class Mat
    {
        /// <summary>
        /// Computes a matrix addition, X + Y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix addition.
        /// </param>
        /// <remarks>
        /// The dimensions of the matrices must match.
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Add<T>(Mat<T> x, Mat<T> y, Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var oy = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var py = oy;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] + sy[py];
                    px++;
                    py++;
                    pd++;
                }
                ox += x.Stride;
                oy += y.Stride;
                od += destination.Stride;
            }
        }

        /// <summary>
        /// Computes a matrix subtraction, X - Y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix subtraction.
        /// </param>
        /// <remarks>
        /// The dimensions of the matrices must match.
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Sub<T>(Mat<T> x, Mat<T> y, Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var oy = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var py = oy;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] - sy[py];
                    px++;
                    py++;
                    pd++;
                }
                ox += x.Stride;
                oy += y.Stride;
                od += destination.Stride;
            }
        }

        /// <summary>
        /// Computes a matrix-and-scalar multiplication, X * y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The scalar y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix-and-scalar multiplication.
        /// The dimensions of <paramref name="destination"/> must match <paramref name="x"/>.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Mul<T>(Mat<T> x, T y, Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref destination);

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] * y;
                    px++;
                    pd++;
                }
                ox += x.Stride;
                od += destination.Stride;
            }
        }

        /// <summary>
        /// Computes a pointwise-multiplication of matrices, X .* Y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the pointwise-multiplication.
        /// </param>
        /// <remarks>
        /// The dimensions of the matrices must match.
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void PointwiseMul<T>(Mat<T> x, Mat<T> y, Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var oy = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var py = oy;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] * sy[py];
                    px++;
                    py++;
                    pd++;
                }
                ox += x.Stride;
                oy += y.Stride;
                od += destination.Stride;
            }
        }

        /// <summary>
        /// Computes a pointwise-division of matrices, X ./ Y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the pointwise-division.
        /// </param>
        /// <remarks>
        /// The dimensions of the matrices must match.
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void PointwiseDiv<T>(Mat<T> x, Mat<T> y, Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var oy = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var py = oy;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] / sy[py];
                    px++;
                    py++;
                    pd++;
                }
                ox += x.Stride;
                oy += y.Stride;
                od += destination.Stride;
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
        /// 'y.Count' must match 'x.ColCount'.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix-and-vector multiplication.
        /// 'destination.Count' must match 'x.RowCount'.
        /// </param>
        /// <param name="transposeX">
        /// If true, the matrix X is treated as transposed.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Mul(Mat<float> x, Vec<float> y, Vec<float> destination, bool transposeX)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

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

            var transX = transposeX ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;

            fixed (float* px = x.Memory.Span)
            fixed (float* py = y.Memory.Span)
            fixed (float* pd = destination.Memory.Span)
            {
                Blas.Sgemv(
                    Order.ColMajor,
                    transX,
                    x.RowCount, x.ColCount,
                    1.0F,
                    px, x.Stride,
                    py, y.Stride,
                    0.0F,
                    pd, destination.Stride);
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
        /// 'y.Count' must match 'x.ColCount'.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix-and-vector multiplication.
        /// 'destination.Count' must match 'x.RowCount'.
        /// </param>
        /// <param name="transposeX">
        /// If true, the matrix X is treated as transposed.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Mul(Mat<double> x, Vec<double> y, Vec<double> destination, bool transposeX)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

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

            var transX = transposeX ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;

            fixed (double* px = x.Memory.Span)
            fixed (double* py = y.Memory.Span)
            fixed (double* pd = destination.Memory.Span)
            {
                Blas.Dgemv(
                    Order.ColMajor,
                    transX,
                    x.RowCount, x.ColCount,
                    1.0,
                    px, x.Stride,
                    py, y.Stride,
                    0.0,
                    pd, destination.Stride);
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
        /// 'y.Count' must match 'x.ColCount'.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix-and-vector multiplication.
        /// 'destination.Count' must match 'x.RowCount'.
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
        public static unsafe void Mul(Mat<Complex> x, Vec<Complex> y, Vec<Complex> destination, bool transposeX, bool conjugateX)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

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

            var transX = transposeX ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            if (conjugateX)
            {
                transX = transX == OpenBlasSharp.Transpose.Trans ? OpenBlasSharp.Transpose.ConjTrans : OpenBlasSharp.Transpose.ConjNoTrans;
            }

            var one = Complex.One;
            var zero = Complex.Zero;

            fixed (Complex* px = x.Memory.Span)
            fixed (Complex* py = y.Memory.Span)
            fixed (Complex* pd = destination.Memory.Span)
            {
                Blas.Zgemv(
                    Order.ColMajor,
                    transX,
                    x.RowCount, x.ColCount,
                    &one,
                    px, x.Stride,
                    py, y.Stride,
                    &zero,
                    pd, destination.Stride);
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
        /// The destination of the result of the matrix multiplication.
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
        public static unsafe void Mul(Mat<float> x, Mat<float> y, Mat<float> destination, bool transposeX, bool transposeY)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            var transX = transposeX ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var transY = transposeY ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var (m, n, k) = GetMulArgs(ref x, transposeX, ref y, transposeY, ref destination);

            fixed (float* px = x.Memory.Span)
            fixed (float* py = y.Memory.Span)
            fixed (float* pd = destination.Memory.Span)
            {
                Blas.Sgemm(
                    Order.ColMajor,
                    transX, transY,
                    m, n, k,
                    1.0F,
                    px, x.Stride,
                    py, y.Stride,
                    0.0F,
                    pd, destination.Stride);
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
        /// The destination of the result of the matrix multiplication.
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
        public static unsafe void Mul(Mat<double> x, Mat<double> y, Mat<double> destination, bool transposeX, bool transposeY)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            var transX = transposeX ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var transY = transposeY ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var (m, n, k) = GetMulArgs(ref x, transposeX, ref y, transposeY, ref destination);

            fixed (double* px = x.Memory.Span)
            fixed (double* py = y.Memory.Span)
            fixed (double* pd = destination.Memory.Span)
            {
                Blas.Dgemm(
                    Order.ColMajor,
                    transX, transY,
                    m, n, k,
                    1.0,
                    px, x.Stride,
                    py, y.Stride,
                    0.0,
                    pd, destination.Stride);
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
        /// The destination of the result of the matrix multiplication.
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
        public static unsafe void Mul(Mat<Complex> x, Mat<Complex> y, Mat<Complex> destination, bool transposeX, bool conjugateX, bool transposeY, bool conjugateY)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            var transX = transposeX ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var transY = transposeY ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var (m, n, k) = GetMulArgs(ref x, transposeX, ref y, transposeY, ref destination);
            if (conjugateX)
            {
                transX = transX == OpenBlasSharp.Transpose.Trans ? OpenBlasSharp.Transpose.ConjTrans : OpenBlasSharp.Transpose.ConjNoTrans;
            }
            if (conjugateY)
            {
                transY = transY == OpenBlasSharp.Transpose.Trans ? OpenBlasSharp.Transpose.ConjTrans : OpenBlasSharp.Transpose.ConjNoTrans;
            }

            var one = Complex.One;
            var zero = Complex.Zero;

            fixed (Complex* px = x.Memory.Span)
            fixed (Complex* py = y.Memory.Span)
            fixed (Complex* pd = destination.Memory.Span)
            {
                Blas.Zgemm(
                    Order.ColMajor,
                    transX, transY,
                    m, n, k,
                    &one,
                    px, x.Stride,
                    py, y.Stride,
                    &zero,
                    pd, destination.Stride);
            }
        }

        /// <summary>
        /// Computes a matrix transposition, X^T.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix transposition.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// Since in-place transposition is not supported,
        /// <paramref name="x"/> and <paramref name="destination"/> must be different.
        /// To efficiently perform matrix multiplication with matrix transposition,
        /// use <see cref="Mat.Mul(Mat{double}, Mat{double}, Mat{double}, bool, bool)"/>.
        /// </remarks>
        public static void Transpose<T>(Mat<T> x, Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            if (destination.RowCount != x.ColCount)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.ColCount'.");
            }

            if (destination.ColCount != x.RowCount)
            {
                throw new ArgumentException("'destination.ColCount' must match 'x.RowCount'.");
            }

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            for (var col = 0; col < destination.ColCount; col++)
            {
                var px = col;
                var pd = destination.Stride * col;
                var end = pd + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px];
                    px += x.Stride;
                    pd++;
                }
            }
        }

        /// <summary>
        /// Conjugates the complex matrix.
        /// </summary>
        /// <param name="x">
        /// The complex matrix to be conjugated.
        /// </param>
        /// <param name="destination">
        /// The conjugated complex matrix.
        /// </param>
        /// <remarks>
        /// The dimensions of the matrices must match.
        /// This method does not allocate managed heap memory.
        /// To efficiently perform matrix multiplication with matrix conjugation,
        /// use <see cref="Mat.Mul(Mat{Complex}, Mat{Complex}, Mat{Complex}, bool, bool, bool, bool)"/>.
        /// </remarks>
        public static void Conjugate(Mat<Complex> x, Mat<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref destination);

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px].Conjugate();
                    px++;
                    pd++;
                }
                ox += x.Stride;
                od += destination.Stride;
            }
        }

        /// <summary>
        /// Computes the Hermitian transpose of a complex matrix, X^H.
        /// </summary>
        /// <param name="x">
        /// The complex matrix to be transposed.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the Hermitian transposition.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// Since in-place transposition is not supported,
        /// <paramref name="x"/> and <paramref name="destination"/> must be different.
        /// To efficiently perform matrix multiplication with matrix transposition,
        /// use <see cref="Mat.Mul(Mat{Complex}, Mat{Complex}, Mat{Complex}, bool, bool, bool, bool)"/>.
        /// </remarks>
        public static void ConjugateTranspose(Mat<Complex> x, Mat<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            if (destination.RowCount != x.ColCount)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.ColCount'.");
            }

            if (destination.ColCount != x.RowCount)
            {
                throw new ArgumentException("'destination.ColCount' must match 'x.RowCount'.");
            }

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            for (var col = 0; col < destination.ColCount; col++)
            {
                var px = col;
                var pd = destination.Stride * col;
                var end = pd + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px].Conjugate();
                    px += x.Stride;
                    pd++;
                }
            }
        }

        /// <summary>
        /// Computes a matrix inversion, X^-1.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix inversion.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// This method internally uses <see cref="ArrayPool{T}.Shared"/> to allocate buffer.
        /// </remarks>
        public static unsafe void Inverse(Mat<float> x, Mat<float> destination)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            if (x.RowCount != x.ColCount)
            {
                throw new ArgumentException("'x' must be a square matrix.");
            }

            if (destination.RowCount != x.RowCount)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.RowCount'.");
            }

            if (destination.ColCount != x.ColCount)
            {
                throw new ArgumentException("'destination.ColCount' must match 'x.ColCount'.");
            }

            x.CopyTo(destination);

            var piv = ArrayPool<int>.Shared.Rent(x.RowCount);
            try
            {
                fixed (float* px = x.Memory.Span)
                fixed (int* ppiv = piv)
                {
                    var info = Lapack.Sgetrf(
                        MatrixLayout.ColMajor,
                        x.RowCount, x.ColCount,
                        px, x.Stride,
                        ppiv);
                    if (info != LapackInfo.None)
                    {
                        throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Sgetrf), (int)info);
                    }

                    info = Lapack.Sgetri(
                        MatrixLayout.ColMajor,
                        x.RowCount,
                        px, x.Stride,
                        ppiv);
                    if (info != LapackInfo.None)
                    {
                        throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Sgetri), (int)info);
                    }
                }
            }
            finally
            {
                ArrayPool<int>.Shared.Return(piv);
            }
        }

        /// <summary>
        /// Computes a matrix inversion, X^-1.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix inversion.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// This method internally uses <see cref="ArrayPool{T}.Shared"/> to allocate buffer.
        /// </remarks>
        public static unsafe void Inverse(Mat<double> x, Mat<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            if (x.RowCount != x.ColCount)
            {
                throw new ArgumentException("'x' must be a square matrix.");
            }

            if (destination.RowCount != x.RowCount)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.RowCount'.");
            }

            if (destination.ColCount != x.ColCount)
            {
                throw new ArgumentException("'destination.ColCount' must match 'x.ColCount'.");
            }

            x.CopyTo(destination);

            var piv = ArrayPool<int>.Shared.Rent(x.RowCount);
            try
            {
                fixed (double* px = x.Memory.Span)
                fixed (int* ppiv = piv)
                {
                    var info = Lapack.Dgetrf(
                        MatrixLayout.ColMajor,
                        x.RowCount, x.ColCount,
                        px, x.Stride,
                        ppiv);
                    if (info != LapackInfo.None)
                    {
                        throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Dgetrf), (int)info);
                    }

                    info = Lapack.Dgetri(
                        MatrixLayout.ColMajor,
                        x.RowCount,
                        px, x.Stride,
                        ppiv);
                    if (info != LapackInfo.None)
                    {
                        throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Dgetri), (int)info);
                    }
                }
            }
            finally
            {
                ArrayPool<int>.Shared.Return(piv);
            }
        }

        /// <summary>
        /// Computes a matrix inversion, X^-1.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix inversion.
        /// </param>
        /// <exception cref="LapackException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// This method internally uses <see cref="ArrayPool{T}.Shared"/> to allocate buffer.
        /// </remarks>
        public static unsafe void Inverse(Mat<Complex> x, Mat<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            if (x.RowCount != x.ColCount)
            {
                throw new ArgumentException("'x' must be a square matrix.");
            }

            if (destination.RowCount != x.RowCount)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.RowCount'.");
            }

            if (destination.ColCount != x.ColCount)
            {
                throw new ArgumentException("'destination.ColCount' must match 'x.ColCount'.");
            }

            x.CopyTo(destination);

            var piv = ArrayPool<int>.Shared.Rent(x.RowCount);
            try
            {
                fixed (Complex* px = x.Memory.Span)
                fixed (int* ppiv = piv)
                {
                    var info = Lapack.Zgetrf(
                        MatrixLayout.ColMajor,
                        x.RowCount, x.ColCount,
                        px, x.Stride,
                        ppiv);
                    if (info != LapackInfo.None)
                    {
                        throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Zgetrf), (int)info);
                    }

                    info = Lapack.Zgetri(
                        MatrixLayout.ColMajor,
                        x.RowCount,
                        px, x.Stride,
                        ppiv);
                    if (info != LapackInfo.None)
                    {
                        throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Zgetri), (int)info);
                    }
                }
            }
            finally
            {
                ArrayPool<int>.Shared.Return(piv);
            }
        }

        private static (int m, int n, int k) GetMulArgs<T>(ref Mat<T> x, bool transposeX, ref Mat<T> y, bool transposeY, ref Mat<T> destination) where T : unmanaged, INumberBase<T>
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
                throw new ArgumentException($"The dimensions of 'destination' must match '{m.name}'.");
            }

            if (destination.ColCount != n.count)
            {
                throw new ArgumentException($"The dimensions of 'destination' must match '{n.name}'.");
            }

            return (m.count, n.count, k.count);
        }
    }
}
