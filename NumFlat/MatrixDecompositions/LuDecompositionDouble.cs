using System;
using System.Buffers;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the LU decomposition.
    /// </summary>
    public sealed class LuDecompositionDouble : MatrixDecompositionBase<double>
    {
        private Mat<double> l;
        private Mat<double> u;
        private int[] permutation;
        private int pivotSign;

        /// <summary>
        /// Decomposes the matrix using LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <exception cref="LapackException">
        /// The matrix is ill-conditioned.
        /// </exception>
        public LuDecompositionDouble(in Mat<double> a) : base(a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var min = Math.Min(a.RowCount, a.ColCount);

            var l = new Mat<double>(a.RowCount, min);
            var u = new Mat<double>(min, a.ColCount);
            var permutation = new int[a.RowCount];

            Decompose(a, l, u, permutation);

            this.l = l;
            this.u = u;
            this.permutation = permutation;
        }

        /// <summary>
        /// Decomposes the matrix using LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <param name="l">
        /// The destination of the matrix L.
        /// </param>
        /// <param name="u">
        /// The destination of the matrix U.
        /// </param>
        /// <param name="permutation">
        /// The destination of the permutation info.
        /// </param>
        /// <returns>
        /// The pivot sign.
        /// </returns>
        /// <exception cref="LapackException">
        /// The matrix is ill-conditioned.
        /// </exception>
        public unsafe static int Decompose(in Mat<double> a, in Mat<double> l, in Mat<double> u, Span<int> permutation)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(l, nameof(l));
            ThrowHelper.ThrowIfEmpty(u, nameof(u));

            var min = Math.Min(a.RowCount, a.ColCount);

            if (l.RowCount != a.RowCount)
            {
                throw new ArgumentException("'l.RowCount' must match 'a.RowCount'.");
            }

            if (l.ColCount != min)
            {
                throw new ArgumentException("'l.ColCount' must match 'min(a.RowCount, a.ColCount)'.");
            }

            if (u.RowCount != min)
            {
                throw new ArgumentException("'u.RowCount' must match 'min(a.RowCount, a.ColCount)'.");
            }

            if (u.ColCount != a.ColCount)
            {
                throw new ArgumentException("'u.ColCount' must match 'a.ColCount'.");
            }

            if (permutation.Length != a.RowCount)
            {
                throw new ArgumentException("'permutation.Length' must match 'a.RowCount'.");
            }

            using var utmp = TemporalMatrix.CopyFrom(a);
            ref readonly var tmp = ref utmp.Item;

            var sign = 0;
            fixed (double* ptmp = tmp.Memory.Span)
            fixed (int* pprm = permutation)
            {
                MatFlat.Factorization.Lu(tmp.RowCount, tmp.ColCount, ptmp, tmp.Stride, pprm, &sign);
            }

            ExtractL(tmp, l);
            ExtractU(tmp, u);

            return sign;
        }

        /// <inheritdoc/>
        public unsafe override void Solve(in Vec<double> b, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowIfInvalidSize(b, destination);

            if (l.RowCount != u.ColCount)
            {
                throw new InvalidOperationException("Calling this method against a non-square LU decomposition is not allowed.");
            }

            var fd = destination.GetUnsafeFastIndexer();
            var fb = b.GetUnsafeFastIndexer();
            for (var i = 0; i < destination.Count; i++)
            {
                fd[i] = fb[permutation[i]];
            }

            fixed (double* pl = l.Memory.Span)
            fixed (double* pu = u.Memory.Span)
            fixed (double* pd = destination.Memory.Span)
            {
                Blas.Dtrsv(
                    Order.ColMajor,
                    Uplo.Lower,
                    Transpose.NoTrans,
                    Diag.Unit,
                    l.RowCount,
                    pl, l.Stride,
                    pd, destination.Stride);

                Blas.Dtrsv(
                    Order.ColMajor,
                    Uplo.Upper,
                    Transpose.NoTrans,
                    Diag.NonUnit,
                    u.RowCount,
                    pu, u.Stride,
                    pd, destination.Stride);
            }
        }

        /// <summary>
        /// Gets the permutation matrix.
        /// </summary>
        /// <returns>
        /// The permutation matrix.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix.
        /// </remarks>
        public Mat<double> GetPermutationMatrix()
        {
            var p = new Mat<double>(permutation.Length, permutation.Length);
            var fp = p.GetUnsafeFastIndexer();
            for (var i = 0; i < permutation.Length; i++)
            {
                fp[permutation[i], i] = 1;
            }

            return p;
        }

        /// <summary>
        /// Computes the determinant of the source matrix.
        /// </summary>
        /// <returns>
        /// The determinant of the source matrix.
        /// </returns>
        public double Determinant()
        {
            var fu = u.GetUnsafeFastIndexer();
            var determinant = 1.0;
            for (var i = 0; i < u.RowCount; i++)
            {
                determinant *= fu[i, i];
            }
            return pivotSign * determinant;
        }

        private static void ExtractL(in Mat<double> source, in Mat<double> l)
        {
            var min = Math.Min(source.RowCount, source.ColCount);

            var sCols = source.Cols;
            var lCols = l.Cols;

            for (var i = 0; i < min; i++)
            {
                var sCol = sCols[i];
                var lCol = lCols[i];

                var zeroLength = i + 1;
                var copyLength = l.RowCount - i - 1;

                if (zeroLength > 0)
                {
                    lCol.Subvector(0, zeroLength).Clear();
                }
                if (copyLength > 0)
                {
                    sCol.Subvector(zeroLength, copyLength).CopyTo(lCol.Subvector(zeroLength, copyLength));
                }

                lCol[i] = 1;
            }
        }

        private static void ExtractU(in Mat<double> source, in Mat<double> u)
        {
            var min = Math.Min(source.RowCount, source.ColCount);

            var sRows = source.Rows;
            var uRows = u.Rows;

            for (var i = 0; i < min; i++)
            {
                var sRow = sRows[i];
                var uRow = uRows[i];

                var zeroLength = i;
                var copyLength = u.ColCount - i;

                if (zeroLength > 0)
                {
                    uRow.Subvector(0, zeroLength).Clear();
                }
                if (copyLength > 0)
                {
                    sRow.Subvector(zeroLength, copyLength).CopyTo(uRow.Subvector(zeroLength, copyLength));
                }
            }
        }

        /// <summary>
        /// The matrix L.
        /// </summary>
        public ref readonly Mat<double> L => ref l;

        /// <summary>
        /// The matrix U.
        /// </summary>
        public ref readonly Mat<double> U => ref u;

        /// <summary>
        /// The permutation of rows.
        /// </summary>
        public ReadOnlySpan<int> Permutation => permutation;
    }
}
