using System;
using System.Numerics;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the eigen value decomposition (EVD).
    /// </summary>
    public class EigenValueDecompositionComplex
    {
        private readonly Vec<double> d;
        private readonly Mat<Complex> v;

        /// <summary>
        /// Decomposes the matrix using EVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed to compute the EVD.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be Hermitian symmetric.
        /// Note that this implementation does not check if the input matrix is Hermitian symmetric.
        /// Specifically, only the lower triangular part of the input matrix is referenced, and the rest is ignored.
        /// </remarks>
        public EigenValueDecompositionComplex(in Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            if (a.RowCount != a.ColCount)
            {
                throw new ArgumentException("The matrix must be a square matrix.");
            }

            var d = new Vec<double>(a.RowCount);
            var v = new Mat<Complex>(a.RowCount, a.RowCount);
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
        /// <remarks>
        /// The matrix to be decomposed must be Hermitian symmetric.
        /// Note that this implementation does not check if the input matrix is Hermitian symmetric.
        /// Specifically, only the lower triangular part of the input matrix is referenced, and the rest is ignored.
        /// </remarks>
        public static unsafe void Decompose(in Mat<Complex> a, in Vec<double> d, in Mat<Complex> v)
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

            fixed (Complex* pv = v.Memory.Span)
            fixed (double* pcd = cd.Memory.Span)
            {
                var info = Lapack.Zheev(
                    MatrixLayout.ColMajor,
                    'V',
                    'L',
                    v.RowCount,
                    pv, v.Stride,
                    pcd);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("Failed to compute the EVD.", nameof(Lapack.Zheev), (int)info);
                }
            }
        }

        /// <summary>
        /// Solves the linear equation, Ax = b.
        /// </summary>
        /// <param name="b">
        /// The input vector.
        /// </param>
        /// <param name="destination">
        /// The destination of the solution vector.
        /// </param>
        public unsafe void Solve(in Vec<Complex> b, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (b.Count != v.RowCount)
            {
                throw new ArgumentException("'b.Count' must match the order of V.");
            }

            if (destination.Count != v.RowCount)
            {
                throw new ArgumentException("'destination.Count' must match the order of V.");
            }

            using var utmp = new TemporalVector<Complex>(v.RowCount);
            ref readonly var tmp = ref utmp.Item;

            Mat.Mul(v, b, tmp, true, true);
            PointwiseDiv(tmp, d, tmp);
            Mat.Mul(v, tmp, destination, false, false);
        }

        /// <summary>
        /// Solves the linear equation, Ax = b.
        /// </summary>
        /// <param name="b">
        /// The input vector.
        /// </param>
        /// <returns>
        /// The solution vector.
        /// </returns>
        public Vec<Complex> Solve(in Vec<Complex> b)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            if (b.Count != v.RowCount)
            {
                throw new ArgumentException("'b.Count' must match the order of V.");
            }

            var x = new Vec<Complex>(v.RowCount);
            Solve(b, x);
            return x;
        }

        private static void PointwiseDiv(in Vec<Complex> x, in Vec<double> y, in Vec<Complex> destination)
        {
            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var py = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px] / sy[py];
                px += x.Stride;
                py += y.Stride;
                pd += destination.Stride;
            }
        }

        /// <summary>
        /// The diagonal elements of the matrix D.
        /// </summary>
        public ref readonly Vec<double> D => ref d;

        /// <summary>
        /// The matrix V.
        /// </summary>
        public ref readonly Mat<Complex> V => ref v;
    }
}
