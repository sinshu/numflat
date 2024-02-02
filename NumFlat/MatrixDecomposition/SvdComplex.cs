using System;
using System.Buffers;
using System.Numerics;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the singular value decomposition (SVD).
    /// </summary>
    public class SvdComplex
    {
        private readonly Vec<double> s;
        private readonly Mat<Complex> u;
        private readonly Mat<Complex> vt;

        /// <summary>
        /// Decomposes the matrix A with SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix A to be decomposed.
        /// </param>
        public SvdComplex(in Mat<Complex> a)
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
        /// Gets the singular values of the matrix A.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <param name="s">
        /// The destination of the diagonal elements of the matrix S.
        /// </param>
        /// <exception cref="LapackException">
        /// The SVD computation did not converge.
        /// </exception>
        /// <remarks>
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
        public static unsafe void GetSingularValues(in Mat<Complex> a, in Vec<double> s)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(s, nameof(s));

            if (s.Count != Math.Min(a.RowCount, a.ColCount))
            {
                throw new ArgumentException("'s.Count' must match 'Min(a.RowCount, a.ColCount)'.");
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
            using var aBuffer = MemoryPool<Complex>.Shared.Rent(aLength);
            var aCopy = new Mat<Complex>(a.RowCount, a.ColCount, a.RowCount, aBuffer.Memory.Slice(0, aLength));
            a.CopyTo(aCopy);

            var sLength = Math.Min(aCopy.RowCount, aCopy.ColCount);
            using var sBuffer = MemoryPool<double>.Shared.Rent(sLength);
            var sCopy = new Vec<double>(sLength, 1, sBuffer.Memory.Slice(0, sLength));

            using var workBuffer = MemoryPool<double>.Shared.Rent(Math.Min(aCopy.RowCount, aCopy.ColCount) - 1);
            var work = workBuffer.Memory.Span;

            fixed (Complex* pa = aCopy.Memory.Span)
            fixed (double* ps = sCopy.Memory.Span)
            fixed (double* pwork = work)
            {
                var info = Lapack.Zgesvd(
                    MatrixLayout.ColMajor,
                    'N', 'N',
                    aCopy.RowCount, aCopy.ColCount,
                    pa, aCopy.Stride,
                    ps,
                    null, 1,
                    null, 1,
                    pwork);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The SVD computation did not converge.", nameof(Lapack.Zgesvd), (int)info);
                }
            }

            sCopy.CopyTo(s);
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
        /// <remarks>
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
        public static unsafe void Decompose(in Mat<Complex> a, in Vec<double> s, in Mat<Complex> u, in Mat<Complex> vt)
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
            using var aBuffer = MemoryPool<Complex>.Shared.Rent(aLength);
            var aCopy = new Mat<Complex>(a.RowCount, a.ColCount, a.RowCount, aBuffer.Memory.Slice(0, aLength));
            a.CopyTo(aCopy);

            var sLength = Math.Min(aCopy.RowCount, aCopy.ColCount);
            using var sBuffer = MemoryPool<double>.Shared.Rent(sLength);
            var sCopy = new Vec<double>(sLength, 1, sBuffer.Memory.Slice(0, sLength));

            using var workBuffer = MemoryPool<double>.Shared.Rent(Math.Min(aCopy.RowCount, aCopy.ColCount) - 1);
            var work = workBuffer.Memory.Span;

            fixed (Complex* pa = aCopy.Memory.Span)
            fixed (double* ps = sCopy.Memory.Span)
            fixed (Complex* pu = u.Memory.Span)
            fixed (Complex* pvt = vt.Memory.Span)
            fixed (double* pwork = work)
            {
                var info = Lapack.Zgesvd(
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
                    throw new LapackException("The SVD computation did not converge.", nameof(Lapack.Zgesvd), (int)info);
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
        public ref readonly Mat<Complex> U => ref u;

        /// <summary>
        /// The matrix V^T.
        /// </summary>
        public ref readonly Mat<Complex> VT => ref vt;
    }
}
