using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NumFlat
{
    public partial struct Vec<T>
    {
        /// <summary>
        /// Computes a vector addition, <c>x + y</c>.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <returns>
        /// The vector addition.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Add{T}(in Vec{T}, in Vec{T}, in Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> operator +(in Vec<T> x, in Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            var result = new Vec<T>(x.count);
            Vec.Add(x, y, result);
            return result;
        }

        /// <summary>
        /// Computes a pointwise vector-and-scalar addition.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The scalar y.
        /// </param>
        /// <returns>
        /// The pointwise vector-and-scalar addition.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Add{T}(in Vec{T}, T, in Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> operator +(in Vec<T> x, T y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Vec<T>(x.count);
            Vec.Add(x, y, result);
            return result;
        }

        /// <summary>
        /// Computes a pointwise scalar-and-vector addition.
        /// </summary>
        /// <param name="x">
        /// The scalar x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <returns>
        /// The pointwise scalar-and-vector addition.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Add{T}(in Vec{T}, T, in Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> operator +(T x, in Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(y, nameof(y));

            var result = new Vec<T>(y.count);
            Vec.Add(y, x, result);
            return result;
        }

        /// <summary>
        /// Computes a vector subtraction, <c>x - y</c>.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <returns>
        /// The vector subtraction.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Sub{T}(in Vec{T}, in Vec{T}, in Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> operator -(in Vec<T> x, in Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            var result = new Vec<T>(x.count);
            Vec.Sub(x, y, result);
            return result;
        }

        /// <summary>
        /// Computes a pointwise vector-and-scalar subtraction.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The scalar y.
        /// </param>
        /// <returns>
        /// The pointwise vector-and-scalar subtraction.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Sub{T}(in Vec{T}, T, in Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> operator -(in Vec<T> x, T y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Vec<T>(x.count);
            Vec.Sub(x, y, result);
            return result;
        }

        /// <summary>
        /// Computes a pointwise scalar-and-vector subtraction.
        /// </summary>
        /// <param name="x">
        /// The scalar x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <returns>
        /// The pointwise scalar-and-vector subtraction.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Sub{T}(in Vec{T}, T, in Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> operator -(T x, in Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(y, nameof(y));

            var result = new Vec<T>(y.count);
            Vec.Sub(y, x, result);
            return result;
        }

        /// <summary>
        /// Computes a vector-and-scalar multiplication, <c>x * y</c>.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The scalar y.
        /// </param>
        /// <returns>
        /// The vector-and-scalar multiplication.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Mul{T}(in Vec{T}, T, in Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> operator *(in Vec<T> x, T y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Vec<T>(x.count);
            Vec.Mul(x, y, result);
            return result;
        }

        /// <summary>
        /// Computes a scalar-and-vector multiplication, <c>x * y</c>.
        /// </summary>
        /// <param name="x">
        /// The scalar x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <returns>
        /// The scalar-and-vector multiplication.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Mul{T}(in Vec{T}, T, in Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> operator *(T x, in Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(y, nameof(y));

            var result = new Vec<T>(y.count);
            Vec.Mul(y, x, result);
            return result;
        }

        /// <summary>
        /// Computes a dot product of vectors, <c>x^T * y</c>.
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
        public static T operator *(Vec<T> x, Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            if (typeof(T) == typeof(float))
            {
                var ux = Unsafe.As<Vec<T>, Vec<float>>(ref x);
                var uy = Unsafe.As<Vec<T>, Vec<float>>(ref y);
                var result = Vec.Dot(ux, uy);
                return Unsafe.As<float, T>(ref result);
            }
            else if (typeof(T) == typeof(double))
            {
                var ux = Unsafe.As<Vec<T>, Vec<double>>(ref x);
                var uy = Unsafe.As<Vec<T>, Vec<double>>(ref y);
                var result = Vec.Dot(ux, uy);
                return Unsafe.As<double, T>(ref result);
            }
            else if (typeof(T) == typeof(Complex))
            {
                var ux = Unsafe.As<Vec<T>, Vec<Complex>>(ref x);
                var uy = Unsafe.As<Vec<T>, Vec<Complex>>(ref y);
                var result = Vec.Dot(ux, uy, false);
                return Unsafe.As<Complex, T>(ref result);
            }
            else
            {
                throw new NotSupportedException($"Dot product for the type '{typeof(T).Name}' is not supported.");
            }
        }

        /// <summary>
        /// Computes a vector-and-scalar division, <c>x / y</c>.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The scalar y.
        /// </param>
        /// <returns>
        /// The vector-and-scalar division.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Div{T}(in Vec{T}, T, in Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> operator /(in Vec<T> x, T y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Vec<T>(x.count);
            Vec.Div(x, y, result);
            return result;
        }
    }
}
