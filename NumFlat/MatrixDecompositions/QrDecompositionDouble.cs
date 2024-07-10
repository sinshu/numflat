using System;
using System.Buffers;
using MatFlat;

namespace NumFlat
{
    /// <summary>
    /// Provides the QR decomposition.
    /// </summary>
    public sealed class QrDecompositionDouble : MatrixDecompositionBase<double>
    {
        private readonly Mat<double> q;
        private readonly Mat<double> r;

        /// <summary>
        /// Decomposes the matrix using QR decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        public QrDecompositionDouble(in Mat<double> a) : base(a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            if (a.RowCount < a.ColCount)
            {
                throw new ArgumentException("The number of rows must be greater than or equal to the number of columns.");
            }

            var q = new Mat<double>(a.RowCount, a.ColCount);
            var r = new Mat<double>(a.ColCount, a.ColCount);
            Decompose(a, q, r);

            this.q = q;
            this.r = r;
        }

        /// <summary>
        /// Decomposes the matrix using QR decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <param name="q">
        /// The destination of the matrix Q.
        /// </param>
        /// <param name="r">
        /// The destination of the matrix R.
        /// </param>
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

            using var utmp = TemporalMatrix.CopyFrom(a);
            ref readonly var tmp = ref utmp.Item;

            using var urdiag = MemoryPool<double>.Shared.Rent(a.ColCount);
            var rdiag = urdiag.Memory.Span;

            fixed (double* ptmp = tmp.Memory.Span)
            fixed (double* prdiag = rdiag)
            fixed (double* pq = q.Memory.Span)
            fixed (double* pr = r.Memory.Span)
            {
                Factorization.Qr(tmp.RowCount, tmp.ColCount, ptmp, tmp.Stride, prdiag);
                Factorization.QrOrthogonalFactor(tmp.RowCount, tmp.ColCount, ptmp, tmp.Stride, pq, q.Stride);
                Factorization.QrUpperTriangularFactor(tmp.RowCount, tmp.ColCount, ptmp, tmp.Stride, pr, r.Stride, prdiag);
            }
        }

        /// <inheritdoc/>
        public unsafe override void Solve(in Vec<double> b, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowIfInvalidSize(b, destination);

            Mat.Mul(q, b, destination, true);

            fixed (double* pr = r.Memory.Span)
            fixed (double* pd = destination.Memory.Span)
            {
                Blas.SolveTriangular(Uplo.Upper, Transpose.NoTrans, r.RowCount, pr, r.Stride, pd, destination.Stride);
            }
        }

        /// <summary>
        /// Computes the absolute determinant of the source matrix.
        /// </summary>
        /// <returns>
        /// The absolute determinant of the source matrix.
        /// </returns>
        public double Determinant()
        {
            if (q.RowCount != q.ColCount)
            {
                throw new InvalidOperationException("Calling this method against a non-square QR decomposition is not allowed.");
            }

            var determinant = 1.0;
            foreach (var value in r.EnumerateDiagonalElements())
            {
                determinant *= Math.Abs(value);
            }
            return determinant;
        }

        /// <summary>
        /// Computes the log absolute determinant of the source matrix.
        /// </summary>
        /// <returns>
        /// The log absolute determinant of the source matrix.
        /// </returns>
        public double LogDeterminant()
        {
            if (q.RowCount != q.ColCount)
            {
                throw new InvalidOperationException("Calling this method against a non-square QR decomposition is not allowed.");
            }

            var logDeterminant = 0.0;
            foreach (var value in r.EnumerateDiagonalElements())
            {
                logDeterminant += Math.Log(Math.Abs(value));
            }
            return logDeterminant;
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
