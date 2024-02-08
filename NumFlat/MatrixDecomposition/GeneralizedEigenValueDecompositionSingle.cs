using System;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the generalized eigen value decomposition (GEVD).
    /// </summary>
    public class GeneralizedEigenValueDecompositionSingle
    {
        private readonly Vec<float> d;
        private readonly Mat<float> v;

        /// <summary>
        /// Decomposes the matrix using GEVD.
        /// </summary>
        /// <param name="a">
        /// The source matrix A.
        /// </param>
        /// <param name="b">
        /// The source matrix B.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed to compute the GEVD.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be Hermitian symmetric.
        /// Note that this implementation does not check if the input matrix is Hermitian symmetric.
        /// Specifically, only the lower triangular part of the input matrix is referenced, and the rest is ignored.
        /// </remarks>
        public GeneralizedEigenValueDecompositionSingle(in Mat<float> a, in Mat<float> b)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            if (a.RowCount != a.ColCount)
            {
                throw new ArgumentException("'a' must be a square matrix.");
            }

            if (b.RowCount != a.RowCount || b.ColCount != a.ColCount)
            {
                throw new ArgumentException("The order of 'b' must match 'a'.");
            }

            var d = new Vec<float>(a.RowCount);
            var v = new Mat<float>(a.RowCount, a.ColCount);
            Decompose(a, b, d, v);

            this.d = d;
            this.v = v;
        }

        /// <summary>
        /// Decomposes the matrix using GEVD.
        /// </summary>
        /// <param name="a">
        /// The source matrix A.
        /// </param>
        /// <param name="b">
        /// The source matrix B.
        /// </param>
        /// <param name="d">
        /// The destination of the diagonal elements of the matrix D.
        /// </param>
        /// <param name="v">
        /// The destination of the the matrix V.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed to compute the GEVD.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be Hermitian symmetric.
        /// Note that this implementation does not check if the input matrix is Hermitian symmetric.
        /// Specifically, only the lower triangular part of the input matrix is referenced, and the rest is ignored.
        /// </remarks>
        public static unsafe void Decompose(in Mat<float> a, in Mat<float> b, in Vec<float> d, in Mat<float> v)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(d, nameof(d));
            ThrowHelper.ThrowIfEmpty(v, nameof(v));

            if (a.RowCount != a.ColCount)
            {
                throw new ArgumentException("'a' must be a square matrix.");
            }

            if (b.RowCount != a.RowCount || b.ColCount != a.ColCount)
            {
                throw new ArgumentException("The order of 'b' must match 'a'.");
            }

            if (d.Count != a.RowCount)
            {
                throw new ArgumentException("'d.Count' must match 'a.RowCount'.");
            }

            if (v.RowCount != a.RowCount || v.ColCount != a.ColCount)
            {
                throw new ArgumentException("The order of 'v' must match 'a'.");
            }

            using var utmp = TemporalMatrix.CopyFrom(b);
            ref readonly var tmp = ref utmp.Item;

            using var ucd = d.EnsureContiguous(false);
            ref readonly var cd = ref ucd.Item;

            a.CopyTo(v);

            fixed (float* pv = v.Memory.Span)
            fixed (float* ptmp = tmp.Memory.Span)
            fixed (float* pcd = cd.Memory.Span)
            {
                var info = Lapack.Ssygv(
                    MatrixLayout.ColMajor,
                    1,
                    'V',
                    'L',
                    v.RowCount,
                    pv, v.Stride,
                    ptmp, tmp.Stride,
                    pcd);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("Failed to compute the GEVD.", nameof(Lapack.Ssygv), (int)info);
                }
            }
        }

        /// <summary>
        /// The diagonal elements of the matrix D.
        /// </summary>
        public ref readonly Vec<float> D => ref d;

        /// <summary>
        /// The matrix V.
        /// </summary>
        public ref readonly Mat<float> V => ref v;
    }
}
