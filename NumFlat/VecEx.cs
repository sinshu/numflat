﻿using System;
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
        /// To avoid the allocation, use <see cref="Vec.PointwiseMul{T}(in Vec{T}, in Vec{T}, in Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> PointwiseMul<T>(in this Vec<T> x, in Vec<T> y) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

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
        /// To avoid the allocation, use <see cref="Vec.PointwiseDiv{T}(in Vec{T}, in Vec{T}, in Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> PointwiseDiv<T>(in this Vec<T> x, in Vec<T> y) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

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
        public static float Dot(in this Vec<float> x, in Vec<float> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

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
        public static double Dot(in this Vec<double> x, in Vec<double> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

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
        public static Complex Dot(in this Vec<Complex> x, in Vec<Complex> y, bool conjugateX)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

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
        /// To avoid the allocation, use <see cref="Vec.Outer(in Vec{float}, in Vec{float}, in Mat{float})"/> instead.
        /// </remarks>
        public static Mat<float> Outer(in this Vec<float> x, in Vec<float> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));

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
        /// To avoid the allocation, use <see cref="Vec.Outer(in Vec{double}, in Vec{double}, in Mat{double})"/> instead.
        /// </remarks>
        public static Mat<double> Outer(in this Vec<double> x, in Vec<double> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));

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
        /// To avoid the allocation, use <see cref="Vec.Outer(in Vec{Complex}, in Vec{Complex}, in Mat{Complex}, bool)"/> instead.
        /// </remarks>
        public static Mat<Complex> Outer(in this Vec<Complex> x, in Vec<Complex> y, bool conjugateY)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));

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
        /// To avoid the allocation, use <see cref="Vec.Conjugate(in Vec{Complex}, in Vec{Complex})"/> instead.
        /// </remarks>
        public static Vec<Complex> Conjugate(in this Vec<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Vec<Complex>(x.Count);
            Vec.Conjugate(x, result);
            return result;
        }

        /// <summary>
        /// Converts the vector to a single-row matrix.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="x">
        /// The vector to be converted.
        /// </param>
        /// <returns>
        /// This method allocates a new matrix which is independent from the original vector.
        /// </returns>
        public static Mat<T> ToRowMatrix<T>(in this Vec<T> x) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Mat<T>(1, x.Count);
            x.CopyTo(result.Rows[0]);
            return result;
        }

        /// <summary>
        /// Converts the vector to a single-column matrix.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="x">
        /// The vector to be converted.
        /// </param>
        /// <returns>
        /// This method allocates a new matrix which is independent from the original vector.
        /// </returns>
        public static Mat<T> ToColMatrix<T>(in this Vec<T> x) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Mat<T>(x.Count, 1);
            x.CopyTo(result.Cols[0]);
            return result;
        }
    }
}
