using System;
using MatFlat;

namespace NumFlat
{
    /// <summary>
    /// Provides the eigenvalue decomposition (EVD).
    /// </summary>
    public sealed class EigenValueDecompositionDouble : MatrixDecompositionBase<double>
    {
        private readonly Vec<double> d;
        private readonly Mat<double> v;

        /// <summary>
        /// Decomposes the matrix using EVD.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the EVD.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public EigenValueDecompositionDouble(in Mat<double> a) : base(a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfNonSquare(a, nameof(a));

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
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the EVD.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public static unsafe void Decompose(in Mat<double> a, in Vec<double> d, in Mat<double> v)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(d, nameof(d));
            ThrowHelper.ThrowIfEmpty(v, nameof(v));
            ThrowHelper.ThrowIfNonSquare(a, nameof(a));

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
                Factorization.Evd(v.RowCount, pv, v.Stride, pcd);
            }
        }

        /// <inheritdoc/>
        public unsafe override void Solve(in Vec<double> b, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowIfInvalidSize(b, destination);

            using var utmp = new TemporalVector<double>(v.RowCount);
            ref readonly var tmp = ref utmp.Item;

            Mat.Mul(v, b, tmp, true);
            Vec.PointwiseDiv(tmp, d, tmp);
            Mat.Mul(v, tmp, destination, false);
        }

        /// <summary>
        /// Computes the determinant of the source matrix.
        /// </summary>
        /// <returns>
        /// The determinant of the source matrix.
        /// </returns>
        public double Determinant()
        {
            var determinant = 1.0;
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
        public double LogDeterminant()
        {
            var logDeterminant = 0.0;
            foreach (var value in d)
            {
                logDeterminant += Math.Log(value);
            }
            return logDeterminant;
        }

        /// <summary>
        /// Gets the rank of the matrix.
        /// </summary>
        /// <param name="tolerance">
        /// Eigenvalues below this threshold will be treated as zero.
        /// </param>
        public int Rank(double tolerance)
        {
            // If tolerance is NaN, set the tolerance by the Math.NET's method.
            if (double.IsNaN(tolerance))
            {
                tolerance = Special.Eps(d[0]) * d.Count;
            }

            var rank = 0;
            foreach (var value in d)
            {
                if (value > tolerance)
                {
                    rank++;
                }
            }

            return rank;
        }

        /// <summary>
        /// Gets the rank of the matrix.
        /// </summary>
        public int Rank()
        {
            // Set NaN to tolerance to set the tolerance automatically.
            return Rank(double.NaN);
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
