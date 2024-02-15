using System;
using System.Buffers;
using System.Numerics;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the LU decomposition.
    /// </summary>
    public class LuDecompositionComplex : MatrixDecompositionBase<Complex>
    {
        private Mat<Complex> l;
        private Mat<Complex> u;
        private int[] pivot;
        private int[] permutation;

        /// <summary>
        /// Decomposes the matrix using LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <exception cref="LapackException">
        /// The matrix is ill-conditioned.
        /// </exception>
        public LuDecompositionComplex(in Mat<Complex> a) : base(a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var min = Math.Min(a.RowCount, a.ColCount);

            var l = new Mat<Complex>(a.RowCount, min);
            var u = new Mat<Complex>(min, a.ColCount);
            var pivot = new int[min];

            Decompose(a, l, u, pivot);

            this.l = l;
            this.u = u;
            this.pivot = pivot;

            permutation = new int[a.RowCount];
            for (var i = 0; i < a.RowCount; i++)
            {
                permutation[i] = i;
            }
            for (var i = 0; i < min; i++)
            {
                var j = pivot[i];
                (permutation[i], permutation[j]) = (permutation[j], permutation[i]);
            }
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
        /// <param name="piv">
        /// The destination of the pivot info.
        /// </param>
        /// <exception cref="LapackException">
        /// The matrix is ill-conditioned.
        /// </exception>
        public unsafe static void Decompose(in Mat<Complex> a, in Mat<Complex> l, in Mat<Complex> u, Span<int> piv)
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

            if (piv.Length != min)
            {
                throw new ArgumentException("'piv.Length' must match 'min(a.RowCount, a.ColCount)'");
            }

            using var utmp = TemporalMatrix.CopyFrom(a);
            ref readonly var tmp = ref utmp.Item;

            fixed (Complex* ptmp = tmp.Memory.Span)
            fixed (int* ppiv = piv)
            {
                var info = Lapack.Zgetrf(
                    MatrixLayout.ColMajor,
                    tmp.RowCount, tmp.ColCount,
                    ptmp, tmp.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Zgetrf), (int)info);
                }
            }

            ExtractL(tmp, l);
            ExtractU(tmp, u);

            for (var i = 0; i < piv.Length; i++)
            {
                piv[i] -= 1;
            }
        }

        /// <inheritdoc/>
        public unsafe override void Solve(in Vec<Complex> b, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowIfInvalidSize(b, destination);

            if (l.RowCount != u.ColCount)
            {
                throw new InvalidOperationException("Calling this method against a non-square LU decomposition is not allowed.");
            }

            for (var i = 0; i < destination.Count; i++)
            {
                destination[i] = b[permutation[i]];
            }

            fixed (Complex* pl = l.Memory.Span)
            fixed (Complex* pu = u.Memory.Span)
            fixed (Complex* pd = destination.Memory.Span)
            {
                Blas.Ztrsv(
                    Order.ColMajor,
                    Uplo.Lower,
                    Transpose.NoTrans,
                    Diag.Unit,
                    l.RowCount,
                    pl, l.Stride,
                    pd, destination.Stride);

                Blas.Ztrsv(
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
        public Mat<Complex> GetPermutationMatrix()
        {
            var p = new Mat<Complex>(permutation.Length, permutation.Length);
            for (var i = 0; i < permutation.Length; i++)
            {
                p[permutation[i], i] = 1;
            }

            return p;
        }

        /// <summary>
        /// Computes the determinant of the source matrix.
        /// </summary>
        /// <returns>
        /// The determinant of the source matrix.
        /// </returns>
        public Complex Determinant()
        {
            var determinant = Complex.One;
            for (var i = 0; i < pivot.Length; i++)
            {
                if (pivot[i] == i)
                {
                    determinant *= u[i, i];
                }
                else
                {
                    determinant *= -u[i, i];
                }
            }
            return determinant;
        }

        private static void ExtractL(in Mat<Complex> source, in Mat<Complex> l)
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

        private static void ExtractU(in Mat<Complex> source, in Mat<Complex> u)
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
        public ref readonly Mat<Complex> L => ref l;

        /// <summary>
        /// The matrix U.
        /// </summary>
        public ref readonly Mat<Complex> U => ref u;

        /// <summary>
        /// The permutation of rows.
        /// </summary>
        public ReadOnlySpan<int> Permutation => permutation;
    }
}
