using System;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the Cholesky decomposition.
    /// </summary>
    public class CholeskyDecompositionSingle
    {
        private Mat<float> l;

        /// <summary>
        /// Decomposes the matrix using Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <exception cref="LapackException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be Hermitian symmetric.
        /// Note that this implementation does not check if the input matrix is Hermitian symmetric.
        /// Specifically, only the lower triangular part of the input matrix is referenced, and the rest is ignored.
        /// </remarks>
        public CholeskyDecompositionSingle(in Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            if (a.RowCount != a.ColCount)
            {
                throw new ArgumentException("The matrix must be a square matrix.");
            }

            var l = new Mat<float>(a.RowCount, a.ColCount);
            Decompose(a, l);

            this.l = l;
        }

        /// <summary>
        /// Decomposes the matrix using Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <param name="l">
        /// The destination of the the matrix L.
        /// </param>
        /// <exception cref="LapackException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be Hermitian symmetric.
        /// Note that this implementation does not check if the input matrix is Hermitian symmetric.
        /// Specifically, only the lower triangular part of the input matrix is referenced, and the rest is ignored.
        /// </remarks>
        public static unsafe void Decompose(in Mat<float> a, in Mat<float> l)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(l, nameof(l));
            ThrowHelper.ThrowIfDifferentSize(a, l);

            if (a.RowCount != a.ColCount)
            {
                throw new ArgumentException("'a' must be a square matrix.");
            }

            if (l.RowCount != a.RowCount || l.ColCount != a.ColCount)
            {
                throw new ArgumentException("The order of 'l' must match 'a'.");
            }

            a.CopyTo(l);

            fixed (float* pl = l.Memory.Span)
            {
                var info = Lapack.Spotrf(
                    MatrixLayout.ColMajor,
                    'L',
                    l.RowCount,
                    pl, l.Stride);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Spotrf), (int)info);
                }
            }

            var lCols = l.Cols;
            for (var col = 1; col < lCols.Count; col++)
            {
                lCols[col].Subvector(0, col).Clear();
            }
        }

        /// <summary>
        /// Solves the linear equation, Ax = b.
        /// </summary>
        /// <param name="b">
        /// The input vector.
        /// </param>
        /// <param name="destination">
        /// The destination of the solution vector.
        /// </param>
        public unsafe void Solve(in Vec<float> b, in Vec<float> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (b.Count != l.RowCount)
            {
                throw new ArgumentException("'b.Count' must match the order of L.");
            }

            if (destination.Count != l.RowCount)
            {
                throw new ArgumentException("'destination.Count' must match the order of L.");
            }

            using var ucdst = TemporalContiguousVector.EnsureContiguous(destination, false);
            ref readonly var cdst = ref ucdst.Item;
            b.CopyTo(cdst);

            fixed (float* pl = l.Memory.Span)
            fixed (float* pcdst = cdst.Memory.Span)
            {
                var info = Lapack.Spotrs(
                    MatrixLayout.ColMajor,
                    'L',
                    l.RowCount,
                    1,
                    pl, l.Stride,
                    pcdst, cdst.Count);
            }
        }

        /// <summary>
        /// Solves the linear equation, Ax = b.
        /// </summary>
        /// <param name="b">
        /// The input vector.
        /// </param>
        /// <returns>
        /// The solution vector.
        /// </returns>
        public Vec<float> Solve(in Vec<float> b)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            if (b.Count != l.RowCount)
            {
                throw new ArgumentException("The length of the input vector does not meet the requirement.");
            }

            var x = new Vec<float>(l.RowCount);
            Solve(b, x);
            return x;
        }

        /// <summary>
        /// The matrix L.
        /// </summary>
        public ref readonly Mat<float> L => ref l;
    }
}
