using System;
using MatFlat;

namespace NumFlat
{
    /// <summary>
    /// Provides the Cholesky decomposition.
    /// </summary>
    public sealed class CholeskyDecompositionDouble : MatrixDecompositionBase<double>
    {
        private readonly Mat<double> l;

        /// <summary>
        /// Decomposes the matrix using Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <exception cref="MatrixFactorizationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public CholeskyDecompositionDouble(in Mat<double> a) : base(a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfNonSquare(a, nameof(a));

            var l = new Mat<double>(a.RowCount, a.ColCount);
            Decompose(a, l);

            this.l = l;
        }

        /// <summary>
        /// Decomposes the matrix using Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <param name="l">
        /// The destination of the the matrix L.
        /// </param>
        /// <exception cref="MatrixFactorizationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public static unsafe void Decompose(in Mat<double> a, in Mat<double> l)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(l, nameof(l));
            ThrowHelper.ThrowIfNonSquare(a, nameof(a));
            ThrowHelper.ThrowIfDifferentSize(a, l);

            a.CopyTo(l);

            fixed (double* pl = l.Memory.Span)
            {
                try
                {
                    Factorization.Cholesky(l.RowCount, pl, l.Stride);
                }
                catch (MatFlat.MatrixFactorizationException)
                {
                    throw new MatrixFactorizationException("Cholesky decomposition failed. The matrix must be positive definite.");
                }
            }
        }

        /// <inheritdoc/>
        public unsafe override void Solve(in Vec<double> b, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowIfInvalidSize(b, destination);

            Solve(l, b, destination);
        }

        internal static unsafe void Solve(in Mat<double> l, in Vec<double> b, in Vec<double> destination)
        {
            b.CopyTo(destination);

            fixed (double* pl = l.Memory.Span)
            fixed (double* pd = destination.Memory.Span)
            {
                Blas.SolveTriangular(
                    Uplo.Lower,
                    Transpose.NoTrans,
                    l.RowCount,
                    pl, l.Stride,
                    pd, destination.Stride);

                Blas.SolveTriangular(
                    Uplo.Lower,
                    Transpose.Trans,
                    l.RowCount,
                    pl, l.Stride,
                    pd, destination.Stride);
            }
        }

        /// <summary>
        /// Computes the determinant of the source matrix.
        /// </summary>
        /// <returns>
        /// The determinant of the source matrix.
        /// </returns>
        public double Determinant()
        {
            var acc = 1.0;
            foreach (var value in l.EnumerateDiagonalElements())
            {
                acc *= value;
            }
            return acc * acc;
        }

        /// <summary>
        /// Computes the log determinant of the source matrix.
        /// </summary>
        /// <returns>
        /// The log determinant of the source matrix.
        /// </returns>
        public double LogDeterminant()
        {
            var acc = 0.0;
            foreach (var value in l.EnumerateDiagonalElements())
            {
                acc += Math.Log(value);
            }
            return 2 * acc;
        }

        /// <summary>
        /// The matrix L.
        /// </summary>
        public ref readonly Mat<double> L => ref l;
    }
}
