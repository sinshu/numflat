using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Reperesents a matrix.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the matrix.
    /// </typeparam>
    public partial struct Mat<T> : IFormattable where T : unmanaged, INumberBase<T>
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
        /// If the length of the <paramref name="memory"/> matches <paramref name="rowCount"/> * <paramref name="colCount"/>
        /// and the elements are arranged consecutively without gaps in the <paramref name="memory"/>,
        /// then this value matches <paramref name="rowCount"/>.
        /// </param>
        /// <param name="memory">
        /// The storage of the elements in the matrix.
        /// The length of the <paramref name="memory"/> must match
        /// <paramref name="stride"/> * (<paramref name="colCount"/> - 1) + <paramref name="rowCount"/>.
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
        public void Fill(T value)
        {
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
        public void Clear()
        {
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
                throw new ArgumentOutOfRangeException("'startRow + rowCount' must be less than or equal to 'Vec<T>.RowCount'.");
            }

            if (startCol + colCount > this.colCount)
            {
                throw new ArgumentOutOfRangeException("'startCol + colCount' must be less than or equal to 'Vec<T>.ColCount'.");
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
        public readonly void CopyTo(Mat<T> destination)
        {
            if (destination.rowCount != this.rowCount)
            {
                throw new ArgumentException("'destination.RowCount' must match 'Mat<T>.RowCount'.");
            }

            if (destination.colCount != this.colCount)
            {
                throw new ArgumentException("'destination.ColCount' must match 'Mat<T>.ColCount'.");
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
        public T this[int row, int col]
        {
            readonly get => memory.Span[stride * col + row];
            set => memory.Span[stride * col + row] = value;
        }

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
        /// Gets the view of the matrix as a list of row vectors.
        /// </summary>
        public readonly RowList Rows => new RowList(in this);

        /// <summary>
        /// Gets the view of the matrix as a list of column vectors.
        /// </summary>
        public readonly ColList Cols => new ColList(in this);
    }
}
