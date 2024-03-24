using System;
using MatFlat;

namespace NumFlat
{
    /// <summary>
    /// Provides the Cholesky decomposition.
    /// </summary>
    public sealed class CholeskyDecompositionSingle : MatrixDecompositionBase<float>
    {
        private Mat<float> l;

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
        public CholeskyDecompositionSingle(in Mat<float> a) : base(a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfNonSquare(a, nameof(a));

            var l = new Mat<float>(a.RowCount, a.ColCount);
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
        public static unsafe void Decompose(in Mat<float> a, in Mat<float> l)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(l, nameof(l));
            ThrowHelper.ThrowIfNonSquare(a, nameof(a));
            ThrowHelper.ThrowIfDifferentSize(a, l);

            a.CopyTo(l);

            fixed (float* pl = l.Memory.Span)
            {
                Factorization.Cholesky(l.RowCount, pl, l.Stride);
            }
        }

        /// <inheritdoc/>
        public unsafe override void Solve(in Vec<float> b, in Vec<float> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowIfInvalidSize(b, destination);

            Solve(l, b, destination);
        }

        internal static unsafe void Solve(in Mat<float> l, in Vec<float> b, in Vec<float> destination)
        {
            b.CopyTo(destination);

            fixed (float* pl = l.Memory.Span)
            fixed (float* pd = destination.Memory.Span)
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
        public float Determinant()
        {
            var fl = l.GetUnsafeFastIndexer();
            var value = 1.0F;
            for (var i = 0; i < l.RowCount; i++)
            {
                value *= fl[i, i];
            }
            return value * value;
        }

        /// <summary>
        /// Computes the log determinant of the source matrix.
        /// </summary>
        /// <returns>
        /// The log determinant of the source matrix.
        /// </returns>
        public float LogDeterminant()
        {
            var fl = l.GetUnsafeFastIndexer();
            var value = 0.0F;
            for (var i = 0; i < l.RowCount; i++)
            {
                value += MathF.Log(fl[i, i]);
            }
            return 2 * value;
        }

        /// <summary>
        /// The matrix L.
        /// </summary>
        public ref readonly Mat<float> L => ref l;
    }
}
