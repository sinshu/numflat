﻿using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides extension methods for <see cref="Vec{T}"/>.
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        /// Copies the vector.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="source">
        /// The source vector being copied.
        /// </param>
        /// <returns>
        /// The copied vector.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vector.
        /// To avoid the allocation, use <see cref="Vec{T}.CopyTo(in Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> Copy<T>(in this Vec<T> source) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            var destination = new Vec<T>(source.Count);
            source.CopyTo(destination);
            return destination;
        }

        /// <summary>
        /// Computes a pointwise multiplication of vectors, <c>x .* y</c>.
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
        /// The pointwise multiplication.
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
        /// Computes a pointwise division of vectors, <c>x ./ y</c>.
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
        /// The pointwise division.
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
        /// Reverses the order of elements in a vector.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="x">
        /// The vector to be reversed.
        /// </param>
        /// <returns>
        /// The reversed vector.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vectors.
        /// To avoid the allocation, use <see cref="Vec.Reverse{T}(in Vec{T}, in Vec{T})"/> instead.
        /// </remarks>
        public static Vec<T> Reverse<T>(in this Vec<T> x) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var destination = new Vec<T>(x.Count);
            Vec.Reverse(x, destination);
            return destination;
        }

        /// <summary>
        /// Computes an outer product of vectors, <c>x * y^T</c>.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <returns>
        /// The outer product.
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
        /// Computes an outer product of vectors, <c>x * y^T</c>.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <returns>
        /// The outer product.
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
        /// Computes an outer product of complex vectors, <c>x * y^T</c> or <c>x * y^H</c>.
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
        /// The outer product.
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
        /// <returns>
        /// This method allocates a new vector which is independent from the original vector.
        /// To avoid the allocation, use <see cref="Vec.Map{TSource, TResult}(in Vec{TSource}, Func{TSource, TResult}, in Vec{TResult})"/> instead.
        /// </returns>
        public static Vec<TResult> Map<TSource, TResult>(in this Vec<TSource> source, Func<TSource, TResult> func) where TSource : unmanaged, INumberBase<TSource> where TResult : unmanaged, INumberBase<TResult>
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));

            var result = new Vec<TResult>(source.Count);
            Vec.Map(source, func, result);
            return result;
        }

        /// <summary>
        /// Normalize the vector.
        /// </summary>
        /// <param name="x">
        /// The vector to be normalized.
        /// </param>
        /// <returns>
        /// The normalized vector.
        /// </returns>
        /// <remarks>
        /// The L2 norm of the destination vector will be 1.
        /// This method allocates a new vector which is independent from the original vector.
        /// To avoid the allocation, use <see cref="Vec.Noramlize(in Vec{float}, in Vec{float})"/> instead.
        /// </remarks>
        public static Vec<float> Normalize(in this Vec<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var normalized = new Vec<float>(x.Count);
            Vec.Noramlize(x, normalized);
            return normalized;
        }

        /// <summary>
        /// Normalize the vector.
        /// </summary>
        /// <param name="x">
        /// The vector to be normalized.
        /// </param>
        /// <returns>
        /// The normalized vector.
        /// </returns>
        /// <remarks>
        /// The L2 norm of the destination vector will be 1.
        /// This method allocates a new vector which is independent from the original vector.
        /// To avoid the allocation, use <see cref="Vec.Noramlize(in Vec{double}, in Vec{double})"/> instead.
        /// </remarks>
        public static Vec<double> Normalize(in this Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var normalized = new Vec<double>(x.Count);
            Vec.Noramlize(x, normalized);
            return normalized;
        }

        /// <summary>
        /// Normalize the vector.
        /// </summary>
        /// <param name="x">
        /// The vector to be normalized.
        /// </param>
        /// <returns>
        /// The normalized vector.
        /// </returns>
        /// <remarks>
        /// The L2 norm of the destination vector will be 1.
        /// This method allocates a new vector which is independent from the original vector.
        /// To avoid the allocation, use <see cref="Vec.Noramlize(in Vec{Complex}, in Vec{Complex})"/> instead.
        /// </remarks>
        public static Vec<Complex> Normalize(in this Vec<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var normalized = new Vec<Complex>(x.Count);
            Vec.Noramlize(x, normalized);
            return normalized;
        }

        /// <summary>
        /// Normalize the vector.
        /// </summary>
        /// <param name="x">
        /// The vector to be normalized.
        /// </param>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <returns>
        /// The normalized vector.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vector.
        /// To avoid the allocation, use <see cref="Vec.Noramlize(in Vec{float}, in Vec{float}, float)"/> instead.
        /// </remarks>
        public static Vec<float> Normalize(in this Vec<float> x, float p)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            if (p < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(p), "'p' must be greater than or equal to one.");
            }

            var normalized = new Vec<float>(x.Count);
            Vec.Noramlize(x, normalized, p);
            return normalized;
        }

        /// <summary>
        /// Normalize the vector.
        /// </summary>
        /// <param name="x">
        /// The vector to be normalized.
        /// </param>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <returns>
        /// The normalized vector.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vector.
        /// To avoid the allocation, use <see cref="Vec.Noramlize(in Vec{double}, in Vec{double}, double)"/> instead.
        /// </remarks>
        public static Vec<double> Normalize(in this Vec<double> x, double p)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            if (p < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(p), "'p' must be greater than or equal to one.");
            }

            var normalized = new Vec<double>(x.Count);
            Vec.Noramlize(x, normalized, p);
            return normalized;
        }

        /// <summary>
        /// Normalize the vector.
        /// </summary>
        /// <param name="x">
        /// The vector to be normalized.
        /// </param>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <returns>
        /// The normalized vector.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original vector.
        /// To avoid the allocation, use <see cref="Vec.Noramlize(in Vec{Complex}, in Vec{Complex}, double)"/> instead.
        /// </remarks>
        public static Vec<Complex> Normalize(in this Vec<Complex> x, double p)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            if (p < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(p), "'p' must be greater than or equal to one.");
            }

            var normalized = new Vec<Complex>(x.Count);
            Vec.Noramlize(x, normalized, p);
            return normalized;
        }
    }
}
