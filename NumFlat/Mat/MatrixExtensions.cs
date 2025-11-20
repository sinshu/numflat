using System;
using System.Numerics;
using MatFlat;

namespace NumFlat
{
    /// <summary>
    /// Provides extension methods for <see cref="Mat{T}"/>.
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Copies the matrix.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="source">
        /// The source matrix being copied.
        /// </param>
        /// <returns>
        /// The copied matrix.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat{T}.CopyTo(in Mat{T})"/> instead.
        /// </remarks>
        public static Mat<T> Copy<T>(in this Mat<T> source) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            var destination = new Mat<T>(source.RowCount, source.ColCount);
            source.CopyTo(destination);
            return destination;
        }

        /// <summary>
        /// Computes a pointwise multiplication of matrices, <c>X .* Y</c>.
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
        /// <returns>
        /// The pointwise multiplication.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.PointwiseMul{T}(in Mat{T}, in Mat{T}, in Mat{T})"/> instead.
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
        /// Computes a pointwise division of matrices, <c>X ./ Y</c>.
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
        /// <returns>
        /// The pointwise division.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.PointwiseDiv{T}(in Mat{T}, in Mat{T}, in Mat{T})"/> instead.
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
        /// Computes a matrix transposition, <c>X^T</c>.
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
        /// To avoid the allocation, use <see cref="Mat.Transpose{T}(in Mat{T}, in Mat{T})"/> instead.
        /// To efficiently perform matrix multiplication with matrix transposition,
        /// use <see cref="Mat.Mul(in Mat{double}, in Mat{double}, in Mat{double}, bool, bool)"/>.
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
        /// To avoid the allocation, use <see cref="Mat.Conjugate(in Mat{Complex}, in Mat{Complex})"/> instead.
        /// To efficiently perform matrix multiplication with matrix conjugation,
        /// use <see cref="Mat.Mul(in Mat{Complex}, in Mat{Complex}, in Mat{Complex}, bool, bool, bool, bool)"/>.
        /// </remarks>
        public static Mat<Complex> Conjugate(in this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Mat<Complex>(x.RowCount, x.ColCount);
            Mat.Conjugate(x, result);
            return result;
        }

        /// <summary>
        /// Computes the Hermitian transpose of a complex matrix, <c>X^H</c>.
        /// </summary>
        /// <param name="x">
        /// The complex matrix to be transposed.
        /// </param>
        /// <returns>
        /// The transposed matrix.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.ConjugateTranspose(in Mat{Complex}, in Mat{Complex})"/> instead.
        /// To efficiently perform matrix multiplication with matrix transposition,
        /// use <see cref="Mat.Mul(in Mat{Complex}, in Mat{Complex}, in Mat{Complex}, bool, bool, bool, bool)"/>.
        /// </remarks>
        public static Mat<Complex> ConjugateTranspose(in this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Mat<Complex>(x.ColCount, x.RowCount);
            Mat.ConjugateTranspose(x, result);
            return result;
        }

        /// <summary>
        /// Extracts the real part of each element in the complex matrix.
        /// </summary>
        /// <param name="x">
        /// The complex matrix.
        /// </param>
        /// <returns>
        /// The real parts of the complex matrix.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.Real(in Mat{Complex}, in Mat{double})"/> instead.
        /// </remarks>
        public static Mat<double> Real(in this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Mat<double>(x.RowCount, x.ColCount);
            Mat.Real(x, result);
            return result;
        }

        /// <summary>
        /// Extracts the imaginary part of each element in the complex matrix.
        /// </summary>
        /// <param name="x">
        /// The complex matrix.
        /// </param>
        /// <returns>
        /// The imaginary parts of the complex matrix.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.Imaginary(in Mat{Complex}, in Mat{double})"/> instead.
        /// </remarks>
        public static Mat<double> Imaginary(in this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Mat<double>(x.RowCount, x.ColCount);
            Mat.Imaginary(x, result);
            return result;
        }

        /// <summary>
        /// Computes a matrix inversion, <c>X^-1</c>.
        /// </summary>
        /// <param name="x">
        /// The matrix to be inverted.
        /// </param>
        /// <returns>
        /// The inverted matrix.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.Inverse(in Mat{float}, in Mat{float})"/> instead.
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
        /// Computes a matrix inversion, <c>X^-1</c>.
        /// </summary>
        /// <param name="x">
        /// The matrix to be inverted.
        /// </param>
        /// <returns>
        /// The inverted matrix.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.Inverse(in Mat{double}, in Mat{double})"/> instead.
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
        /// Computes a matrix inversion, <c>X^-1</c>.
        /// </summary>
        /// <param name="x">
        /// The matrix to be inverted.
        /// </param>
        /// <returns>
        /// The inverted matrix.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.Inverse(in Mat{Complex}, in Mat{Complex})"/> instead.
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
        /// Computes a pseudo inversion of a matrix, <c>pinv(A)</c>.
        /// </summary>
        /// <param name="a">
        /// The matrix to be inverted.
        /// </param>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.PseudoInverse(in Mat{float}, in Mat{float})"/> instead.
        /// </remarks>
        public static Mat<float> PseudoInverse(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var result = new Mat<float>(a.ColCount, a.RowCount);
            Mat.PseudoInverse(a, result);
            return result;
        }

        /// <summary>
        /// Computes a pseudo inversion of a matrix, <c>pinv(A)</c>.
        /// </summary>
        /// <param name="a">
        /// The matrix to be inverted.
        /// </param>
        /// <param name="tolerance">
        /// Singular values below this threshold will be replaced with zero.
        /// </param>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.PseudoInverse(in Mat{float}, in Mat{float}, float)"/> instead.
        /// </remarks>
        public static Mat<float> PseudoInverse(in this Mat<float> a, float tolerance)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var result = new Mat<float>(a.ColCount, a.RowCount);
            Mat.PseudoInverse(a, result, tolerance);
            return result;
        }

        /// <summary>
        /// Computes a pseudo inversion of a matrix, <c>pinv(A)</c>.
        /// </summary>
        /// <param name="a">
        /// The matrix to be inverted.
        /// </param>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.PseudoInverse(in Mat{double}, in Mat{double})"/> instead.
        /// </remarks>
        public static Mat<double> PseudoInverse(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var result = new Mat<double>(a.ColCount, a.RowCount);
            Mat.PseudoInverse(a, result);
            return result;
        }

        /// <summary>
        /// Computes a pseudo inversion of a matrix, <c>pinv(A)</c>.
        /// </summary>
        /// <param name="a">
        /// The matrix to be inverted.
        /// </param>
        /// <param name="tolerance">
        /// Singular values below this threshold will be replaced with zero.
        /// </param>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.PseudoInverse(in Mat{double}, in Mat{double}, double)"/> instead.
        /// </remarks>
        public static Mat<double> PseudoInverse(in this Mat<double> a, double tolerance)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var result = new Mat<double>(a.ColCount, a.RowCount);
            Mat.PseudoInverse(a, result, tolerance);
            return result;
        }

        /// <summary>
        /// Computes a pseudo inversion of a matrix, <c>pinv(A)</c>.
        /// </summary>
        /// <param name="a">
        /// The matrix to be inverted.
        /// </param>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.PseudoInverse(in Mat{Complex}, in Mat{Complex})"/> instead.
        /// </remarks>
        public static Mat<Complex> PseudoInverse(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var result = new Mat<Complex>(a.ColCount, a.RowCount);
            Mat.PseudoInverse(a, result);
            return result;
        }

        /// <summary>
        /// Computes a pseudo inversion of a matrix, <c>pinv(A)</c>.
        /// </summary>
        /// <param name="a">
        /// The matrix to be inverted.
        /// </param>
        /// <param name="tolerance">
        /// Singular values below this threshold will be replaced with zero.
        /// </param>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrix.
        /// To avoid the allocation, use <see cref="Mat.PseudoInverse(in Mat{Complex}, in Mat{Complex}, double)"/> instead.
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
        /// To avoid the allocation, use <see cref="Mat.Map{TSource, TResult}(in Mat{TSource}, Func{TSource, TResult}, in Mat{TResult})"/> instead.
        /// </returns>
        public static Mat<TResult> Map<TSource, TResult>(in this Mat<TSource> source, Func<TSource, TResult> func) where TSource : unmanaged, INumberBase<TSource> where TResult : unmanaged, INumberBase<TResult>
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));

            var result = new Mat<TResult>(source.RowCount, source.ColCount);
            Mat.Map(source, func, result);
            return result;
        }


        /// <summary>
        /// Enumerates the diagonal elements of the matrix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static Mat<T>.DiagonalElements EnumerateDiagonalElements<T>(in this Mat<T> mat) where T : unmanaged, INumberBase<T>
        {
            return new Mat<T>.DiagonalElements(in mat);
        }
    }
}
