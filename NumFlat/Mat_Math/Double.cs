using System;
using System.Buffers;
using OpenBlasSharp;

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
        public static unsafe void Mul(in Mat<double> x, in Mat<double> y, in Mat<double> destination, bool transposeX, bool transposeY)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            var transX = transposeX ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var transY = transposeY ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var (m, n, k) = GetMulArgs(x, transposeX, y, transposeY, destination);

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
        /// Computes the determinant of a matrix, det(X).
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <returns>
        /// The determinant of the matrix.
        /// </returns>
        public static unsafe double Determinant(in Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            if (x.RowCount != x.ColCount)
            {
                throw new ArgumentException("'x' must be a square matrix.");
            }

            var xLength = x.RowCount * x.ColCount;
            using var xBuffer = MemoryPool<double>.Shared.Rent(xLength);
            var xCopy = new Mat<double>(x.RowCount, x.ColCount, x.RowCount, xBuffer.Memory.Slice(0, xLength));
            x.CopyTo(xCopy);

            using var pivBuffer = MemoryPool<int>.Shared.Rent(xCopy.RowCount);
            var piv = pivBuffer.Memory.Span;

            fixed (double* px = xCopy.Memory.Span)
            fixed (int* ppiv = piv)
            {
                var info = Lapack.Dgetrf(
                    MatrixLayout.ColMajor,
                    xCopy.RowCount, xCopy.ColCount,
                    px, xCopy.Stride,
                    ppiv);
            }

            var determinant = 1.0;
            for (var i = 0; i < xCopy.RowCount; i++)
            {
                if (piv[i] - 1 == i)
                {
                    determinant *= xCopy[i, i];
                }
                else
                {
                    determinant *= -xCopy[i, i];
                }
            }

            return determinant;
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
        public static unsafe void Inverse(in Mat<double> x, in Mat<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            if (x.RowCount != x.ColCount)
            {
                throw new ArgumentException("'x' must be a square matrix.");
            }

            x.CopyTo(destination);

            using var pivBuffer = MemoryPool<int>.Shared.Rent(destination.RowCount);
            var piv = pivBuffer.Memory.Span;

            fixed (double* pd = destination.Memory.Span)
            fixed (int* ppiv = piv)
            {
                var info = Lapack.Dgetrf(
                    MatrixLayout.ColMajor,
                    destination.RowCount, destination.ColCount,
                    pd, destination.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Dgetrf), (int)info);
                }

                info = Lapack.Dgetri(
                    MatrixLayout.ColMajor,
                    destination.RowCount,
                    pd, destination.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Dgetri), (int)info);
                }
            }
        }

        /// <summary>
        /// Gets the rank of the matrix.
        /// </summary>
        /// <param name="x">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The rank of the matrix.
        /// </returns>
        public static int Rank(this in Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var sLength = Math.Min(x.RowCount, x.ColCount);
            using var sBuffer = MemoryPool<double>.Shared.Rent(sLength);
            var s = new Vec<double>(sBuffer.Memory.Slice(0, sLength));
            SvdDouble.GetSingularValues(x, s);

            var tolerance = Special.Eps(s[0]) * Math.Max(x.RowCount, x.RowCount);

            var rank = 0;
            foreach (var value in s)
            {
                if (value > tolerance)
                {
                    rank++;
                }
            }

            return rank;
        }

        /// <summary>
        /// Gets the rank of the matrix.
        /// </summary>
        /// <param name="x">
        /// The matrix.
        /// </param>
        /// <param name="tolerance">
        /// Singular values below this threshold will be ignored.
        /// </param>
        /// <returns>
        /// The rank of the matrix.
        /// </returns>
        public static int Rank(this in Mat<double> x, double tolerance)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var sLength = Math.Min(x.RowCount, x.ColCount);
            using var sBuffer = MemoryPool<double>.Shared.Rent(sLength);
            var s = new Vec<double>(sBuffer.Memory.Slice(0, sLength));
            SvdDouble.GetSingularValues(x, s);

            var rank = 0;
            foreach (var value in s)
            {
                if (value > tolerance)
                {
                    rank++;
                }
            }

            return rank;
        }
    }
}
