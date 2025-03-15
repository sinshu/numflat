using System;
using System.Buffers;
using System.Numerics;
using MatFlat;

namespace NumFlat
{
    public static partial class Mat
    {
        /// <summary>
        /// Computes a matrix inversion, <c>X^-1</c>.
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

            using var upiv = new TemporalArray<int>(destination.RowCount);
            var piv = upiv.Item;

            fixed (float* pd = destination.Memory.Span)
            fixed (int* ppiv = piv)
            {
                Factorization.Lu(destination.RowCount, destination.RowCount, pd, destination.Stride, ppiv);

                try
                {
                    Factorization.LuInverse(destination.RowCount, pd, destination.Stride, ppiv);
                }
                catch (MatFlat.MatrixFactorizationException)
                {
                    throw new MatrixFactorizationException("The matrix is not invertible.");
                }
            }
        }

        /// <summary>
        /// Computes a matrix inversion, <c>X^-1</c>.
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

            using var upiv = new TemporalArray<int>(destination.RowCount);
            var piv = upiv.Item;

            fixed (double* pd = destination.Memory.Span)
            fixed (int* ppiv = piv)
            {
                Factorization.Lu(destination.RowCount, destination.RowCount, pd, destination.Stride, ppiv);

                try
                {
                    Factorization.LuInverse(destination.RowCount, pd, destination.Stride, ppiv);
                }
                catch (MatFlat.MatrixFactorizationException)
                {
                    throw new MatrixFactorizationException("The matrix is not invertible.");
                }
            }
        }

        /// <summary>
        /// Computes a matrix inversion, <c>X^-1</c>.
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

            using var upiv = new TemporalArray<int>(destination.RowCount);
            var piv = upiv.Item;

            fixed (Complex* pd = destination.Memory.Span)
            fixed (int* ppiv = piv)
            {
                Factorization.Lu(destination.RowCount, destination.RowCount, pd, destination.Stride, ppiv);

                try
                {
                    Factorization.LuInverse(destination.RowCount, pd, destination.Stride, ppiv);
                }
                catch (MatFlat.MatrixFactorizationException)
                {
                    throw new MatrixFactorizationException("The matrix is not invertible.");
                }
            }
        }
    }
}
