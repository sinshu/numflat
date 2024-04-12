using System;
using System.Numerics;
using MatFlat;

namespace NumFlat
{
    public static partial class Vec
    {
        /// <summary>
        /// Computes an outer product of vectors, x * y^T.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <param name="destination">
        /// The destination of the outer product.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Outer(in Vec<float> x, in Vec<float> y, in Mat<float> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.RowCount != x.Count)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.Count'.");
            }

            if (destination.ColCount != y.Count)
            {
                throw new ArgumentException("'destination.ColCount' must match 'y.Count'.");
            }

            fixed (float* px = x.Memory.Span)
            fixed (float* py = y.Memory.Span)
            fixed (float* pd = destination.Memory.Span)
            {
                Blas.Outer(destination.RowCount, destination.ColCount, px, x.Stride, py, y.Stride, pd, destination.Stride);
            }
        }

        /// <summary>
        /// Computes an outer product of vectors, x * y^T.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <param name="destination">
        /// The destination of the outer product.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Outer(in Vec<double> x, in Vec<double> y, in Mat<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.RowCount != x.Count)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.Count'.");
            }

            if (destination.ColCount != y.Count)
            {
                throw new ArgumentException("'destination.ColCount' must match 'y.Count'.");
            }

            fixed (double* px = x.Memory.Span)
            fixed (double* py = y.Memory.Span)
            fixed (double* pd = destination.Memory.Span)
            {
                Blas.Outer(destination.RowCount, destination.ColCount, px, x.Stride, py, y.Stride, pd, destination.Stride);
            }
        }

        /// <summary>
        /// Computes an outer product of vectors, x * y^T.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <param name="destination">
        /// The destination of the outer product.
        /// </param>
        /// <param name="conjugateY">
        /// If true, the vector y is treated as conjugated.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Outer(in Vec<Complex> x, in Vec<Complex> y, in Mat<Complex> destination, bool conjugateY)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.RowCount != x.Count)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.Count'.");
            }

            if (destination.ColCount != y.Count)
            {
                throw new ArgumentException("'destination.ColCount' must match 'y.Count'.");
            }

            fixed (Complex* px = x.Memory.Span)
            fixed (Complex* py = y.Memory.Span)
            fixed (Complex* pd = destination.Memory.Span)
            {
                if (conjugateY)
                {
                    Blas.OuterConj(destination.RowCount, destination.ColCount, px, x.Stride, py, y.Stride, pd, destination.Stride);
                }
                else
                {
                    Blas.Outer(destination.RowCount, destination.ColCount, px, x.Stride, py, y.Stride, pd, destination.Stride);
                }
            }
        }
    }
}
