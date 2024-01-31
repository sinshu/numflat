using System;
using System.Buffers;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides singular value decomposition (SVD).
    /// </summary>
    public class SingularValueDecompositionSingle
    {
        private Vec<float> s;
        private Mat<float> u;
        private Mat<float> vt;

        /// <summary>
        /// Decomposes the matrix A with SVD.
        /// </summary>
        /// <param name="a">
        /// The matrix A to be decomposed.
        /// </param>
        public SingularValueDecompositionSingle(Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(ref a, nameof(a));

            var s = new Vec<float>(Math.Min(a.RowCount, a.ColCount));
            var u = new Mat<float>(a.RowCount, a.RowCount);
            var vt = new Mat<float>(a.ColCount, a.ColCount);
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
        public static unsafe void Decompose(Mat<float> a, Vec<float> s, Mat<float> u, Mat<float> vt)
        {
            ThrowHelper.ThrowIfEmpty(ref a, nameof(a));
            ThrowHelper.ThrowIfEmpty(ref s, nameof(s));
            ThrowHelper.ThrowIfEmpty(ref u, nameof(u));
            ThrowHelper.ThrowIfEmpty(ref vt, nameof(vt));

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
            //   s: Write buffer for 's' must be contiguous. Given Vec<double> might not be so.
            //      Creating Vec<double> for 's' with stride = 1 to ensure contiguity.
            //

            var aLength = a.RowCount * a.ColCount;
            var aBuffer = ArrayPool<float>.Shared.Rent(aLength);
            var sLength = Math.Min(a.RowCount, a.ColCount);
            var sBuffer = ArrayPool<float>.Shared.Rent(sLength);
            var work = ArrayPool<float>.Shared.Rent(Math.Min(a.RowCount, a.ColCount) - 1);
            try
            {
                var aCopy = new Mat<float>(a.RowCount, a.ColCount, a.RowCount, ((Memory<float>)aBuffer).Slice(0, aLength));
                a.CopyTo(aCopy);
                var sCopy = new Vec<float>(sLength, 1, ((Memory<float>)sBuffer).Slice(0, sLength));
                fixed (float* pa = aCopy.Memory.Span)
                fixed (float* ps = sCopy.Memory.Span)
                fixed (float* pu = u.Memory.Span)
                fixed (float* pvt = vt.Memory.Span)
                fixed (float* pwork = work)
                {
                    var info = Lapack.Sgesvd(
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
                        throw new LapackException("The SVD computation did not converge.", nameof(Lapack.Sgesvd), (int)info);
                    }
                }
                sCopy.CopyTo(s);
            }
            finally
            {
                ArrayPool<float>.Shared.Return(aBuffer);
                ArrayPool<float>.Shared.Return(sBuffer);
                ArrayPool<float>.Shared.Return(work);
            }
        }

        /// <summary>
        /// The diagonal elements of the matrix S.
        /// </summary>
        public Vec<float> S => s;

        /// <summary>
        /// The matrix U.
        /// </summary>
        public Mat<float> U => u;

        /// <summary>
        /// The matrix V^T.
        /// </summary>
        public Mat<float> VT => vt;
    }
}
