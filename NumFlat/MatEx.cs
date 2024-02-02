using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides extension methods for <see cref="Mat{T}"/>.
    /// </summary>
    public static class MatEx
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
        /// To avoid the allocation, use '<see cref="Mat.Transpose{T}(in Mat{T}, in Mat{T})"/>' instead.
        /// To efficiently perform matrix multiplication with matrix transposition,
        /// use '<see cref="Mat.Mul(in Mat{double}, in Mat{double}, in Mat{double}, bool, bool)"/>'.
        /// </remarks>
        public static Mat<T> Transpose<T>(in this Mat<T> x) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Mat<T>(x.ColCount, x.RowCount);
            Mat.Transpose(x, result);
            return result;
        }

        /// <summary>
        /// Conjugates the complex matrix.
        /// </summary>
        /// <param name="x">
        /// The complex matrix to be conjugated.
        /// </param>
        /// <returns>
        /// The conjugated complex matrix.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use '<see cref="Mat.Conjugate(in Mat{Complex}, in Mat{Complex})"/>' instead.
        /// To efficiently perform matrix multiplication with matrix conjugation,
        /// use '<see cref="Mat.Mul(in Mat{Complex}, in Mat{Complex}, in Mat{Complex}, bool, bool, bool, bool)"/>'.
        /// </remarks>
        public static Mat<Complex> Conjugate(in this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Mat<Complex>(x.RowCount, x.ColCount);
            Mat.Conjugate(x, result);
            return result;
        }

        /// <summary>
        /// Computes the Hermitian transpose of a complex matrix, X^H.
        /// </summary>
        /// <param name="x">
        /// The complex matrix to be transposed.
        /// </param>
        /// <returns>
        /// The transposed matrix.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use '<see cref="Mat.ConjugateTranspose(in Mat{Complex}, in Mat{Complex})"/>' instead.
        /// To efficiently perform matrix multiplication with matrix transposition,
        /// use '<see cref="Mat.Mul(in Mat{Complex}, in Mat{Complex}, in Mat{Complex}, bool, bool, bool, bool)"/>'.
        /// </remarks>
        public static Mat<Complex> ConjugateTranspose(in this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Mat<Complex>(x.ColCount, x.RowCount);
            Mat.ConjugateTranspose(x, result);
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
        /// To avoid the allocation, use '<see cref="Mat.Inverse(in Mat{float}, in Mat{float})"/>' instead.
        /// </remarks>
        public static Mat<float> Inverse(in this Mat<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

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
        /// To avoid the allocation, use '<see cref="Mat.Inverse(in Mat{double}, in Mat{double})"/>' instead.
        /// </remarks>
        public static Mat<double> Inverse(in this Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

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
        /// To avoid the allocation, use '<see cref="Mat.Inverse(in Mat{Complex}, in Mat{Complex})"/>' instead.
        /// </remarks>
        public static Mat<Complex> Inverse(in this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

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
