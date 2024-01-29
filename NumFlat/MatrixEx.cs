using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides extension methods for <see cref="Mat{T}"/>.
    /// </summary>
    public static class MatrixEx
    {
        /// <summary>
        /// Computes a matrix transposition, X^T.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <returns>
        /// The transposed matrix.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.Transpose{T}(Mat{T}, Mat{T})"/> instead.
        /// </remarks>
        public static Mat<T> Transpose<T>(this Mat<T> x) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));

            var result = new Mat<T>(x.ColCount, x.RowCount);
            Mat.Transpose(x, result);
            return result;
        }

        /// <summary>
        /// Computes a matrix inversion, X^-1.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <returns>
        /// The inverted matrix.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.Inverse(Mat{float}, Mat{float})"/> instead.
        /// </remarks>
        public static Mat<float> Inverse(this Mat<float> x)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));

            if (x.RowCount != x.ColCount)
            {
                throw new ArgumentException("The matrix must be square.");
            }

            var result = new Mat<float>(x.RowCount, x.ColCount);
            Mat.Inverse(x, result);
            return result;
        }

        /// <summary>
        /// Computes a matrix inversion, X^-1.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <returns>
        /// The inverted matrix.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.Inverse(Mat{double}, Mat{double})"/> instead.
        /// </remarks>
        public static Mat<double> Inverse(this Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));

            if (x.RowCount != x.ColCount)
            {
                throw new ArgumentException("The matrix must be square.");
            }

            var result = new Mat<double>(x.RowCount, x.ColCount);
            Mat.Inverse(x, result);
            return result;
        }

        /// <summary>
        /// Computes a matrix inversion, X^-1.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <returns>
        /// The inverted matrix.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.Inverse(Mat{Complex}, Mat{Complex})"/> instead.
        /// </remarks>
        public static Mat<Complex> Inverse(this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));

            if (x.RowCount != x.ColCount)
            {
                throw new ArgumentException("The matrix must be square.");
            }

            var result = new Mat<Complex>(x.RowCount, x.ColCount);
            Mat.Inverse(x, result);
            return result;
        }
    }
}
