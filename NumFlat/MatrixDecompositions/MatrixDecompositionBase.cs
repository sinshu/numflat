using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides common functionality across matrix decomposition methods.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the matrix.
    /// </typeparam>
    public abstract class MatrixDecompositionBase<T> where T : unmanaged, INumberBase<T>
    {
        private int rhsVectorLength;
        private int solutionVectorLength;

        /// <summary>
        /// Decomposes the matrix.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        protected MatrixDecompositionBase(in Mat<T> a)
        {
            rhsVectorLength = a.RowCount;
            solutionVectorLength = a.ColCount;
        }

        /// <summary>
        /// Solves the linear equation, Ax = b.
        /// </summary>
        /// <param name="b">
        /// The right-hand side vector.
        /// </param>
        /// <param name="destination">
        /// The destination of the solution vector.
        /// </param>
        public abstract void Solve(in Vec<T> b, in Vec<T> destination);

        /// <summary>
        /// Solves the linear equation, Ax = b.
        /// </summary>
        /// <param name="b">
        /// The right-hand side vector.
        /// </param>
        /// <returns>
        /// The solution vector.
        /// </returns>
        public Vec<T> Solve(in Vec<T> b)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            if (b.Count != rhsVectorLength)
            {
                throw new ArgumentException("The length of the right-hand side vector does not meet the requirement.");
            }

            var x = new Vec<T>(solutionVectorLength);
            Solve(b, x);
            return x;
        }
    }
}
