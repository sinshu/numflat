using System;
using System.Buffers;
using OpenBlasSharp;

namespace NumFlat
{
    public class SingularValueDecompositionDouble
    {
        public static unsafe void Decompose(Mat<double> a, Vec<double> s, Mat<double> u, Mat<double> vt)
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
            var aBuffer = ArrayPool<double>.Shared.Rent(aLength);
            var sLength = Math.Min(a.RowCount, a.ColCount);
            var sBuffer = ArrayPool<double>.Shared.Rent(sLength);
            var work = ArrayPool<double>.Shared.Rent(Math.Min(a.RowCount, a.ColCount) - 1);
            try
            {
                var aCopy = new Mat<double>(a.RowCount, a.ColCount, a.RowCount, ((Memory<double>)aBuffer).Slice(0, aLength));
                a.CopyTo(aCopy);
                var sCopy = new Vec<double>(sLength, 1, ((Memory<double>)sBuffer).Slice(0, sLength));
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
            finally
            {
                ArrayPool<double>.Shared.Return(aBuffer);
                ArrayPool<double>.Shared.Return(sBuffer);
                ArrayPool<double>.Shared.Return(work);
            }
        }
    }
}
