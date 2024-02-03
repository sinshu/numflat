using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides extension methods for <see cref="Mat{T}"/>.
    /// </summary>
    public static class MatrixExtension
    {
        /// <summary>
        /// Computes a pointwise-multiplication of matrices, X .* Y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use '<see cref="Mat.PointwiseMul{T}(in Mat{T}, in Mat{T}, in Mat{T})"/>' instead.
        /// </remarks>
        public static Mat<T> PointwiseMul<T>(in this Mat<T> x, in Mat<T> y) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            var result = new Mat<T>(x.RowCount, x.ColCount);
            Mat.PointwiseMul(x, y, result);
            return result;
        }

        /// <summary>
        /// Computes a pointwise-division of matrices, X ./ Y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use '<see cref="Mat.PointwiseDiv{T}(in Mat{T}, in Mat{T}, in Mat{T})"/>' instead.
        /// </remarks>
        public static Mat<T> PointwiseDiv<T>(in this Mat<T> x, in Mat<T> y) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            var result = new Mat<T>(x.RowCount, x.ColCount);
            Mat.PointwiseDiv(x, y, result);
            return result;
        }

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

        /// <summary>
        /// Computes a pseudo inverse of the matrix A.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed in computing SVD.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use '<see cref="Mat.PseudoInverse(in Mat{float}, in Mat{float})"/>' instead.
        /// </remarks>
        public static Mat<float> PseudoInverse(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var result = new Mat<float>(a.ColCount, a.RowCount);
            Mat.PseudoInverse(a, result);
            return result;
        }

        /// <summary>
        /// Computes a pseudo inverse of the matrix A.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <param name="tolerance">
        /// Singular values below this threshold will be ignored.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed in computing SVD.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use '<see cref="Mat.PseudoInverse(in Mat{float}, in Mat{float}, float)"/>' instead.
        /// </remarks>
        public static Mat<float> PseudoInverse(in this Mat<float> a, float tolerance)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var result = new Mat<float>(a.ColCount, a.RowCount);
            Mat.PseudoInverse(a, result, tolerance);
            return result;
        }

        /// <summary>
        /// Computes a pseudo inverse of the matrix A.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed in computing SVD.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use '<see cref="Mat.PseudoInverse(in Mat{double}, in Mat{double})"/>' instead.
        /// </remarks>
        public static Mat<double> PseudoInverse(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var result = new Mat<double>(a.ColCount, a.RowCount);
            Mat.PseudoInverse(a, result);
            return result;
        }

        /// <summary>
        /// Computes a pseudo inverse of the matrix A.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <param name="tolerance">
        /// Singular values below this threshold will be ignored.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed in computing SVD.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use '<see cref="Mat.PseudoInverse(in Mat{double}, in Mat{double}, double)"/>' instead.
        /// </remarks>
        public static Mat<double> PseudoInverse(in this Mat<double> a, double tolerance)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var result = new Mat<double>(a.ColCount, a.RowCount);
            Mat.PseudoInverse(a, result, tolerance);
            return result;
        }

        /// <summary>
        /// Computes a pseudo inverse of the matrix A.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed in computing SVD.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use '<see cref="Mat.PseudoInverse(in Mat{Complex}, in Mat{Complex})"/>' instead.
        /// </remarks>
        public static Mat<Complex> PseudoInverse(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var result = new Mat<Complex>(a.ColCount, a.RowCount);
            Mat.PseudoInverse(a, result);
            return result;
        }

        /// <summary>
        /// Computes a pseudo inverse of the matrix A.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <param name="tolerance">
        /// Singular values below this threshold will be ignored.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed in computing SVD.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use '<see cref="Mat.PseudoInverse(in Mat{Complex}, in Mat{Complex}, double)"/>' instead.
        /// </remarks>
        public static Mat<Complex> PseudoInverse(in this Mat<Complex> a, double tolerance)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var result = new Mat<Complex>(a.ColCount, a.RowCount);
            Mat.PseudoInverse(a, result, tolerance);
            return result;
        }

        /// <summary>
        /// Applies a function to each value of the source matrix.
        /// </summary>
        /// <typeparam name="TSource">
        /// The source type.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The destination type.
        /// </typeparam>
        /// <param name="source">
        /// The source matrix.
        /// </param>
        /// <param name="func">
        /// The function to be applied.
        /// </param>
        /// <returns>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use '<see cref="Mat.Map{TSource, TResult}(in Mat{TSource}, Func{TSource, TResult}, in Mat{TResult})"/>' instead.
        /// </returns>
        public static Mat<TResult> Map<TSource, TResult>(in this Mat<TSource> source, Func<TSource, TResult> func) where TSource : unmanaged, INumberBase<TSource> where TResult : unmanaged, INumberBase<TResult>
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));

            var result = new Mat<TResult>(source.RowCount, source.ColCount);
            Mat.Map(source, func, result);
            return result;
        }
    }
}
