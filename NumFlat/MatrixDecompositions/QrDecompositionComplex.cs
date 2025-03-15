using System;
using System.Buffers;
using System.Numerics;
using MatFlat;

namespace NumFlat
{
    /// <summary>
    /// Provides the QR decomposition.
    /// </summary>
    public sealed class QrDecompositionComplex : MatrixDecompositionBase<Complex>
    {
        private readonly Mat<Complex> q;
        private readonly Mat<Complex> r;

        /// <summary>
        /// Decomposes the matrix using QR decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        public QrDecompositionComplex(in Mat<Complex> a) : base(a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            if (a.RowCount < a.ColCount)
            {
                throw new ArgumentException("The number of rows must be greater than or equal to the number of columns.");
            }

            var q = new Mat<Complex>(a.RowCount, a.ColCount);
            var r = new Mat<Complex>(a.ColCount, a.ColCount);
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
        public static unsafe void Decompose(in Mat<Complex> a, in Mat<Complex> q, in Mat<Complex> r)
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

            using var urdiag = new TemporalArray<double>(a.ColCount);
            var rdiag = urdiag.Item;

            fixed (Complex* ptmp = tmp.Memory.Span)
            fixed (double* prdiag = rdiag)
            fixed (Complex* pq = q.Memory.Span)
            fixed (Complex* pr = r.Memory.Span)
            {
                Factorization.Qr(tmp.RowCount, tmp.ColCount, ptmp, tmp.Stride, prdiag);
                Factorization.QrOrthogonalFactor(tmp.RowCount, tmp.ColCount, ptmp, tmp.Stride, pq, q.Stride);
                Factorization.QrUpperTriangularFactor(tmp.RowCount, tmp.ColCount, ptmp, tmp.Stride, pr, r.Stride, prdiag);
            }
        }

        /// <inheritdoc/>
        public unsafe override void Solve(in Vec<Complex> b, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowIfInvalidSize(b, destination);

            Mat.Mul(q, b, destination, true, true);

            fixed (Complex* pr = r.Memory.Span)
            fixed (Complex* pd = destination.Memory.Span)
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
                determinant *= value.Magnitude;
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
                logDeterminant += Math.Log(value.Magnitude);
            }
            return logDeterminant;
        }

        /// <summary>
        /// The matrix Q.
        /// </summary>
        public ref readonly Mat<Complex> Q => ref q;

        /// <summary>
        /// The matrix R.
        /// </summary>
        public ref readonly Mat<Complex> R => ref r;
    }
}
