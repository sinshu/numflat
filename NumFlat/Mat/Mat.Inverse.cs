using System;
using System.Buffers;
using System.Numerics;
using MatFlat;

namespace NumFlat
{
    public static partial class Mat
    {
        /// <summary>
        /// Computes a matrix inversion, X^-1.
        /// </summary>
        /// <param name="x">
        /// The matrix to be inverted.
        /// </param>
        /// <param name="destination">
        /// The destination of the matrix inversion.
        /// </param>
        /// <exception cref="MatrixFactorizationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        public static unsafe void Inverse(in Mat<float> x, in Mat<float> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            x.CopyTo(destination);

            using var upiv = MemoryPool<int>.Shared.Rent(destination.RowCount);
            var piv = upiv.Memory.Span;

            fixed (float* pd = destination.Memory.Span)
            fixed (int* ppiv = piv)
            {
                Factorization.Lu(destination.RowCount, destination.RowCount, pd, destination.Stride, ppiv);
                Factorization.LuInverse(destination.RowCount, pd, destination.Stride, ppiv);
            }
        }

        /// <summary>
        /// Computes a matrix inversion, X^-1.
        /// </summary>
        /// <param name="x">
        /// The matrix to be inverted.
        /// </param>
        /// <param name="destination">
        /// The destination of the matrix inversion.
        /// </param>
        /// <exception cref="MatrixFactorizationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        public static unsafe void Inverse(in Mat<double> x, in Mat<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            x.CopyTo(destination);

            using var upiv = MemoryPool<int>.Shared.Rent(destination.RowCount);
            var piv = upiv.Memory.Span;

            fixed (double* pd = destination.Memory.Span)
            fixed (int* ppiv = piv)
            {
                Factorization.Lu(destination.RowCount, destination.RowCount, pd, destination.Stride, ppiv);
                Factorization.LuInverse(destination.RowCount, pd, destination.Stride, ppiv);
            }
        }

        /// <summary>
        /// Computes a matrix inversion, X^-1.
        /// </summary>
        /// <param name="x">
        /// The matrix to be inverted.
        /// </param>
        /// <param name="destination">
        /// The destination of the matrix inversion.
        /// </param>
        /// <exception cref="MatrixFactorizationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        public static unsafe void Inverse(in Mat<Complex> x, in Mat<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            x.CopyTo(destination);

            using var upiv = MemoryPool<int>.Shared.Rent(destination.RowCount);
            var piv = upiv.Memory.Span;

            fixed (Complex* pd = destination.Memory.Span)
            fixed (int* ppiv = piv)
            {
                Factorization.Lu(destination.RowCount, destination.RowCount, pd, destination.Stride, ppiv);
                Factorization.LuInverse(destination.RowCount, pd, destination.Stride, ppiv);
            }
        }
    }
}
