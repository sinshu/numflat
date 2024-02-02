using System;
using System.Buffers;
using System.Numerics;
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

            var transX = transposeX ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var transY = transposeY ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var (m, n, k) = GetMulArgs(x, transposeX, y, transposeY, destination);
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
        /// Computes the determinant of a matrix, det(X).
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <returns>
        /// The determinant of the matrix.
        /// </returns>
        /// <remarks>
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
        public static unsafe Complex Determinant(in Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            if (x.RowCount != x.ColCount)
            {
                throw new ArgumentException("'x' must be a square matrix.");
            }

            var xLength = x.RowCount * x.ColCount;
            using var xBuffer = MemoryPool<Complex>.Shared.Rent(xLength);
            var xCopy = new Mat<Complex>(x.RowCount, x.ColCount, x.RowCount, xBuffer.Memory.Slice(0, xLength));
            x.CopyTo(xCopy);

            using var pivBuffer = MemoryPool<int>.Shared.Rent(xCopy.RowCount);
            var piv = pivBuffer.Memory.Span;

            fixed (Complex* px = xCopy.Memory.Span)
            fixed (int* ppiv = piv)
            {
                var info = Lapack.Zgetrf(
                    MatrixLayout.ColMajor,
                    xCopy.RowCount, xCopy.ColCount,
                    px, xCopy.Stride,
                    ppiv);
            }

            var determinant = Complex.One;
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
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
        public static unsafe void Inverse(in Mat<Complex> x, in Mat<Complex> destination)
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

            fixed (Complex* pd = destination.Memory.Span)
            fixed (int* ppiv = piv)
            {
                var info = Lapack.Zgetrf(
                    MatrixLayout.ColMajor,
                    destination.RowCount, destination.ColCount,
                    pd, destination.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Zgetrf), (int)info);
                }

                info = Lapack.Zgetri(
                    MatrixLayout.ColMajor,
                    destination.RowCount,
                    pd, destination.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Zgetri), (int)info);
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
        /// <remarks>
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
        public static int Rank(this in Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var sLength = Math.Min(x.RowCount, x.ColCount);
            using var sBuffer = MemoryPool<double>.Shared.Rent(sLength);
            var s = new Vec<double>(sBuffer.Memory.Slice(0, sLength));
            SvdComplex.GetSingularValues(x, s);

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
        /// <remarks>
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
        public static int Rank(this in Mat<Complex> x, double tolerance)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var sLength = Math.Min(x.RowCount, x.ColCount);
            using var sBuffer = MemoryPool<double>.Shared.Rent(sLength);
            var s = new Vec<double>(sBuffer.Memory.Slice(0, sLength));
            SvdComplex.GetSingularValues(x, s);

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
        /// use '<see cref="Mat.Mul(in Mat{Complex}, in Mat{Complex}, in Mat{Complex}, bool, bool, bool, bool)"/>'.
        /// </remarks>
        public static void Conjugate(in Mat<Complex> x, in Mat<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

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
        /// use '<see cref="Mat.Mul(in Mat{Complex}, in Mat{Complex}, in Mat{Complex}, bool, bool, bool, bool)"/>'.
        /// </remarks>
        public static void ConjugateTranspose(in Mat<Complex> x, in Mat<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

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
    }
}
