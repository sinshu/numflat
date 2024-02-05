using System;
using System.Buffers;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the LU decomposition.
    /// </summary>
    public class LuDecompositionDouble
    {
        private Mat<double> lapackDecomposed;
        private int[] lapackPiv;

        /// <summary>
        /// Decomposes the matrix A using LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix A to be decomposed.
        /// </param>
        /// <exception cref="LapackException">
        /// The matrix is ill-conditioned.
        /// </exception>
        public unsafe LuDecompositionDouble(in Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            lapackDecomposed = new Mat<double>(a.RowCount, a.ColCount);
            lapackPiv = new int[Math.Min(a.RowCount, a.ColCount)];

            a.CopyTo(lapackDecomposed);

            fixed (double* pld = lapackDecomposed.Memory.Span)
            fixed (int* ppiv = lapackPiv)
            {
                var info = Lapack.Dgetrf(
                    MatrixLayout.ColMajor,
                    lapackDecomposed.RowCount, lapackDecomposed.ColCount,
                    pld, lapackDecomposed.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Dgetrf), (int)info);
                }
            }
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
        public unsafe void Solve(in Vec<double> b, in Vec<double> destination)
        {
            if (lapackDecomposed.RowCount != lapackDecomposed.ColCount)
            {
                throw new InvalidOperationException("This method does not support non-square matrices.");
            }

            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (b.Count != lapackDecomposed.RowCount)
            {
                throw new ArgumentException("'b.Count' must match 'L.RowCount'.");
            }

            if (destination.Count != lapackDecomposed.RowCount)
            {
                throw new ArgumentException("'destination.Count' must match 'L.RowCount'.");
            }

            var tmpLength = lapackDecomposed.RowCount;
            using var tmpBuffer = MemoryPool<double>.Shared.Rent(tmpLength);
            var tmp = new Vec<double>(tmpBuffer.Memory.Slice(0, tmpLength));
            b.CopyTo(tmp);

            fixed (double* pld = lapackDecomposed.Memory.Span)
            fixed (int* ppiv = lapackPiv)
            fixed (double* ptmp = tmp.Memory.Span)
            {
                var info = Lapack.Dgetrs(
                    MatrixLayout.ColMajor,
                    'N',
                    lapackDecomposed.RowCount,
                    1,
                    pld, lapackDecomposed.Stride,
                    ppiv,
                    ptmp, tmpLength);

            }

            tmp.CopyTo(destination);
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
        public Vec<double> Solve(in Vec<double> b)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            if (lapackDecomposed.RowCount != lapackDecomposed.ColCount)
            {
                throw new InvalidOperationException("This method does not support non-square matrices.");
            }

            if (b.Count != lapackDecomposed.RowCount)
            {
                throw new ArgumentException("'b.Count' must match 'a.RowCount'.");
            }

            var x = new Vec<double>(lapackDecomposed.RowCount);
            Solve(b, x);
            return x;
        }

        /// <summary>
        /// Gets the matrix L.
        /// </summary>
        /// <returns>
        /// The matrix L.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix.
        /// </remarks>
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
        /// <remarks>
        /// This method allocates a new matrix.
        /// </remarks>
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
        /// <remarks>
        /// This method allocates a new matrix.
        /// </remarks>
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
