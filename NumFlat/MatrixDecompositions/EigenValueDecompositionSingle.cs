using System;
using OpenBlasSharp;

namespace NumFlat
{
    /// <summary>
    /// Provides the eigen value decomposition (EVD).
    /// </summary>
    public class EigenValueDecompositionSingle
    {
        private readonly Vec<float> d;
        private readonly Mat<float> v;

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
        public EigenValueDecompositionSingle(in Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            if (a.RowCount != a.ColCount)
            {
                throw new ArgumentException("The matrix must be a square matrix.");
            }

            var d = new Vec<float>(a.RowCount);
            var v = new Mat<float>(a.RowCount, a.RowCount);
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
        public static unsafe void Decompose(in Mat<float> a, in Vec<float> d, in Mat<float> v)
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

            fixed (float* pv = v.Memory.Span)
            fixed (float* pcd = cd.Memory.Span)
            {
                var info = Lapack.Ssyev(
                    MatrixLayout.ColMajor,
                    'V',
                    'L',
                    v.RowCount,
                    pv, v.Stride,
                    pcd);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("Failed to compute the EVD.", nameof(Lapack.Ssyev), (int)info);
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
        public unsafe void Solve(in Vec<float> b, in Vec<float> destination)
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

            using var utmp = new TemporalVector<float>(v.RowCount);
            ref readonly var tmp = ref utmp.Item;

            Mat.Mul(v, b, tmp, true);
            Vec.PointwiseDiv(tmp, d, tmp);
            Mat.Mul(v, tmp, destination, false);
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
        public Vec<float> Solve(in Vec<float> b)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            if (b.Count != v.RowCount)
            {
                throw new ArgumentException("'b.Count' must match the order of V.");
            }

            var x = new Vec<float>(v.RowCount);
            Solve(b, x);
            return x;
        }

        /// <summary>
        /// Computes the determinant of the source matrix.
        /// </summary>
        /// <returns>
        /// The determinant of the source matrix.
        /// </returns>
        public float Determinant()
        {
            var determinant = 1.0F;
            foreach (var value in d)
            {
                determinant *= value;
            }
            return determinant;
        }

        /// <summary>
        /// Computes the log determinant of the source matrix.
        /// </summary>
        /// <returns>
        /// The log determinant of the source matrix.
        /// </returns>
        public float LogDeterminant()
        {
            var logDeterminant = 0.0F;
            foreach (var value in d)
            {
                logDeterminant += MathF.Log(value);
            }
            return logDeterminant;
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
