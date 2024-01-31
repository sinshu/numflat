using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NumFlat
{
    public partial struct Vec<T>
    {
        /// <summary>
        /// Computes a vector addition, x + y.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <returns>
        /// The result of the vector addition.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Add{T}(Vec{T}, Vec{T}, Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> operator +(Vec<T> x, Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            var result = new Vec<T>(x.count);
            Vec.Add(x, y, result);
            return result;
        }

        /// <summary>
        /// Computes a vector subtraction, x + y.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <returns>
        /// The result of the vector subtraction.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Sub{T}(Vec{T}, Vec{T}, Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> operator -(Vec<T> x, Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            var result = new Vec<T>(x.count);
            Vec.Sub(x, y, result);
            return result;
        }

        /// <summary>
        /// Computes a vector-and-scalar multiplication, x * y.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The scalar y.
        /// </param>
        /// <returns>
        /// The result of the vector-and-scalar multiplication.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Mul{T}(Vec{T}, T, Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> operator *(Vec<T> x, T y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));

            var result = new Vec<T>(x.count);
            Vec.Mul(x, y, result);
            return result;
        }

        /// <summary>
        /// Computes a vector-and-scalar division, x * y.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The scalar y.
        /// </param>
        /// <returns>
        /// The result of the vector-and-scalar division.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Div{T}(Vec{T}, T, Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> operator /(Vec<T> x, T y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));

            var result = new Vec<T>(x.count);
            Vec.Div(x, y, result);
            return result;
        }

        /// <summary>
        /// Computes a scalar-and-vector multiplication, x * y.
        /// </summary>
        /// <param name="x">
        /// The scalar x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <returns>
        /// The result of the scalar-and-vector multiplication.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Mul{T}(Vec{T}, T, Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> operator *(T x, Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));

            var result = new Vec<T>(y.count);
            Vec.Mul(y, x, result);
            return result;
        }

        /// <summary>
        /// Computes an dot product of vectors, x^T * y.
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
        public static T operator *(Vec<T> x, Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

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
                throw new NotSupportedException($"dot product for the type '{typeof(T).Name}' is not supported.");
            }
        }
    }
}
