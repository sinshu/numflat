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
        public static float Dot(this Vec<float> x, Vec<float> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            return Vec.Dot(x, y);
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
        public static double Dot(this Vec<double> x, Vec<double> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            return Vec.Dot(x, y);
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
        public static Complex Dot(this Vec<Complex> x, Vec<Complex> y, bool conjugateX)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            return Vec.Dot(x, y, conjugateX);
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
        /// <returns>
        /// The result of the outer product.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Outer(Vec{float}, Vec{float}, Mat{float})"/> instead.
        /// </remarks>
        public static Mat<float> Outer(this Vec<float> x, Vec<float> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));

            var result = new Mat<float>(x.Count, y.Count);
            Vec.Outer(x, y, result);
            return result;
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
        /// <returns>
        /// The result of the outer product.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Outer(Vec{double}, Vec{double}, Mat{double})"/> instead.
        /// </remarks>
        public static Mat<double> Outer(this Vec<double> x, Vec<double> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));

            var result = new Mat<double>(x.Count, y.Count);
            Vec.Outer(x, y, result);
            return result;
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
        /// <param name="conjugateY">
        /// If true, the vector y is treated as conjugated.
        /// </param>
        /// <returns>
        /// The result of the outer product.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Outer(Vec{Complex}, Vec{Complex}, Mat{Complex}, bool)"/> instead.
        /// </remarks>
        public static Mat<Complex> Outer(this Vec<Complex> x, Vec<Complex> y, bool conjugateY)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));

            var result = new Mat<Complex>(x.Count, y.Count);
            Vec.Outer(x, y, result, conjugateY);
            return result;
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
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vector.
        /// To avoid the allocation, use <see cref="Vec.Conjugate(Vec{Complex}, Vec{Complex})"/> instead.
        /// </remarks>
        public static Vec<Complex> Conjugate(this Vec<Complex> x)
        {
            var result = new Vec<Complex>(x.Count);
            Vec.Conjugate(x, result);
            return result;
        }
    }
}
