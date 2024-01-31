using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides methods to create matrices.
    /// </summary>
    public static class MatrixBuilder
    {
        /// <summary>
        /// Create a new matrix from the specified elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="elements">
        /// The elements in the matrix.
        /// </param>
        /// <returns>
        /// A new matrix that contains the specified elements.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original storage.
        /// </remarks>
        public static Mat<T> ToMatrix<T>(this T[,] elements) where T : unmanaged, INumberBase<T>
        {
            if (elements.GetLength(0) == 0 || elements.GetLength(1) == 0)
            {
                throw new ArgumentException("One or more elements are required.");
            }

            var destination = new Mat<T>(elements.GetLength(0), elements.GetLength(1));
            var sd = destination.Memory.Span;
            var position = 0;
            for (var col = 0; col < destination.ColCount; col++)
            {
                for (var row = 0; row < destination.RowCount; row++)
                {
                    sd[position] = elements[row, col];
                    position++;
                }
            }

            return destination;
        }
    }
}
