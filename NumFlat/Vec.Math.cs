using System;
using System.Numerics;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides low-level vector operations.
    /// </summary>
    public static class Vec
    {
        /// <summary>
        /// Computes a vector addition, x + y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the vector addition.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Add<T>(Vec<T> x, Vec<T> y, Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var py = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px] + sy[py];
                px += x.Stride;
                py += y.Stride;
                pd += destination.Stride;
            }
        }

        /// <summary>
        /// Computes a vector subtraction, x - y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the vector subtraction.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Sub<T>(Vec<T> x, Vec<T> y, Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var py = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px] - sy[py];
                px += x.Stride;
                py += y.Stride;
                pd += destination.Stride;
            }
        }

        /// <summary>
        /// Computes a vector-and-scalar multiplication, x * y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The scalar y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the vector-and-scalar multiplication.
        /// </param>
        /// /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Mul<T>(Vec<T> x, T y, Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref destination);

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px] * y;
                px += x.Stride;
                pd += destination.Stride;
            }
        }

        /// <summary>
        /// Computes a pointwise-multiplication of vectors, x .* y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the pointwise-multiplication.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void PointwiseMul<T>(Vec<T> x, Vec<T> y, Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var py = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px] * sy[py];
                px += x.Stride;
                py += y.Stride;
                pd += destination.Stride;
            }
        }

        /// <summary>
        /// Computes a pointwise-division of vectors, x ./ y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the pointwise-division.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void PointwiseDiv<T>(Vec<T> x, Vec<T> y, Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var py = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px] / sy[py];
                px += x.Stride;
                py += y.Stride;
                pd += destination.Stride;
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
        /// The result of the dot product.
        /// </returns>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe float Dot(Vec<float> x, Vec<float> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            fixed (float* px = x.Memory.Span)
            fixed (float* py = y.Memory.Span)
            {
                return Blas.Sdot(x.Count, px, x.Stride, py, y.Stride);
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
        /// The result of the dot product.
        /// </returns>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe double Dot(Vec<double> x, Vec<double> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            fixed (double* px = x.Memory.Span)
            fixed (double* py = y.Memory.Span)
            {
                return Blas.Ddot(x.Count, px, x.Stride, py, y.Stride);
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
        /// The result of the dot product.
        /// </returns>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe Complex Dot(Vec<Complex> x, Vec<Complex> y, bool conjugateX)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            fixed (Complex* px = x.Memory.Span)
            fixed (Complex* py = y.Memory.Span)
            {
                if (conjugateX)
                {
                    return Blas.Zdotc(x.Count, px, x.Stride, py, y.Stride);
                }
                else
                {
                    return Blas.Zdotu(x.Count, px, x.Stride, py, y.Stride);
                }
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
        /// The result of outer dot product.
        /// </param>
        /// <param name="conjugateY">
        /// If true, the vector y is treated as conjugated.
        /// </param>
        /// <returns>
        /// The result of the outer product.
        /// </returns>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Outer(Vec<Complex> x, Vec<Complex> y, Mat<Complex> destination, bool conjugateY)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            if (destination.RowCount != x.Count)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.Count'.");
            }

            if (destination.ColCount != y.Count)
            {
                throw new ArgumentException("'destination.ColCount' must match 'y.Count'.");
            }

            var one = Complex.One;

            destination.Clear();

            fixed (Complex* px = x.Memory.Span)
            fixed (Complex* py = y.Memory.Span)
            fixed (Complex* pd = destination.Memory.Span)
            {
                if (conjugateY)
                {
                    Blas.Zgerc(
                        Order.ColMajor,
                        destination.RowCount, destination.ColCount,
                        &one,
                        px, x.Stride,
                        py, y.Stride,
                        pd, destination.Stride);
                }
                else
                {
                    Blas.Zgeru(
                        Order.ColMajor,
                        destination.RowCount, destination.ColCount,
                        &one,
                        px, x.Stride,
                        py, y.Stride,
                        pd, destination.Stride);
                }
            }
        }

        /// <summary>
        /// Conjugates the complex vector.
        /// </summary>
        /// <param name="x">
        /// The complex vector to be conjugated.
        /// </param>
        /// <param name="destination">
        /// The conjugated complex vector.
        /// </param>
        public static void Conjugate(Vec<Complex> x, Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref destination);

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px].Conjugate();
                px += x.Stride;
                pd += destination.Stride;
            }
        }
    }
}
