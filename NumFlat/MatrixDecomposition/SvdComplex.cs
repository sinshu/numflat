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
        /// Decomposes the matrix A using SVD.
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
        /// The matrix A to be decomposed.
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
            var sCopy = new Vec<double>(sBuffer.Memory.Slice(0, sLength));

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
        /// Decomposes the matrix A using SVD.
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
            var sCopy = new Vec<double>(sBuffer.Memory.Slice(0, sLength));

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

            var tmpLength = vt.RowCount;
            using var tmpBuffer = MemoryPool<Complex>.Shared.Rent(tmpLength);
            var tmp = new Vec<Complex>(tmpBuffer.Memory.Slice(0, tmpLength));
            tmp.Clear();

            var ts = tmp.Subvector(0, s.Count);
            Mat.Mul(u.Submatrix(0, 0, u.RowCount, s.Count), b, ts, true, true);
            PointwiseDiv(ts, s, ts);
            Mat.Mul(vt, tmp, destination, true, true);
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
        public Vec<Complex> Solve(in Vec<Complex> b)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            if (b.Count != u.RowCount)
            {
                throw new ArgumentException("'b.Count' must match 'U.RowCount'.");
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
