using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NumFlat
{
    /// <summary>
    /// Reperesents a matrix.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the matrix.
    /// </typeparam>
    [CollectionBuilder(typeof(MatrixBuilder), nameof(MatrixBuilder.Create))]
    [StructLayout(LayoutKind.Auto)]
    public readonly partial struct Mat<T> : IEnumerable<Vec<T>>, IFormattable where T : unmanaged, INumberBase<T>
    {
        private readonly int rowCount;
        private readonly int colCount;
        private readonly int stride;
        private readonly Memory<T> memory;

        /// <summary>
        /// Creates a new matrix.
        /// </summary>
        /// <param name="rowCount">
        /// The number of rows.
        /// </param>
        /// <param name="colCount">
        /// The number of columns.
        /// </param>
        /// <param name="stride">
        /// The stride of the columns.
        /// This value indicates the difference between the starting index of adjacent columns in the <paramref name="memory"/>.
        /// If the length of the <paramref name="memory"/> matches <c><paramref name="rowCount"/> * <paramref name="colCount"/></c>
        /// and the elements are arranged consecutively without gaps in the <paramref name="memory"/>,
        /// then this value matches <paramref name="rowCount"/>.
        /// </param>
        /// <param name="memory">
        /// The storage of the elements in the matrix.
        /// The length of the <paramref name="memory"/> must match
        /// <c><paramref name="stride"/> * (<paramref name="colCount"/> - 1) + <paramref name="rowCount"/></c>.
        /// </param>
        /// <remarks>
        /// This constructor does not allocate heap memory.
        /// The given <paramref name="memory"/> is directly used as the storage of the matrix.
        /// </remarks>
        public Mat(int rowCount, int colCount, int stride, Memory<T> memory)
        {
            if (rowCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), "'rowCount' must be a positive value.");
            }

            if (colCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), "'colCount' must be a positive value.");
            }

            if (stride < rowCount)
            {
                throw new ArgumentOutOfRangeException(nameof(stride), "'stride' must be greater than or equal to 'rowCount'.");
            }

            if (memory.Length != stride * (colCount - 1) + rowCount)
            {
                throw new ArgumentException("'memory.Length' must match 'stride * (colCount - 1) + rowCount'.");
            }

            this.rowCount = rowCount;
            this.colCount = colCount;
            this.stride = stride;
            this.memory = memory;
        }

        /// <summary>
        /// Creates a zero matrix.
        /// </summary>
        /// <param name="rowCount">
        /// The number of rows.
        /// </param>
        /// <param name="colCount">
        /// The number of columns.
        /// </param>
        /// <remarks>
        /// This constructor allocates heap memory to store the elements in the matrix.
        /// </remarks>
        public Mat(int rowCount, int colCount) : this(rowCount, colCount, rowCount, new T[rowCount * colCount])
        {
        }

        /// <summary>
        /// Fills the elements of the matrix with a specified value.
        /// </summary>
        /// <param name="value">
        /// The value to assign to each element of the matrix.
        /// </param>
        public readonly void Fill(T value)
        {
            if (this.rowCount == 0 || this.colCount == 0)
            {
                throw new InvalidOperationException("Method call against an empty matrix is not allowed.");
            }

            var span = memory.Span;
            var offset = 0;
            while (offset < span.Length)
            {
                var position = offset;
                var end = offset + rowCount;
                while (position < end)
                {
                    span[position] = value;
                    position++;
                }
                offset += stride;
            }
        }

        /// <summary>
        /// Zero-clears the elements of the matrix.
        /// </summary>
        public readonly void Clear()
        {
            if (this.rowCount == 0 || this.colCount == 0)
            {
                throw new InvalidOperationException("Method call against an empty matrix is not allowed.");
            }

            Fill(default);
        }

        /// <summary>
        /// Gets a submatrix from the matrix.
        /// </summary>
        /// <param name="startRow">
        /// The starting row position of the submatrix in the matrix.
        /// </param>
        /// <param name="startCol">
        /// The starting column position of the submatrix in the matrix.
        /// </param>
        /// <param name="rowCount">
        /// The number of rows of the submatrix.
        /// </param>
        /// <param name="colCount">
        /// The number of columns of the submatrix.
        /// </param>
        /// <returns>
        /// The specified submatrix in the matrix.
        /// </returns>
        /// <remarks>
        /// This method does not allocate heap memory.
        /// The returned submatrix will be a view of the original matrix.
        /// </remarks>
        public readonly Mat<T> Submatrix(int startRow, int startCol, int rowCount, int colCount)
        {
            if (this.rowCount == 0 || this.colCount == 0)
            {
                throw new InvalidOperationException("Method call against an empty matrix is not allowed.");
            }

            if (startRow < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startRow), "'startRow' must be a non-negative value.");
            }

            if (startCol < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startCol), "'startCol' must be a non-negative value.");
            }

            if (rowCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), "'rowCount' must be a positive value.");
            }

            if (colCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(colCount), "'colCount' must be a positive value.");
            }

            if (startRow + rowCount > this.rowCount)
            {
                throw new ArgumentOutOfRangeException("'startRow + rowCount' must be less than or equal to the number of rows of the source matrix.");
            }

            if (startCol + colCount > this.colCount)
            {
                throw new ArgumentOutOfRangeException("'startCol + colCount' must be less than or equal to the number of columns of the source matrix.");
            }

            var stride = this.stride;
            var memory = this.memory.Slice(stride * startCol + startRow, stride * (colCount - 1) + rowCount);
            return new Mat<T>(rowCount, colCount, stride, memory);
        }

        /// <summary>
        /// Copies the elements of the matrix into another matrix.
        /// </summary>
        /// <param name="destination">
        /// The destination matrix.
        /// </param>
        /// <remarks>
        /// The dimensions of the matrices must match.
        /// </remarks>
        public readonly void CopyTo(in Mat<T> destination)
        {
            if (this.rowCount == 0 || this.colCount == 0)
            {
                throw new InvalidOperationException("Method call against an empty matrix is not allowed.");
            }

            ThrowHelper.ThrowIfDifferentSize(this, destination);

            if (this.rowCount == destination.rowCount &&
                this.colCount == destination.colCount &&
                this.stride == destination.stride &&
                this.memory.Equals(destination.memory))
            {
                return;
            }

            var st = this.memory.Span;
            var sd = destination.memory.Span;
            var ot = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var pt = ot;
                var pd = od;
                var end = od + rowCount;
                while (pd < end)
                {
                    sd[pd] = st[pt];
                    pt++;
                    pd++;
                }
                ot += this.stride;
                od += destination.stride;
            }
        }

        /// <summary>
        /// Converts the matrix to a 2D array.
        /// </summary>
        /// <returns>
        /// The 2D array converted from the matrix.
        /// </returns>
        /// <remarks>
        /// This method allocates a new 2D array which is independent from the storage of the matrix.
        /// </remarks>
        public readonly T[,] ToArray()
        {
            if (this.rowCount == 0 || this.colCount == 0)
            {
                throw new InvalidOperationException("Method call against an empty matrix is not allowed.");
            }

            var array = new T[rowCount, colCount];
            for (var row = 0; row < rowCount; row++)
            {
                var position = row;
                for (var col = 0; col < colCount; col++)
                {
                    array[row, col] = memory.Span[position];
                    position += stride;
                }
            }
            return array;
        }

        /// <summary>
        /// Gets or sets the element at the specified position in the matrix.
        /// </summary>
        /// <param name="row">
        /// The row index of the element.
        /// </param>
        /// <param name="col">
        /// The column index of the element.
        /// </param>
        /// <returns>
        /// The specified element.
        /// </returns>
        public readonly T this[int row, int col]
        {
            get
            {
                if (this.rowCount == 0 || this.colCount == 0)
                {
                    throw new InvalidOperationException("Method call against an empty matrix is not allowed.");
                }

                if ((uint)row >= this.rowCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(row), "Index must be within the number of rows.");
                }

                if ((uint)col >= this.colCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(col), "Index must be within the number of columns.");
                }

                return this.memory.Span[stride * col + row];
            }

            set
            {
                if (this.rowCount == 0 || this.colCount == 0)
                {
                    throw new InvalidOperationException("Method call against an empty matrix is not allowed.");
                }

                if ((uint)row >= this.rowCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(row), "Index must be within the number of rows.");
                }

                if ((uint)col >= this.colCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(col), "Index must be within the number of columns.");
                }

                this.memory.Span[stride * col + row] = value;
            }
        }

        /// <summary>
        /// Gets a submatrix from the matrix.
        /// </summary>
        /// <param name="rowRange">
        /// The range of the rows;
        /// </param>
        /// <param name="colRange">
        /// The range of the columns.
        /// </param>
        /// <returns>
        /// The specified submatrix in the matrix.
        /// </returns>
        /// <remarks>
        /// This method does not allocate heap memory.
        /// The returned submatrix will be a view of the original matrix.
        /// </remarks>
        public readonly Mat<T> this[Range rowRange, Range colRange]
        {
            get
            {
                var (rowOffset, rowLength) = rowRange.GetOffsetAndLength(rowCount);
                var (colOffset, colLength) = colRange.GetOffsetAndLength(colCount);
                return Submatrix(rowOffset, colOffset, rowLength, colLength);
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the matrix is empty.
        /// </summary>
        public readonly bool IsEmpty => rowCount == 0 || colCount == 0;

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        public readonly int RowCount => rowCount;

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        public readonly int ColCount => colCount;

        /// <summary>
        /// Gets the stride value for the storage.
        /// </summary>
        public readonly int Stride => stride;

        /// <summary>
        /// Gets the storage of the matrix.
        /// </summary>
        public readonly Memory<T> Memory => memory;

        /// <summary>
        /// Gets a view of the matrix as a list of row vectors.
        /// </summary>
        public readonly RowList Rows => new RowList(in this);

        /// <summary>
        /// Gets a view of the matrix as a list of column vectors.
        /// </summary>
        public readonly ColList Cols => new ColList(in this);
    }
}
