using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides methods for matrix decomposition.
    /// </summary>
    public static class MatrixDecomposition
    {
        /// <summary>
        /// Computes the LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using LU decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="LuDecompositionSingle"/>'.
        /// </returns>
        public static LuDecompositionSingle Lu(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new LuDecompositionSingle(a);
        }

        /// <summary>
        /// Computes the LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using LU decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="LuDecompositionDouble"/>'.
        /// </returns>
        public static LuDecompositionDouble Lu(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new LuDecompositionDouble(a);
        }

        /// <summary>
        /// Computes the LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using LU decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="LuDecompositionComplex"/>'.
        /// </returns>
        public static LuDecompositionComplex Lu(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new LuDecompositionComplex(a);
        }

        /// <summary>
        /// Computes the QR decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using QR decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="QrDecompositionSingle"/>'.
        /// </returns>
        public static QrDecompositionSingle Qr(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new QrDecompositionSingle(a);
        }

        /// <summary>
        /// Computes the QR decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using QR decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="QrDecompositionDouble"/>'.
        /// </returns>
        public static QrDecompositionDouble Qr(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new QrDecompositionDouble(a);
        }

        /// <summary>
        /// Computes the QR decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using QR decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="QrDecompositionComplex"/>'.
        /// </returns>
        public static QrDecompositionComplex Qr(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new QrDecompositionComplex(a);
        }

        /// <summary>
        /// Computes the Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using Cholesky decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="CholeskyDecompositionSingle"/>'.
        /// </returns>
        public static CholeskyDecompositionSingle Cholesky(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new CholeskyDecompositionSingle(a);
        }

        /// <summary>
        /// Computes the Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using Cholesky decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="CholeskyDecompositionDouble"/>'.
        /// </returns>
        public static CholeskyDecompositionDouble Cholesky(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new CholeskyDecompositionDouble(a);
        }

        /// <summary>
        /// Computes the Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using Cholesky decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="CholeskyDecompositionComplex"/>'.
        /// </returns>
        public static CholeskyDecompositionComplex Cholesky(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new CholeskyDecompositionComplex(a);
        }

        /// <summary>
        /// Gets the singular values of the matrix A.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <returns>
        /// The diagonal elements of the matrix S.
        /// </returns>
        public static Vec<float> GetSingularValues(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var s = new Vec<float>(Math.Min(a.RowCount, a.ColCount));
            SingularValueDecompositionSingle.GetSingularValues(a, s);
            return s;
        }

        /// <summary>
        /// Gets the singular values of the matrix A.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <returns>
        /// The diagonal elements of the matrix S.
        /// </returns>
        public static Vec<double> GetSingularValues(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var s = new Vec<double>(Math.Min(a.RowCount, a.ColCount));
            SingularValueDecompositionDouble.GetSingularValues(a, s);
            return s;
        }

        /// <summary>
        /// Gets the singular values of the matrix A.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <returns>
        /// The diagonal elements of the matrix S.
        /// </returns>
        public static Vec<double> GetSingularValues(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var s = new Vec<double>(Math.Min(a.RowCount, a.ColCount));
            SingularValueDecompositionComplex.GetSingularValues(a, s);
            return s;
        }

        /// <summary>
        /// Computes the SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using SVD.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="SingularValueDecompositionSingle"/>'.
        /// </returns>
        public static SingularValueDecompositionSingle Svd(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new SingularValueDecompositionSingle(a);
        }

        /// <summary>
        /// Computes the SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using SVD.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="SingularValueDecompositionDouble"/>'.
        /// </returns>
        public static SingularValueDecompositionDouble Svd(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new SingularValueDecompositionDouble(a);
        }

        /// <summary>
        /// Computes the SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using SVD.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="SingularValueDecompositionComplex"/>'.
        /// </returns>
        public static SingularValueDecompositionComplex Svd(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new SingularValueDecompositionComplex(a);
        }
    }
}
