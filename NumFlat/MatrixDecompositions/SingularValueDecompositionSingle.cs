using System;
using System.Buffers;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the singular value decomposition (SVD).
    /// </summary>
    public sealed class SingularValueDecompositionSingle : MatrixDecompositionBase<float>
    {
        private readonly Vec<float> s;
        private readonly Mat<float> u;
        private readonly Mat<float> vt;

        /// <summary>
        /// Decomposes the matrix using SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed to compute the SVD.
        /// </exception>
        public SingularValueDecompositionSingle(in Mat<float> a) : base(a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var s = new Vec<float>(Math.Min(a.RowCount, a.ColCount));
            var u = new Mat<float>(a.RowCount, a.RowCount);
            var vt = new Mat<float>(a.ColCount, a.ColCount);
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
        public static unsafe void GetSingularValues(in Mat<float> a, in Vec<float> s)
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

            using var uwork = MemoryPool<float>.Shared.Rent(Math.Min(tmp.RowCount, tmp.ColCount) - 1);
            var work = uwork.Memory.Span;

            fixed (float* ptmp = tmp.Memory.Span)
            fixed (float* pcs = cs.Memory.Span)
            fixed (float* pwork = work)
            {
                var info = Lapack.Sgesvd(
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
                    throw new LapackException("The SVD did not converge.", nameof(Lapack.Sgesvd), (int)info);
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
        public static unsafe void Decompose(in Mat<float> a, in Vec<float> s, in Mat<float> u, in Mat<float> vt)
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

            using var uwork = MemoryPool<float>.Shared.Rent(Math.Min(tmp.RowCount, tmp.ColCount) - 1);
            var work = uwork.Memory.Span;

            fixed (float* ptmp = tmp.Memory.Span)
            fixed (float* pcs = cs.Memory.Span)
            fixed (float* pu = u.Memory.Span)
            fixed (float* pvt = vt.Memory.Span)
            fixed (float* pwork = work)
            {
                var info = Lapack.Sgesvd(
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
                    throw new LapackException("The SVD did not converge.", nameof(Lapack.Sgesvd), (int)info);
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
        /// <exception cref="LapackException">
        /// Failed to compute the SVD.
        /// </exception>
        public static unsafe void Decompose(in Mat<float> a, in Vec<float> s, in Mat<float> u)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(s, nameof(s));
            ThrowHelper.ThrowIfEmpty(u, nameof(u));

            if (s.Count != Math.Min(a.RowCount, a.ColCount))
            {
                throw new ArgumentException("'s.Count' must match 'min(a.RowCount, a.ColCount)'.");
            }

            if (u.RowCount != a.RowCount || u.ColCount != a.RowCount)
            {
                throw new ArgumentException("'u.RowCount' and 'u.ColCount' must match 'a.RowCount'.");
            }

            using var utmp = TemporalMatrix.CopyFrom(a);
            ref readonly var tmp = ref utmp.Item;

            using var ucs = s.EnsureContiguous(false);
            ref readonly var cs = ref ucs.Item;

            using var uwork = MemoryPool<float>.Shared.Rent(Math.Min(tmp.RowCount, tmp.ColCount) - 1);
            var work = uwork.Memory.Span;

            fixed (float* ptmp = tmp.Memory.Span)
            fixed (float* pcs = cs.Memory.Span)
            fixed (float* pu = u.Memory.Span)
            fixed (float* pwork = work)
            {
                var info = Lapack.Sgesvd(
                    MatrixLayout.ColMajor,
                    'A', 'N',
                    tmp.RowCount, tmp.ColCount,
                    ptmp, tmp.Stride,
                    pcs,
                    pu, u.Stride,
                    null, 1,
                    pwork);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The SVD did not converge.", nameof(Lapack.Sgesvd), (int)info);
                }
            }
        }

        /// <inheritdoc/>
        public unsafe override void Solve(in Vec<float> b, in Vec<float> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowIfInvalidSize(b, destination);

            using var utmp = new TemporalVector<float>(vt.RowCount);
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
        public float Determinant()
        {
            var determinant = 1.0F;
            foreach (var value in s.FastEnumerate())
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
        public float LogDeterminant()
        {
            var logDeterminant = 0.0F;
            foreach (var value in s.FastEnumerate())
            {
                logDeterminant += MathF.Log(value);
            }
            return logDeterminant;
        }

        /// <summary>
        /// The diagonal elements of the matrix S.
        /// </summary>
        public ref readonly Vec<float> S => ref s;

        /// <summary>
        /// The matrix U.
        /// </summary>
        public ref readonly Mat<float> U => ref u;

        /// <summary>
        /// The matrix V^T.
        /// </summary>
        public ref readonly Mat<float> VT => ref vt;
    }
}
