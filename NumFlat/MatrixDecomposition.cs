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
        /// The matrix to be decomposed with SVD.
        /// </param>
        /// <returns>
        /// An instance of <see cref="SvdSingle"/>.
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
        /// The matrix to be decomposed with SVD.
        /// </param>
        /// <returns>
        /// An instance of <see cref="SvdDouble"/>.
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
        /// The matrix to be decomposed with SVD.
        /// </param>
        /// <returns>
        /// An instance of <see cref="SvdComplex"/>.
        /// </returns>
        public static SvdComplex Svd(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new SvdComplex(a);
        }
    }
}
