using System;
using System.Numerics;
using MatFlat;

namespace NumFlat
{
    /// <summary>
    /// Provides the Cholesky decomposition.
    /// </summary>
    public sealed class CholeskyDecompositionComplex : MatrixDecompositionBase<Complex>
    {
        private readonly Mat<Complex> l;

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
        /// The matrix to be decomposed must be Hermitian symmetric.
        /// Note that this implementation does not check if the input matrix is Hermitian symmetric.
        /// Specifically, only the upper triangular part of the input matrix is referenced, and the rest is ignored.
        /// </remarks>
        public CholeskyDecompositionComplex(in Mat<Complex> a) : base(a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfNonSquare(a, nameof(a));

            var l = new Mat<Complex>(a.RowCount, a.ColCount);
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
        /// The matrix to be decomposed must be Hermitian symmetric.
        /// Note that this implementation does not check if the input matrix is Hermitian symmetric.
        /// Specifically, only the upper triangular part of the input matrix is referenced, and the rest is ignored.
        /// </remarks>
        public static unsafe void Decompose(in Mat<Complex> a, in Mat<Complex> l)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(l, nameof(l));
            ThrowHelper.ThrowIfNonSquare(a, nameof(a));
            ThrowHelper.ThrowIfDifferentSize(a, l);

            a.CopyTo(l);

            fixed (Complex* pl = l.Memory.Span)
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
        public unsafe override void Solve(in Vec<Complex> b, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowIfInvalidSize(b, destination);

            Solve(l, b, destination);
        }

        internal static unsafe void Solve(in Mat<Complex> l, in Vec<Complex> b, in Vec<Complex> destination)
        {
            b.CopyTo(destination);

            fixed (Complex* pl = l.Memory.Span)
            fixed (Complex* pd = destination.Memory.Span)
            {
                Blas.SolveTriangular(
                    Uplo.Lower,
                    Transpose.NoTrans,
                    l.RowCount,
                    pl, l.Stride,
                    pd, destination.Stride);

                Blas.SolveTriangular(
                    Uplo.Lower,
                    Transpose.ConjTrans,
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
                acc *= value.Real;
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
                acc += Math.Log(value.Real);
            }
            return 2 * acc;
        }

        /// <summary>
        /// The matrix L.
        /// </summary>
        public ref readonly Mat<Complex> L => ref l;
    }
}
