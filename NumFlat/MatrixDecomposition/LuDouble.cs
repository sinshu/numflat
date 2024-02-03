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
        private Mat<double> lapackDecomposed;
        private int[] lapackPiv;

        /// <summary>
        /// Decomposes the matrix A using LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix A to be decomposed.
        /// </param>
        public unsafe LuDouble(in Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            this.lapackDecomposed = new Mat<double>(a.RowCount, a.ColCount);
            this.lapackPiv = new int[Math.Min(a.RowCount, a.ColCount)];

            a.CopyTo(this.lapackDecomposed);

            fixed (double* pd = lapackDecomposed.Memory.Span)
            fixed (int* ppiv = lapackPiv)
            {
                var info = Lapack.Dgetrf(
                    MatrixLayout.ColMajor,
                    lapackDecomposed.RowCount, lapackDecomposed.ColCount,
                    pd, lapackDecomposed.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Dgetrf), (int)info);
                }
            }
        }

        /// <summary>
        /// Gets the matrix L.
        /// </summary>
        /// <returns>
        /// The matrix L.
        /// </returns>
        public Mat<double> GetL()
        {
            var min = Math.Min(lapackDecomposed.RowCount, lapackDecomposed.ColCount);

            var l = new Mat<double>(lapackDecomposed.RowCount, min);
            for (var i = 0; i < min; i++)
            {
                var lCol = l.Cols[i];
                lapackDecomposed.Cols[i].CopyTo(lCol);
                for (var j = 0; j < i; j++)
                {
                    lCol[j] = 0;
                }
                lCol[i] = 1;
            }

            return l;
        }

        /// <summary>
        /// Gets the matrix U.
        /// </summary>
        /// <returns>
        /// The matrix U.
        /// </returns>
        public Mat<double> GetU()
        {
            var min = Math.Min(lapackDecomposed.RowCount, lapackDecomposed.ColCount);

            var u = new Mat<double>(min, lapackDecomposed.ColCount);
            for (var i = 0; i < min; i++)
            {
                var uRow = u.Rows[i];
                lapackDecomposed.Rows[i].CopyTo(uRow);
                for (var j = 0; j < i; j++)
                {
                    uRow[j] = 0;
                }
            }

            return u;
        }

        /// <summary>
        /// Gets the permutation info.
        /// </summary>
        /// <returns>
        /// The permutation info.
        /// </returns>
        public int[] GetPermutation()
        {
            var permutation = new int[lapackDecomposed.RowCount];
            for (var i = 0; i < lapackDecomposed.RowCount; i++)
            {
                permutation[i] = i;
            }

            var min = Math.Min(lapackDecomposed.RowCount, lapackDecomposed.ColCount);
            for (var i = 0; i < min; i++)
            {
                var j = lapackPiv[i] - 1;
                (permutation[i], permutation[j]) = (permutation[j], permutation[i]);
            }

            return permutation;
        }

        /// <summary>
        /// Gets the permutation matrix.
        /// </summary>
        /// <returns>
        /// The permutation matrix.
        /// </returns>
        public Mat<double> GetP()
        {
            var permutation = GetPermutation();

            var p = new Mat<double>(permutation.Length, permutation.Length);
            for (var i = 0; i < permutation.Length; i++)
            {
                p[permutation[i], i] = 1;
            }

            return p;
        }
    }
}
