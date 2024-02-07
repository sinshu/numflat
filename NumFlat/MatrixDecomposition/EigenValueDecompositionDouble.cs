using System;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the eigen value decomposition (EVD).
    /// </summary>
    public class EigenValueDecompositionDouble
    {
        private readonly Vec<double> d;
        private readonly Mat<double> v;

        /// <summary>
        /// Decomposes the matrix using EVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        public EigenValueDecompositionDouble(in Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            if (a.RowCount != a.ColCount)
            {
                throw new ArgumentException("The matrix must be a square matrix.");
            }

            var d = new Vec<double>(a.RowCount);
            var v = new Mat<double>(a.RowCount, a.RowCount);
            Decompose(a, d, v);

            this.d = d;
            this.v = v;
        }

        /// <summary>
        /// Decomposes the matrix using EVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <param name="d">
        /// The destination of the diagonal elements of the matrix D.
        /// </param>
        /// <param name="v">
        /// The destination of the the matrix V.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed to compute the EVD.
        /// </exception>
        public static unsafe void Decompose(in Mat<double> a, in Vec<double> d, in Mat<double> v)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(d, nameof(d));
            ThrowHelper.ThrowIfEmpty(v, nameof(v));

            if (a.RowCount != a.ColCount)
            {
                throw new ArgumentException("'a' must be a square matrix.");
            }

            if (d.Count != a.RowCount)
            {
                throw new ArgumentException("'d.Count' must match 'a.RowCount'.");
            }

            if (v.RowCount != a.RowCount || v.ColCount != a.ColCount)
            {
                throw new ArgumentException("The order of 'v' must match 'a'.");
            }

            using var ucd = d.EnsureContiguous(false);
            ref readonly var cd = ref ucd.Item;

            a.CopyTo(v);

            fixed (double* pv = v.Memory.Span)
            fixed (double* pcd = cd.Memory.Span)
            {
                var info = Lapack.Dsyev(
                    MatrixLayout.ColMajor,
                    'V',
                    'L',
                    v.RowCount,
                    pv, v.Stride,
                    pcd);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("Failed to compute the EVD.", nameof(Lapack.Dsyev), (int)info);
                }
            }
        }

        /// <summary>
        /// The diagonal elements of the matrix D.
        /// </summary>
        public ref readonly Vec<double> D => ref d;

        /// <summary>
        /// The matrix V.
        /// </summary>
        public ref readonly Mat<double> V => ref v;
    }
}
