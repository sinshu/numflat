using System;

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
        /// An instance of <see cref="SingularValueDecompositionDouble"/>.
        /// </returns>
        public static SingularValueDecompositionDouble Svd(this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(ref a, nameof(a));

            return new SingularValueDecompositionDouble(a);
        }
    }
}
