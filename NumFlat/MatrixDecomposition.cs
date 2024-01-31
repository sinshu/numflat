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
        /// Compute the SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed with SVD.
        /// </param>
        /// <returns>
        /// An instance of <see cref="SingularValueDecompositionSingle"/>.
        /// </returns>
        public static SingularValueDecompositionSingle Svd(this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(ref a, nameof(a));

            return new SingularValueDecompositionSingle(a);
        }

        /// <summary>
        /// Compute the SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed with SVD.
        /// </param>
        /// <returns>
        /// An instance of <see cref="SingularValueDecompositionDouble"/>.
        /// </returns>
        public static SingularValueDecompositionDouble Svd(this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(ref a, nameof(a));

            return new SingularValueDecompositionDouble(a);
        }

        /// <summary>
        /// Compute the SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed with SVD.
        /// </param>
        /// <returns>
        /// An instance of <see cref="SingularValueDecompositionComplex"/>.
        /// </returns>
        public static SingularValueDecompositionComplex Svd(this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(ref a, nameof(a));

            return new SingularValueDecompositionComplex(a);
        }
    }
}
