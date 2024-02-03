using System;
using System.Buffers;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the LU decomposition.
    /// </summary>
    public class LuDouble
    {
        private Mat<double> l;
        private Mat<double> u;
        private int[] permutation;

        /// <summary>
        /// Decomposes the matrix A using LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix A to be decomposed.
        /// </param>
        public LuDouble(in Mat<double> a)
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
        /// Decomposes the matrix A using LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix A to be decomposed.
        /// </param>
        /// <param name="l">
        /// The destination of the the matrix L.
        /// </param>
        /// <param name="u">
        /// The destination of the the matrix U.
        /// </param>
        /// <param name="permutation">
        /// The permutation of the rows.
        /// </param>
        /// <exception cref="LapackException">
        /// The matrix is ill-conditioned.
        /// </exception>
        public static unsafe void Decompose(in Mat<double> a, in Mat<double> l, in Mat<double> u, Span<int> permutation)
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
                throw new ArgumentException("'pivot.Length' must match 'a.RowCount'.");
            }

            var tmpLength = a.RowCount * a.ColCount;
            using var tmpBuffer = MemoryPool<double>.Shared.Rent(tmpLength);
            var tmp = new Mat<double>(a.RowCount, a.ColCount, a.RowCount, tmpBuffer.Memory.Slice(0, tmpLength));
            a.CopyTo(tmp);

            var pivLength = min;
            using var pivBuffer = MemoryPool<int>.Shared.Rent(pivLength);
            var piv = pivBuffer.Memory.Span;

            fixed (double* ptmp = tmp.Memory.Span)
            fixed (int* ppiv = piv)
            {
                var info = Lapack.Dgetrf(
                    MatrixLayout.ColMajor,
                    tmp.RowCount, tmp.ColCount,
                    ptmp, tmp.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Dgetrf), (int)info);
                }
            }

            for (var i = 0; i < min; i++)
            {
                var lCol = l.Cols[i];
                var uRow = u.Rows[i];
                tmp.Cols[i].CopyTo(lCol);
                tmp.Rows[i].CopyTo(uRow);
                for (var j = 0; j < i; j++)
                {
                    lCol[j] = 0;
                    uRow[j] = 0;
                }
                lCol[i] = 1;
            }

            for (var i = 0; i < a.RowCount; i++)
            {
                permutation[i] = i;
            }
            for (var i = 0; i < min; i++)
            {
                var j = piv[i] - 1;
                (permutation[i], permutation[j]) = (permutation[j], permutation[i]);
            }
        }

        /// <summary>
        /// Creates a permutation matrix from the permutation info.
        /// </summary>
        /// <param name="permutation">
        /// The permutation info obtained from a LU decomposition.
        /// </param>
        /// <returns>
        /// The permutation matrix.
        /// </returns>
        public static Mat<double> GetP(ReadOnlySpan<int> permutation)
        {
            if (permutation.Length == 0)
            {
                throw new ArgumentException("The length of the permutation info must be greater than zero.");
            }

            var p = new Mat<double>(permutation.Length, permutation.Length);
            for (var i = 0; i < permutation.Length; i++)
            {
                p[permutation[i], i] = 1;
            }
            return p;
        }

        /// <summary>
        /// Creates the permutation matrix.
        /// </summary>
        /// <returns>
        /// The permutation matrix.
        /// </returns>
        public Mat<double> GetP()
        {
            return GetP(permutation);
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
        /// The permutation info.
        /// </summary>
        public ReadOnlySpan<int> Permutation => permutation;
    }
}
