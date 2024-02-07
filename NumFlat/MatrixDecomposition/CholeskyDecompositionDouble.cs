using System;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the Cholesky decomposition.
    /// </summary>
    public class CholeskyDecompositionDouble
    {
        private Mat<double> l;

        /// <summary>
        /// Decomposes the matrix using Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        public CholeskyDecompositionDouble(in Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            if (a.RowCount != a.ColCount)
            {
                throw new ArgumentException("The matrix must be a square matrix.");
            }

            var l = new Mat<double>(a.RowCount, a.ColCount);
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
        public static unsafe void Decompose(in Mat<double> a, in Mat<double> l)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(l, nameof(l));
            ThrowHelper.ThrowIfDifferentSize(a, l);

            if (a.RowCount != a.ColCount)
            {
                throw new ArgumentException("'a' must be a square matrix.");
            }

            a.CopyTo(l);

            fixed (double* pl = l.Memory.Span)
            {
                var info = Lapack.Dpotrf(
                    MatrixLayout.ColMajor,
                    'L',
                    l.RowCount,
                    pl, l.Stride);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Dpotrf), (int)info);
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
        public unsafe void Solve(in Vec<double> b, in Vec<double> destination)
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

            fixed (double* pl = l.Memory.Span)
            fixed (double* pcdst = cdst.Memory.Span)
            {
                var info = Lapack.Dpotrs(
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
        public Vec<double> Solve(in Vec<double> b)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            if (b.Count != l.RowCount)
            {
                throw new ArgumentException("The length of the input vector does not meet the requirement.");
            }

            var x = new Vec<double>(l.RowCount);
            Solve(b, x);
            return x;
        }

        /// <summary>
        /// The matrix L.
        /// </summary>
        public ref readonly Mat<double> L => ref l;
    }
}
