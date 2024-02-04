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
        /// An instance of '<see cref="LuSingle"/>'.
        /// </returns>
        public static LuSingle Lu(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new LuSingle(a);
        }

        /// <summary>
        /// Computes the LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using LU decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="LuDouble"/>'.
        /// </returns>
        public static LuDouble Lu(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new LuDouble(a);
        }

        /// <summary>
        /// Computes the LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using LU decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="LuComplex"/>'.
        /// </returns>
        public static LuComplex Lu(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new LuComplex(a);
        }

        /// <summary>
        /// Computes the QR decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using QR decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="QrSingle"/>'.
        /// </returns>
        public static QrSingle Qr(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new QrSingle(a);
        }

        /// <summary>
        /// Computes the QR decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using QR decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="QrDouble"/>'.
        /// </returns>
        public static QrDouble Qr(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new QrDouble(a);
        }

        /// <summary>
        /// Computes the QR decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using QR decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="QrComplex"/>'.
        /// </returns>
        public static QrComplex Qr(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new QrComplex(a);
        }

        /// <summary>
        /// Computes the Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using Cholesky decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="CholeskySingle"/>'.
        /// </returns>
        public static CholeskySingle Cholesky(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new CholeskySingle(a);
        }

        /// <summary>
        /// Computes the Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using Cholesky decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="CholeskyDouble"/>'.
        /// </returns>
        public static CholeskyDouble Cholesky(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new CholeskyDouble(a);
        }

        /// <summary>
        /// Computes the Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using Cholesky decomposition.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="CholeskyComplex"/>'.
        /// </returns>
        public static CholeskyComplex Cholesky(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new CholeskyComplex(a);
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
            SvdSingle.GetSingularValues(a, s);
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
            SvdDouble.GetSingularValues(a, s);
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
            SvdComplex.GetSingularValues(a, s);
            return s;
        }

        /// <summary>
        /// Computes the SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using SVD.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="SvdSingle"/>'.
        /// </returns>
        public static SvdSingle Svd(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new SvdSingle(a);
        }

        /// <summary>
        /// Computes the SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using SVD.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="SvdDouble"/>'.
        /// </returns>
        public static SvdDouble Svd(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new SvdDouble(a);
        }

        /// <summary>
        /// Computes the SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed using SVD.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="SvdComplex"/>'.
        /// </returns>
        public static SvdComplex Svd(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new SvdComplex(a);
        }
    }
}
