using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public static Mat<T> ToDiagonalMatrix<T>(this IEnumerable<T> elements) where T : unmanaged, INumberBase<T>
        {
            var array = elements.ToArray();
            var mat = new Mat<T>(array.Length, array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                mat[i, i] = array[i];
            }
            return mat;
        }

        public static Mat<T> ToDiagonalMatrix<T>(this IEnumerable<T> elements, int rowCount, int colCount) where T : unmanaged, INumberBase<T>
        {
            var mat = new Mat<T>(rowCount, colCount);
            var i = 0;
            foreach (var element in elements)
            {
                mat[i, i] = element;
                i++;
            }
            return mat;
        }

        /// <summary>
        /// Create a new matrix from the specified rows.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="rows">
        /// The rows in the matrix.
        /// </param>
        /// <returns>
        /// A new matrix that contains the specified rows.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original storage.
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
        /// The columns in the matrix.
        /// </param>
        /// <returns>
        /// A new matrix that contains the specified columns.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original storage.
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
                throw new ArgumentException("'rows' must contain at least one row.");
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
