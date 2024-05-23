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
        /// Creates a new matrix from the specified row vectors.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="rows">
        /// The row vectors for the new matrix.
        /// </param>
        /// <returns>
        /// A new matrix that contains the specified row vectors.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original storage.
        /// </remarks>
        public static Mat<T> Create<T>(ReadOnlySpan<Vec<T>> rows) where T : unmanaged, INumberBase<T>
        {
            if (rows.Length == 0)
            {
                throw new ArgumentException("The sequence must contain at least one row.");
            }

            var rowCount = rows.Length;
            var colCount = rows[0].Count;

            foreach (ref readonly var row in rows)
            {
                if (row.IsEmpty)
                {
                    new ArgumentException("Empty rows are not allowed.");
                }

                if (row.Count != colCount)
                {
                    throw new ArgumentException("All the rows must have the same length.");
                }
            }

            var mat = new Mat<T>(rowCount, colCount);
            var i = 0;
            foreach (var dst in mat.Rows)
            {
                rows[i].CopyTo(dst);
                i++;
            }

            return mat;
        }

        /// <summary>
        /// Creates a new matrix which is filled with a specified value.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="rowCount">
        /// The number of rows.
        /// </param>
        /// <param name="colCount">
        /// The number of columns.
        /// </param>
        /// <param name="value">
        /// The value to fill the new matrix.
        /// </param>
        /// <returns>
        /// The new matrix filled with the specified value.
        /// </returns>
        public static Mat<T> Fill<T>(int rowCount, int colCount, T value) where T : unmanaged, INumberBase<T>
        {
            if (rowCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), "The number of rows must be greater than zero.");
            }

            if (colCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(colCount), "The number of columns must be greater than zero.");
            }

            var mat = new Mat<T>(rowCount, colCount);
            mat.Fill(value);
            return mat;
        }

        /// <summary>
        /// Creates a new matrix from a specified function.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="rowCount">
        /// The number of rows.
        /// </param>
        /// <param name="colCount">
        /// The number of columns.
        /// </param>
        /// <param name="func">
        /// The function which generates the values for the new matrix.
        /// The row and column indices are given as arguments of the function.
        /// </param>
        /// <returns>
        /// The new matrix generated with the function.
        /// </returns>
        public static Mat<T> FromFunc<T>(int rowCount, int colCount, Func<int, int, T> func) where T : unmanaged, INumberBase<T>
        {
            if (rowCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), "The number of rows must be greater than zero.");
            }

            if (colCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(colCount), "The number of columns must be greater than zero.");
            }

            ThrowHelper.ThrowIfNull(func, nameof(func));

            var mat = new Mat<T>(rowCount, colCount);
            var span = mat.Memory.Span;
            var position = 0;
            for (var col = 0; col < colCount; col++)
            {
                for (var row = 0; row < rowCount; row++)
                {
                    span[position++] = func(row, col);
                }
            }
            return mat;
        }

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
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "The order of the matrix must be greater than zero.");
            }

            var identity = new Mat<T>(count, count);
            var fi = identity.GetUnsafeFastIndexer();
            for (var i = 0; i < count; i++)
            {
                fi[i, i] = T.One;
            }
            return identity;
        }

        /// <summary>
        /// Creates an identity matrix.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="rowCount">
        /// The number of rows.
        /// </param>
        /// <param name="colCount">
        /// The number of columns.
        /// </param>
        /// <returns>
        /// The specified identity matrix.
        /// </returns>
        public static Mat<T> Identity<T>(int rowCount, int colCount) where T : unmanaged, INumberBase<T>
        {
            if (rowCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), "The number of rows must be greater than zero.");
            }

            if (colCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(colCount), "The number of columns must be greater than zero.");
            }

            var identity = new Mat<T>(rowCount, colCount);
            var fi = identity.GetUnsafeFastIndexer();
            var min = Math.Min(rowCount, colCount);
            for (var i = 0; i < min; i++)
            {
                fi[i, i] = T.One;
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
                throw new ArgumentException("The sequence must contain at least one element.");
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
                throw new ArgumentException("The sequence must contain at least one element.");
            }

            var matrix = new Mat<T>(array.Length, array.Length);
            var fm = matrix.GetUnsafeFastIndexer();
            for (var i = 0; i < array.Length; i++)
            {
                fm[i, i] = array[i];
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
            var fm = matrix.GetUnsafeFastIndexer();
            var limit = Math.Min(rowCount, colCount);
            var i = 0;
            foreach (var element in elements)
            {
                if (i == limit)
                {
                    throw new ArgumentException("Too many elements for the size of the matrix.");
                }

                fm[i, i] = element;
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
                        new ArgumentException("Empty rows are not allowed.");
                    }

                    colCount = row.Length;
                }

                if (row.Length != colCount)
                {
                    throw new ArgumentException("All the rows must have the same length.");
                }

                cache.Add(row);
            }

            var rowCount = cache.Count;
            if (rowCount == 0)
            {
                throw new ArgumentException("The sequence must contain at least one row.");
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
                        new ArgumentException("Empty columns are not allowed.");
                    }

                    rowCount = col.Length;
                }

                if (col.Length != rowCount)
                {
                    throw new ArgumentException("All the columns must have the same length.");
                }

                cache.Add(col);
            }

            var colCount = cache.Count;
            if (colCount == 0)
            {
                throw new ArgumentException("The sequence must contain at least one column.");
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
