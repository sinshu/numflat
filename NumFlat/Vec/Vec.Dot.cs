using System;
using System.Numerics;
using MatFlat;

namespace NumFlat
{
    public static partial class Vec
    {
        /// <summary>
        /// Computes a dot product of vectors, x^T * y.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <returns>
        /// The dot product.
        /// </returns>
        public static unsafe float Dot(in this Vec<float> x, in Vec<float> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            fixed (float* px = x.Memory.Span)
            fixed (float* py = y.Memory.Span)
            {
                return Blas.Dot(x.Count, px, x.Stride, py, y.Stride);
            }
        }

        /// <summary>
        /// Computes a dot product of vectors, x^T * y.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <returns>
        /// The dot product.
        /// </returns>
        public static unsafe double Dot(in this Vec<double> x, in Vec<double> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            fixed (double* px = x.Memory.Span)
            fixed (double* py = y.Memory.Span)
            {
                return Blas.Dot(x.Count, px, x.Stride, py, y.Stride);
            }
        }

        /// <summary>
        /// Computes a dot product of vectors, x^T * y.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <param name="conjugateX">
        /// If true, the vector x is treated as conjugated.
        /// </param>
        /// <returns>
        /// The dot product.
        /// </returns>
        public static unsafe Complex Dot(in this Vec<Complex> x, in Vec<Complex> y, bool conjugateX)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            fixed (Complex* px = x.Memory.Span)
            fixed (Complex* py = y.Memory.Span)
            {
                if (conjugateX)
                {
                    return Blas.DotConj(x.Count, px, x.Stride, py, y.Stride);
                }
                else
                {
                    return Blas.Dot(x.Count, px, x.Stride, py, y.Stride);
                }
            }
        }
    }
}
