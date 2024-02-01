using System;
using System.Buffers;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the singular value decomposition (SVD).
    /// </summary>
    public class SvdDouble
    {
        private readonly Vec<double> s;
        private readonly Mat<double> u;
        private readonly Mat<double> vt;

        /// <summary>
        /// Decomposes the matrix A with SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix A to be decomposed.
        /// </param>
        public SvdDouble(in Mat<double> a)
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
        /// Decomposes the matrix A with SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix A to be decomposed.
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
        /// The SVD computation did not converge.
        /// </exception>
        public static unsafe void Decompose(in Mat<double> a, in Vec<double> s, in Mat<double> u, in Mat<double> vt)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(s, nameof(s));
            ThrowHelper.ThrowIfEmpty(u, nameof(u));
            ThrowHelper.ThrowIfEmpty(vt, nameof(vt));

            if (s.Count != Math.Min(a.RowCount, a.ColCount))
            {
                throw new ArgumentException("'s.Count' must match 'Min(a.RowCount, a.ColCount)'.");
            }

            if (u.RowCount != a.RowCount || u.ColCount != a.RowCount)
            {
                throw new ArgumentException("'u.RowCount' and 'u.ColCount' must match 'a.RowCount'.");
            }

            if (vt.RowCount != a.ColCount || vt.ColCount != a.ColCount)
            {
                throw new ArgumentException("'vt.RowCount' and 'vt.ColCount' must match 'a.ColCount'.");
            }

            //
            // Need to copy 'a' and 's' here.
            //
            //   a: LAPACK SVD will destroy 'a'.
            //      We have to copy original 'a' to preserve the original content.
            //
            //   s: Write buffer for 's' must be contiguous. Given Vec might not be so.
            //      Creating Vec for 's' with stride = 1 to ensure contiguity.
            //

            var aLength = a.RowCount * a.ColCount;
            using var aBuffer = MemoryPool<double>.Shared.Rent(aLength);
            var aCopy = new Mat<double>(a.RowCount, a.ColCount, a.RowCount, aBuffer.Memory.Slice(0, aLength));
            a.CopyTo(aCopy);

            var sLength = Math.Min(a.RowCount, a.ColCount);
            using var sBuffer = MemoryPool<double>.Shared.Rent(sLength);
            var sCopy = new Vec<double>(sLength, 1, sBuffer.Memory.Slice(0, sLength));

            using var workBuffer = MemoryPool<double>.Shared.Rent(Math.Min(a.RowCount, a.ColCount) - 1);
            var work = workBuffer.Memory.Span;

            fixed (double* pa = aCopy.Memory.Span)
            fixed (double* ps = sCopy.Memory.Span)
            fixed (double* pu = u.Memory.Span)
            fixed (double* pvt = vt.Memory.Span)
            fixed (double* pwork = work)
            {
                var info = Lapack.Dgesvd(
                    MatrixLayout.ColMajor,
                    'A', 'A',
                    aCopy.RowCount, aCopy.ColCount,
                    pa, aCopy.Stride,
                    ps,
                    pu, u.Stride,
                    pvt, vt.Stride,
                    pwork);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The SVD computation did not converge.", nameof(Lapack.Dgesvd), (int)info);
                }
            }

            sCopy.CopyTo(s);
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
