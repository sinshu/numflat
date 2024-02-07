using System;
using System.Buffers;
using System.Numerics;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the singular value decomposition (SVD).
    /// </summary>
    public class SingularValueDecompositionComplex
    {
        private readonly Vec<double> s;
        private readonly Mat<Complex> u;
        private readonly Mat<Complex> vt;

        /// <summary>
        /// Decomposes the matrix using SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        public SingularValueDecompositionComplex(in Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var s = new Vec<double>(Math.Min(a.RowCount, a.ColCount));
            var u = new Mat<Complex>(a.RowCount, a.RowCount);
            var vt = new Mat<Complex>(a.ColCount, a.ColCount);
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
        public static unsafe void GetSingularValues(in Mat<Complex> a, in Vec<double> s)
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

            fixed (Complex* ptmp = tmp.Memory.Span)
            fixed (double* pcs = cs.Memory.Span)
            fixed (double* pwork = work)
            {
                var info = Lapack.Zgesvd(
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
                    throw new LapackException("Failed to compute the SVD.", nameof(Lapack.Zgesvd), (int)info);
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
        public static unsafe void Decompose(in Mat<Complex> a, in Vec<double> s, in Mat<Complex> u, in Mat<Complex> vt)
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

            fixed (Complex* ptmp = tmp.Memory.Span)
            fixed (double* pcs = cs.Memory.Span)
            fixed (Complex* pu = u.Memory.Span)
            fixed (Complex* pvt = vt.Memory.Span)
            fixed (double* pwork = work)
            {
                var info = Lapack.Zgesvd(
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
                    throw new LapackException("Failed to compute the SVD.", nameof(Lapack.Zgesvd), (int)info);
                }
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
        public void Solve(in Vec<Complex> b, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (b.Count != u.RowCount)
            {
                throw new ArgumentException("'b.Count' must match 'U.RowCount'.");
            }

            if (destination.Count != vt.RowCount)
            {
                throw new ArgumentException("'destination.Count' must match 'VT.RowCount'.");
            }

            using var utmp = new TemporalVector<Complex>(vt.RowCount);
            ref readonly var tmp = ref utmp.Item;
            tmp.Clear();

            var ts = tmp.Subvector(0, s.Count);
            Mat.Mul(u.Submatrix(0, 0, u.RowCount, s.Count), b, ts, true, true);
            PointwiseDiv(ts, s, ts);
            Mat.Mul(vt, tmp, destination, true, true);
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
        public Vec<Complex> Solve(in Vec<Complex> b)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            if (b.Count != u.RowCount)
            {
                throw new ArgumentException("The length of the input vector does not meet the requirement.");
            }

            var x = new Vec<Complex>(vt.RowCount);
            Solve(b, x);
            return x;
        }

        private static void PointwiseDiv(in Vec<Complex> x, in Vec<double> y, in Vec<Complex> destination)
        {
            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var py = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px] / sy[py];
                px += x.Stride;
                py += y.Stride;
                pd += destination.Stride;
            }
        }

        /// <summary>
        /// The diagonal elements of the matrix S.
        /// </summary>
        public ref readonly Vec<double> S => ref s;

        /// <summary>
        /// The matrix U.
        /// </summary>
        public ref readonly Mat<Complex> U => ref u;

        /// <summary>
        /// The matrix V^T.
        /// </summary>
        public ref readonly Mat<Complex> VT => ref vt;
    }
}
