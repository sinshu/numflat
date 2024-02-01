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
