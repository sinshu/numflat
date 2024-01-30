using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides extension methods for <see cref="Vec{T}"/>.
    /// </summary>
    public static class VecEx
    {
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
        /// <returns>
        /// The result of the pointwise-multiplication.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.PointwiseMul{T}(Vec{T}, Vec{T}, Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> PointwiseMul<T>(this Vec<T> x, Vec<T> y) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            var result = new Vec<T>(x.Count);
            Vec.PointwiseMul(x, y, result);
            return result;
        }

        /// <summary>
        /// Computes a pointwise-division of vectors, x .* y.
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
        /// <returns>
        /// The result of the pointwise-division.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.PointwiseDiv{T}(Vec{T}, Vec{T}, Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> PointwiseDiv<T>(this Vec<T> x, Vec<T> y) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            var result = new Vec<T>(x.Count);
            Vec.PointwiseDiv(x, y, result);
            return result;
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
        public static float DotProduct(this Vec<float> x, Vec<float> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            return Vec.DotProduct(x, y);
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
        public static double DotProduct(this Vec<double> x, Vec<double> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            return Vec.DotProduct(x, y);
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
        public static Complex DotProduct(this Vec<Complex> x, Vec<Complex> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            return Vec.DotProduct(x, y);
        }

        /// <summary>
        /// Conjugates the complex vector.
        /// </summary>
        /// <param name="x">
        /// The complex vector to be conjugated.
        /// </param>
        /// <returns>
        /// The conjugated complex vector.
        /// </returns>
        public static Vec<Complex> Conjugate(this Vec<Complex> x)
        {
            var result = new Vec<Complex>(x.Count);
            Vec.Conjugate(x, result);
            return result;
        }

        /// <summary>
        /// Computes a dot product of complex vectors, x^H * y.
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
        public static Complex ConjugateDotProduct(this Vec<Complex> x, Vec<Complex> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            return Vec.ConjugateDotProduct(x, y);
        }
    }
}
