using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides methods to create matrices.
    /// </summary>
    public static class MatrixBuilder
    {
        /// <summary>
        /// Creates an identity matrix.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="count">
        /// The order of the square matrix.
        /// </param>
        /// <returns>
        /// The specified identity matrix.
        /// </returns>
        public static Mat<T> Identity<T>(int count) where T : unmanaged, INumberBase<T>
        {
            var identity = new Mat<T>(count, count);
            for (var i = 0; i < count; i++)
            {
                identity[i, i] = T.One;
            }
            return identity;
        }

        /// <summary>
        /// Creates a new matrix from the specified elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="elements">
        /// The elements for the new matrix.
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

        /// <summary>
        /// Creates a new matrix from the specified diagonal elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="elements">
        /// The diagonal elements for the new matrix.
        /// </param>
        /// <returns>
        /// A new matrix that contains the specified diagonal elements.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original storage.
        /// </remarks>
        public static Mat<T> ToDiagonalMatrix<T>(this IEnumerable<T> elements) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfNull(elements, nameof(elements));

            var array = elements.ToArray();
            if (array.Length == 0)
            {
                throw new ArgumentException("One or more elements are required.");
            }

            var matrix = new Mat<T>(array.Length, array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                matrix[i, i] = array[i];
            }

            return matrix;
        }

        /// <summary>
        /// Creates a new matrix from the specified diagonal elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="elements">
        /// The diagonal elements for the new matrix.
        /// </param>
        /// <param name="rowCount">
        /// The number of rows.
        /// </param>
        /// <param name="colCount">
        /// The number of columns.
        /// </param>
        /// <returns>
        /// A new matrix that contains the specified diagonal elements.
        /// </returns>
        /// <remarks>
        /// The number of elements must be less than or equal to 'min(<paramref name="rowCount"/>, <paramref name="colCount"/>)'.
        /// This method allocates a new matrix which is independent from the original storage.
        /// </remarks>
        public static Mat<T> ToDiagonalMatrix<T>(this IEnumerable<T> elements, int rowCount, int colCount) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfNull(elements, nameof(elements));

            var matrix = new Mat<T>(rowCount, colCount);
            var limit = Math.Min(rowCount, colCount);
            var i = 0;
            foreach (var element in elements)
            {
                if (i == limit)
                {
                    throw new ArgumentException("Too many elements for the size of the matrix.");
                }

                matrix[i, i] = element;
                i++;
            }

            return matrix;
        }

        /// <summary>
        /// Creates a new matrix from the specified rows.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="rows">
        /// The rows for the new matrix.
        /// </param>
        /// <returns>
        /// A new matrix that contains the specified rows.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original rows.
        /// </remarks>
        public static Mat<T> RowsToMatrix<T>(this IEnumerable<IEnumerable<T>> rows) where T : unmanaged, INumberBase<T>
        {
            var cache = new List<T[]>();
            var colCount = -1;
            foreach (var row in rows.Select(x => x.ToArray()))
            {
                if (colCount == -1)
                {
                    if (row.Length == 0)
                    {
                        new ArgumentException("Zero-length rows are not allowed.");
                    }

                    colCount = row.Length;
                }

                if (row.Length != colCount)
                {
                    throw new ArgumentException("All the rows in 'rows' must be the same length.");
                }

                cache.Add(row);
            }

            var rowCount = cache.Count;
            if (rowCount == 0)
            {
                throw new ArgumentException("'rows' must contain at least one row.");
            }

            var destination = new Mat<T>(rowCount, colCount);
            var sd = destination.Memory.Span;
            var position = 0;
            for (var col = 0; col < colCount; col++)
            {
                for (var row = 0; row < rowCount; row++)
                {
                    sd[position] = cache[row][col];
                    position++;
                }
            }

            return destination;
        }

        /// <summary>
        /// Create a new matrix from the specified columns.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="cols">
        /// The columns for the new matrix.
        /// </param>
        /// <returns>
        /// A new matrix that contains the specified columns.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original columns.
        /// </remarks>
        public static Mat<T> ColsToMatrix<T>(this IEnumerable<IEnumerable<T>> cols) where T : unmanaged, INumberBase<T>
        {
            var cache = new List<T[]>();
            var rowCount = -1;
            foreach (var col in cols.Select(x => x.ToArray()))
            {
                if (rowCount == -1)
                {
                    if (col.Length == 0)
                    {
                        new ArgumentException("Zero-length columns are not allowed.");
                    }

                    rowCount = col.Length;
                }

                if (col.Length != rowCount)
                {
                    throw new ArgumentException("All the columns in 'cols' must be the same length.");
                }

                cache.Add(col);
            }

            var colCount = cache.Count;
            if (colCount == 0)
            {
                throw new ArgumentException("'cols' must contain at least one column.");
            }

            var destination = new Mat<T>(rowCount, colCount);
            var sd = destination.Memory.Span;
            var position = 0;
            for (var col = 0; col < colCount; col++)
            {
                for (var row = 0; row < rowCount; row++)
                {
                    sd[position] = cache[col][row];
                    position++;
                }
            }

            return destination;
        }
    }
}
