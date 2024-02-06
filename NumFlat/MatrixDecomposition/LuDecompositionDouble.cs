using System;
using System.Buffers;
using System.Linq;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the LU decomposition.
    /// </summary>
    public class LuDecompositionDouble
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
        /// <exception cref="LapackException">
        /// The matrix is ill-conditioned.
        /// </exception>
        public LuDecompositionDouble(in Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var min = Math.Min(a.RowCount, a.ColCount);

            var l = new Mat<double>(a.RowCount, min);
            var u = new Mat<double>(min, a.ColCount);

            using var tmp_piv = MemoryPool<int>.Shared.Rent(min);
            var piv = tmp_piv.Memory.Span.Slice(0, min);

            Decompose(a, l, u, piv);

            this.l = l;
            this.u = u;

            permutation = new int[a.RowCount];
            for (var i = 0; i < a.RowCount; i++)
            {
                permutation[i] = i;
            }
            for (var i = 0; i < min; i++)
            {
                var j = piv[i];
                (permutation[i], permutation[j]) = (permutation[j], permutation[i]);
            }
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
        /// <param name="piv">
        /// The destination of the the pivot info.
        /// </param>
        /// <exception cref="LapackException">
        /// The matrix is ill-conditioned.
        /// </exception>
        public unsafe static void Decompose(in Mat<double> a, in Mat<double> l, in Mat<double> u, Span<int> piv)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(l, nameof(l));
            ThrowHelper.ThrowIfEmpty(u, nameof(u));

            var min = Math.Min(a.RowCount, a.ColCount);

            if (l.RowCount != a.RowCount || l.ColCount != min)
            {
                throw new ArgumentException();
            }

            if (u.RowCount != min || u.ColCount != a.ColCount)
            {
                throw new ArgumentException();
            }

            if (piv.Length != min)
            {
                throw new ArgumentException();
            }

            using var tmp_aCopy = TemporalMatrix.CopyFrom(a);
            ref readonly var aCopy = ref tmp_aCopy.Item;

            fixed (double* pa = aCopy.Memory.Span)
            fixed (int* ppiv = piv)
            {
                var info = Lapack.Dgetrf(
                    MatrixLayout.ColMajor,
                    aCopy.RowCount, aCopy.ColCount,
                    pa, aCopy.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Dgetrf), (int)info);
                }
            }

            l.Clear();
            for (var i = 0; i < min; i++)
            {
                var lCol = l.Cols[i];
                aCopy.Cols[i].CopyTo(lCol);
                for (var j = 0; j < i; j++)
                {
                    lCol[j] = 0;
                }
                lCol[i] = 1;
            }

            u.Clear();
            for (var i = 0; i < min; i++)
            {
                var uRow = u.Rows[i];
                aCopy.Rows[i].CopyTo(uRow);
                for (var j = 0; j < i; j++)
                {
                    uRow[j] = 0;
                }
            }

            for (var i = 0; i < piv.Length; i++)
            {
                piv[i] -= 1;
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
        public unsafe void Solve(in Vec<double> b, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (l.RowCount != u.ColCount)
            {
                throw new InvalidOperationException("This method does not support non-square matrices.");
            }

            if (b.Count != l.RowCount)
            {
                throw new ArgumentException("'b.Count' must match 'a.RowCount'.");
            }

            if (destination.Count != l.RowCount)
            {
                throw new ArgumentException("'destination.Count' must match 'a.RowCount'.");
            }

            for (var i = 0; i < destination.Count; i++)
            {
                destination[i] = b[permutation[i]];
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
        /// Compute a vector x from b, where Ax = b.
        /// </summary>
        /// <param name="b">
        /// The vector b.
        /// </param>
        /// <returns>
        /// The vector x.
        /// </returns>
        public Vec<double> Solve(in Vec<double> b)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            if (l.RowCount != u.ColCount)
            {
                throw new InvalidOperationException("This method does not support non-square matrices.");
            }

            if (b.Count != l.RowCount)
            {
                throw new ArgumentException("'b.Count' must match 'a.RowCount'.");
            }

            var x = new Vec<double>(l.RowCount);
            Solve(b, x);
            return x;
        }

        /// <summary>
        /// Gets the permutation matrix.
        /// </summary>
        /// <returns>
        /// The permutation matrix.
        /// </returns>
        public Mat<double> GetP()
        {
            var p = new Mat<double>(permutation.Length, permutation.Length);
            for (var i = 0; i < permutation.Length; i++)
            {
                p[permutation[i], i] = 1;
            }

            return p;
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
