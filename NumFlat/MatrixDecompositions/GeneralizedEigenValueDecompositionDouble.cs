using System;
using MatFlat;

namespace NumFlat
{
    /// <summary>
    /// Provides the generalized eigenvalue decomposition (GEVD).
    /// </summary>
    public sealed class GeneralizedEigenValueDecompositionDouble
    {
        private readonly Vec<double> d;
        private readonly Mat<double> v;

        /// <summary>
        /// Decomposes the matrix using GEVD.
        /// </summary>
        /// <param name="a">
        /// The source matrix A.
        /// </param>
        /// <param name="b">
        /// The source matrix B.
        /// </param>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the GEVD.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public GeneralizedEigenValueDecompositionDouble(in Mat<double> a, in Mat<double> b)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfNonSquare(a, nameof(a));
            ThrowHelper.ThrowIfDifferentSize(a, b);

            var d = new Vec<double>(a.RowCount);
            var v = new Mat<double>(a.RowCount, a.ColCount);
            Decompose(a, b, d, v);

            this.d = d;
            this.v = v;
        }

        /// <summary>
        /// Decomposes the matrix using GEVD.
        /// </summary>
        /// <param name="a">
        /// The source matrix A.
        /// </param>
        /// <param name="b">
        /// The source matrix B.
        /// </param>
        /// <param name="d">
        /// The destination of the diagonal elements of the matrix D.
        /// </param>
        /// <param name="v">
        /// The destination of the the matrix V.
        /// </param>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the GEVD.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public static unsafe void Decompose(in Mat<double> a, in Mat<double> b, in Vec<double> d, in Mat<double> v)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(d, nameof(d));
            ThrowHelper.ThrowIfEmpty(v, nameof(v));
            ThrowHelper.ThrowIfNonSquare(a, nameof(a));
            ThrowHelper.ThrowIfDifferentSize(a, b, v);

            if (d.Count != a.RowCount)
            {
                throw new ArgumentException("'d.Count' must match 'a.RowCount'.");
            }

            using var utmp = TemporalMatrix.CopyFrom(b);
            ref readonly var tmp = ref utmp.Item;

            using var ucd = d.EnsureContiguous(false);
            ref readonly var cd = ref ucd.Item;

            a.CopyTo(v);

            fixed (double* pv = v.Memory.Span)
            fixed (double* ptmp = tmp.Memory.Span)
            fixed (double* pcd = cd.Memory.Span)
            {
                try
                {
                    Factorization.Gevd(v.RowCount, pv, v.Stride, ptmp, tmp.Stride, pcd);
                }
                catch (MatFlat.MatrixFactorizationException)
                {
                    throw new MatrixFactorizationException("GEVD failed. The right-hand side matrix is not positive definite.");
                }
            }
        }

        /// <summary>
        /// The diagonal elements of the matrix D.
        /// </summary>
        public ref readonly Vec<double> D => ref d;

        /// <summary>
        /// The matrix V.
        /// </summary>
        public ref readonly Mat<double> V => ref v;
    }
}
