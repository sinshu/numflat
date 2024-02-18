using System;
using System.Buffers;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the singular value decomposition (SVD).
    /// </summary>
    public sealed class SingularValueDecompositionDouble : MatrixDecompositionBase<double>
    {
        private readonly Vec<double> s;
        private readonly Mat<double> u;
        private readonly Mat<double> vt;

        /// <summary>
        /// Decomposes the matrix using SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed to compute the SVD.
        /// </exception>
        public SingularValueDecompositionDouble(in Mat<double> a) : base(a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var s = new Vec<double>(Math.Min(a.RowCount, a.ColCount));
            var u = new Mat<double>(a.RowCount, a.RowCount);
            var vt = new Mat<double>(a.ColCount, a.ColCount);
            Decompose(a, s, u, vt);

            this.s = s;
            this.u = u;
            this.vt = vt;
        }

        /// <summary>
        /// Gets the singular values of the matrix.
        /// </summary>
        /// <param name="a">
        /// The source matrix.
        /// </param>
        /// <param name="s">
        /// The destination of the diagonal elements of the matrix S.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed to compute the SVD.
        /// </exception>
        public static unsafe void GetSingularValues(in Mat<double> a, in Vec<double> s)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(s, nameof(s));

            if (s.Count != Math.Min(a.RowCount, a.ColCount))
            {
                throw new ArgumentException("'s.Count' must match 'min(a.RowCount, a.ColCount)'.");
            }

            using var utmp = TemporalMatrix.CopyFrom(a);
            ref readonly var tmp = ref utmp.Item;

            using var ucs = s.EnsureContiguous(false);
            ref readonly var cs = ref ucs.Item;

            using var uwork = MemoryPool<double>.Shared.Rent(Math.Min(tmp.RowCount, tmp.ColCount) - 1);
            var work = uwork.Memory.Span;

            fixed (double* ptmp = tmp.Memory.Span)
            fixed (double* pcs = cs.Memory.Span)
            fixed (double* pwork = work)
            {
                var info = Lapack.Dgesvd(
                    MatrixLayout.ColMajor,
                    'N', 'N',
                    tmp.RowCount, tmp.ColCount,
                    ptmp, tmp.Stride,
                    pcs,
                    null, 1,
                    null, 1,
                    pwork);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The SVD did not converge.", nameof(Lapack.Dgesvd), (int)info);
                }
            }
        }

        /// <summary>
        /// Decomposes the matrix using SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <param name="s">
        /// The destination of the diagonal elements of the matrix S.
        /// </param>
        /// <param name="u">
        /// The destination of the the matrix U.
        /// </param>
        /// <param name="vt">
        /// The destination of the the matrix V^T.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed to compute the SVD.
        /// </exception>
        public static unsafe void Decompose(in Mat<double> a, in Vec<double> s, in Mat<double> u, in Mat<double> vt)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(s, nameof(s));
            ThrowHelper.ThrowIfEmpty(u, nameof(u));
            ThrowHelper.ThrowIfEmpty(vt, nameof(vt));

            if (s.Count != Math.Min(a.RowCount, a.ColCount))
            {
                throw new ArgumentException("'s.Count' must match 'min(a.RowCount, a.ColCount)'.");
            }

            if (u.RowCount != a.RowCount || u.ColCount != a.RowCount)
            {
                throw new ArgumentException("'u.RowCount' and 'u.ColCount' must match 'a.RowCount'.");
            }

            if (vt.RowCount != a.ColCount || vt.ColCount != a.ColCount)
            {
                throw new ArgumentException("'vt.RowCount' and 'vt.ColCount' must match 'a.ColCount'.");
            }

            using var utmp = TemporalMatrix.CopyFrom(a);
            ref readonly var tmp = ref utmp.Item;

            using var ucs = s.EnsureContiguous(false);
            ref readonly var cs = ref ucs.Item;

            using var uwork = MemoryPool<double>.Shared.Rent(Math.Min(tmp.RowCount, tmp.ColCount) - 1);
            var work = uwork.Memory.Span;

            fixed (double* ptmp = tmp.Memory.Span)
            fixed (double* pcs = cs.Memory.Span)
            fixed (double* pu = u.Memory.Span)
            fixed (double* pvt = vt.Memory.Span)
            fixed (double* pwork = work)
            {
                var info = Lapack.Dgesvd(
                    MatrixLayout.ColMajor,
                    'A', 'A',
                    tmp.RowCount, tmp.ColCount,
                    ptmp, tmp.Stride,
                    pcs,
                    pu, u.Stride,
                    pvt, vt.Stride,
                    pwork);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("Failed to compute the SVD.", nameof(Lapack.Dgesvd), (int)info);
                }
            }
        }

        /// <inheritdoc/>
        public unsafe override void Solve(in Vec<double> b, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowIfInvalidSize(b, destination);

            using var utmp = new TemporalVector<double>(vt.RowCount);
            ref readonly var tmp = ref utmp.Item;
            tmp.Clear();

            var ts = tmp.Subvector(0, s.Count);
            Mat.Mul(u.Submatrix(0, 0, u.RowCount, s.Count), b, ts, true);
            Vec.PointwiseDiv(ts, s, ts);
            Mat.Mul(vt, tmp, destination, true);
        }

        /// <summary>
        /// Computes the absolute determinant of the source matrix.
        /// </summary>
        /// <returns>
        /// The absolute determinant of the source matrix.
        /// </returns>
        public double Determinant()
        {
            var determinant = 1.0;
            foreach (var value in s)
            {
                determinant *= value;
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
            var logDeterminant = 0.0;
            foreach (var value in s)
            {
                logDeterminant += Math.Log(value);
            }
            return logDeterminant;
        }

        /// <summary>
        /// The diagonal elements of the matrix S.
        /// </summary>
        public ref readonly Vec<double> S => ref s;

        /// <summary>
        /// The matrix U.
        /// </summary>
        public ref readonly Mat<double> U => ref u;

        /// <summary>
        /// The matrix V^T.
        /// </summary>
        public ref readonly Mat<double> VT => ref vt;
    }
}
