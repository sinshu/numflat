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
        /// The destination of the vector addition.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Add<T>(in Vec<T> x, in Vec<T> y, in Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, y, destination);

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
        /// The destination of the vector subtraction.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Sub<T>(in Vec<T> x, in Vec<T> y, in Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, y, destination);

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
        /// The destination the vector-and-scalar multiplication.
        /// </param>
        /// /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Mul<T>(in Vec<T> x, T y, in Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

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
        /// Computes a vector-and-scalar division, x / y.
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
        /// The destination of the vector-and-scalar division.
        /// </param>
        /// /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Div<T>(in Vec<T> x, T y, in Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px] / y;
                px += x.Stride;
                pd += destination.Stride;
            }
        }

        /// <summary>
        /// Computes a pointwise multiplication of vectors, x .* y.
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
        /// The destination of the the pointwise multiplication.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void PointwiseMul<T>(in Vec<T> x, in Vec<T> y, in Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, y, destination);

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
        /// Computes a pointwise division of vectors, x ./ y.
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
        /// The destination of the pointwise division.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void PointwiseDiv<T>(in Vec<T> x, in Vec<T> y, in Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, y, destination);

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

            destination.Clear();

            fixed (float* px = x.Memory.Span)
            fixed (float* py = y.Memory.Span)
            fixed (float* pd = destination.Memory.Span)
            {
                Blas.Sger(
                    Order.ColMajor,
                    destination.RowCount, destination.ColCount,
                    1.0F,
                    px, x.Stride,
                    py, y.Stride,
                    pd, destination.Stride);
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

            destination.Clear();

            fixed (double* px = x.Memory.Span)
            fixed (double* py = y.Memory.Span)
            fixed (double* pd = destination.Memory.Span)
            {
                Blas.Dger(
                    Order.ColMajor,
                    destination.RowCount, destination.ColCount,
                    1.0,
                    px, x.Stride,
                    py, y.Stride,
                    pd, destination.Stride);
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
        /// The destination of the conjugation.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Conjugate(in Vec<Complex> x, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

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

        /// <summary>
        /// Applies a function to each value of the source vector.
        /// </summary>
        /// <typeparam name="TSource">
        /// The source type.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The destination type.
        /// </typeparam>
        /// <param name="source">
        /// The source vector.
        /// </param>
        /// <param name="func">
        /// The function to be applied.
        /// </param>
        /// <param name="destination">
        /// The destination where the results of the function application are stored.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Map<TSource, TResult>(in Vec<TSource> source, Func<TSource, TResult> func, in Vec<TResult> destination) where TSource : unmanaged, INumberBase<TSource> where TResult : unmanaged, INumberBase<TResult>
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (source.Count != destination.Count)
            {
                throw new ArgumentException("The vectors must have the same length.");
            }

            var ss = source.Memory.Span;
            var sd = destination.Memory.Span;
            var ps = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = func(ss[ps]);
                ps += source.Stride;
                pd += destination.Stride;
            }
        }
    }
}
