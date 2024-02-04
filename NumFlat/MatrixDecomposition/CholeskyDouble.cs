using System;
using System.Buffers;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the Cholesky decomposition.
    /// </summary>
    public class CholeskyDouble
    {
        private Mat<double> l;

        /// <summary>
        /// Decomposes the matrix A using Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix A to be decomposed.
        /// </param>
        public CholeskyDouble(in Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            if (a.RowCount != a.ColCount)
            {
                throw new ArgumentException("The matrix 'a' must be a square matrix.");
            }

            var l = new Mat<double>(a.RowCount, a.ColCount);
            Decompose(a, l);

            this.l = l;
        }

        /// <summary>
        /// Decomposes the matrix A using Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix A to be decomposed.
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
                throw new ArgumentException("The matrix 'a' must be a square matrix.");
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
        /// Compute a vector x from b, where Ax = b.
        /// </summary>
        /// <param name="b">
        /// The vector b.
        /// </param>
        /// <param name="destination">
        /// The destination of the vector x.
        /// </param>
        /// <remarks>
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
        public unsafe void Solve(in Vec<double> b, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (b.Count != l.RowCount)
            {
                throw new ArgumentException("'b.Count' must match 'L.RowCount'.");
            }

            if (destination.Count != l.RowCount)
            {
                throw new ArgumentException("'destination.Count' must match 'L.RowCount'.");
            }

            var tmpLength = l.RowCount;
            using var tmpBuffer = MemoryPool<double>.Shared.Rent(tmpLength);
            var tmp = new Vec<double>(tmpBuffer.Memory.Slice(0, tmpLength));
            b.CopyTo(tmp);

            fixed (double* pl = l.Memory.Span)
            fixed (double* ptmp = tmp.Memory.Span)
            {
                var info = Lapack.Dpotrs(
                    MatrixLayout.ColMajor,
                    'L',
                    l.RowCount,
                    1,
                    pl, l.Stride,
                    ptmp, tmpLength);

            }

            tmp.CopyTo(destination);
        }

        /// <summary>
        /// Compute a vector x from b, where Ax = b.
        /// </summary>
        /// <param name="b">
        /// The vector b.
        /// </param>
        /// <returns>
        /// The vector x.
        /// </returns>
        /// <remarks>
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
        public Vec<double> Solve(in Vec<double> b)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            if (b.Count != l.RowCount)
            {
                throw new ArgumentException("'b.Count' must match 'L.RowCount'.");
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
