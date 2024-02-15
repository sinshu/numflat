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
            ThrowIfInvalidSize(b);

            var x = new Vec<T>(solutionVectorLength);
            Solve(b, x);
            return x;
        }

        /// <summary>
        /// Solves the linear equation, Ax = b.
        /// </summary>
        /// <param name="b">
        /// The right-hand side vectors.
        /// </param>
        /// <param name="destination">
        /// The destination of the solution vectors.
        /// </param>
        public void Solve(in Mat<T> b, in Mat<T> destination)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowIfInvalidSize(b, destination);

            var bCols = b.Cols;
            var destinationCols = destination.Cols;
            for (var col = 0; col < destinationCols.Count; col++)
            {
                var bCol = bCols[col];
                var destinationCol = destinationCols[col];
                Solve(bCol, destinationCol);
            }
        }

        /// <summary>
        /// Solves the linear equation, Ax = b.
        /// </summary>
        /// <param name="b">
        /// The right-hand side vectors.
        /// </param>
        /// <returns>
        /// The solution vectors.
        /// </returns>
        public Mat<T> Solve(in Mat<T> b)
        {
            ThrowHelper.ThrowIfEmpty(b, nameof(b));
            ThrowIfInvalidSize(b);

            var destination = new Mat<T>(solutionVectorLength, b.ColCount);
            Solve(b, destination);
            return destination;
        }

        internal void ThrowIfInvalidSize(in Vec<T> b)
        {
            if (b.Count != rhsVectorLength)
            {
                throw new ArgumentException($"The solver requires the length of the right-hand side vector to be {rhsVectorLength}, but was {b.Count}.");
            }
        }

        internal void ThrowIfInvalidSize(in Vec<T> b, in Vec<T> destination)
        {
            if (b.Count != rhsVectorLength)
            {
                throw new ArgumentException($"The solver requires the length of the right-hand side vector to be {rhsVectorLength}, but was {b.Count}.");
            }

            if (destination.Count != solutionVectorLength)
            {
                throw new ArgumentException($"The solver requires the length of the solution vector to be {solutionVectorLength}, but was {destination.Count}.");
            }
        }

        internal void ThrowIfInvalidSize(in Mat<T> b)
        {
            if (b.RowCount != rhsVectorLength)
            {
                throw new ArgumentException($"The solver requires the length of the right-hand side vector to be {rhsVectorLength}, but was {b.RowCount}.");
            }
        }

        internal void ThrowIfInvalidSize(in Mat<T> b, in Mat<T> destination)
        {
            if (b.RowCount != rhsVectorLength)
            {
                throw new ArgumentException($"The solver requires the length of the right-hand side vector to be {rhsVectorLength}, but was {b.RowCount}.");
            }

            if (destination.RowCount != solutionVectorLength)
            {
                throw new ArgumentException($"The solver requires the length of the solution vector to be {solutionVectorLength}, but was {destination.RowCount}.");
            }

            if (b.ColCount != destination.ColCount)
            {
                throw new ArgumentException("The number of the right-hand side vectors and solution vectors must match.");
            }
        }
    }
}
