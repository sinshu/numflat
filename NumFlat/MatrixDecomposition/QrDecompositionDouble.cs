using System;
using System.Buffers;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the QR decomposition.
    /// </summary>
    public class QrDecompositionDouble
    {
        private Mat<double> q;
        private Mat<double> r;

        /// <summary>
        /// Decomposes the matrix A using QR decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix A to be decomposed.
        /// </param>
        public QrDecompositionDouble(in Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            if (a.RowCount < a.ColCount)
            {
                throw new ArgumentException("'a.RowCount' must be greater than or equal to 'a.ColCount'.");
            }

            var q = new Mat<double>(a.RowCount, a.ColCount);
            var r = new Mat<double>(a.ColCount, a.ColCount);
            Decompose(a, q, r);

            this.q = q;
            this.r = r;
        }

        /// <summary>
        /// Decomposes the matrix A using QR decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix A to be decomposed.
        /// </param>
        /// <param name="q">
        /// The matrix Q.
        /// </param>
        /// <param name="r">
        /// The matrix R.
        /// </param>
        /// <remarks>
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
        public static unsafe void Decompose(in Mat<double> a, in Mat<double> q, in Mat<double> r)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(q, nameof(q));
            ThrowHelper.ThrowIfEmpty(r, nameof(r));

            if (a.RowCount < a.ColCount)
            {
                throw new ArgumentException("'a.RowCount' must be greater than or equal to 'a.ColCount'.");
            }

            if (q.RowCount != a.RowCount)
            {
                throw new ArgumentException("'q.RowCount' must match 'a.RowCount'.");
            }

            if (q.ColCount != a.ColCount)
            {
                throw new ArgumentException("'q.ColCount' must match 'a.ColCount'.");
            }

            if (r.RowCount != a.ColCount)
            {
                throw new ArgumentException("'r.RowCount' must match 'a.ColCount'.");
            }

            if (r.ColCount != a.ColCount)
            {
                throw new ArgumentException("'r.ColCount' must match 'a.ColCount'.");
            }

            using var tauBuffer = MemoryPool<double>.Shared.Rent(a.ColCount);
            var tau = tauBuffer.Memory.Span;

            a.CopyTo(q);

            fixed (double* pq = q.Memory.Span)
            fixed (double* ptau = tau)
            {
                Lapack.Dgeqrf(
                    MatrixLayout.ColMajor,
                    q.RowCount, q.ColCount,
                    pq, q.Stride,
                    ptau);

                var qCols = q.Cols;
                var rCols = r.Cols;
                r.Clear();
                for (var col = 0; col < r.ColCount; col++)
                {
                    qCols[col].Subvector(0, col + 1).CopyTo(rCols[col].Subvector(0, col + 1));
                }

                Lapack.Dorgqr(
                    MatrixLayout.ColMajor,
                    q.RowCount, q.ColCount, q.ColCount,
                    pq, q.Stride,
                    ptau);
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
        public unsafe void Solve(in Vec<double> b, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (b.Count != q.RowCount)
            {
                throw new ArgumentException("'b.Count' must match 'Q.RowCount'.");
            }

            if (destination.Count != r.RowCount)
            {
                throw new ArgumentException("'destination.Count' must match 'R.RowCount'.");
            }

            Mat.Mul(q, b, destination, true);

            fixed (double* pr = r.Memory.Span)
            fixed (double* pd = destination.Memory.Span)
            {
                Blas.Dtrsv(
                    Order.ColMajor,
                    Uplo.Upper,
                    Transpose.NoTrans,
                    Diag.NonUnit,
                    r.RowCount,
                    pr, r.Stride,
                    pd, destination.Stride);
            }
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
        public unsafe Vec<double> Solve(in Vec<double> b)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            if (b.Count != q.RowCount)
            {
                throw new ArgumentException("'b.Count' must match 'Q.RowCount'.");
            }

            var x = new Vec<double>(r.RowCount);
            Solve(b, x);
            return x;
        }

        /// <summary>
        /// The matrix Q.
        /// </summary>
        public ref readonly Mat<double> Q => ref q;

        /// <summary>
        /// The matrix R.
        /// </summary>
        public ref readonly Mat<double> R => ref r;
    }
}
