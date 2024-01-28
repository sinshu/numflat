using System;
using System.Numerics;

namespace NumFlat
{
    public partial struct Mat<T> where T : unmanaged, INumberBase<T>
    {
        private readonly int rowCount;
        private readonly int colCount;
        private readonly int stride;
        private readonly Memory<T> memory;

        public Mat(int rowCount, int colCount, int stride, Memory<T> memory)
        {
            if (rowCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), "`rowCount` must be a positive value.");
            }

            if (colCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), "`colCount` must be a positive value.");
            }

            if (stride <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stride), "`stride` must be a positive value.");
            }

            if (memory.Length != stride * (colCount - 1) + rowCount)
            {
                throw new ArgumentException("`memory.Length` must match `stride * (colCount - 1) + rowCount`.");
            }

            this.rowCount = rowCount;
            this.colCount = colCount;
            this.stride = stride;
            this.memory = memory;
        }

        public Mat(int rowCount, int colCount) : this(rowCount, colCount, rowCount, new T[rowCount * colCount])
        {
        }

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

        public void Clear()
        {
            Fill(default);
        }

        public Mat<T> Submatrix(int startRow, int startCol, int rowCount, int colCount)
        {
            if (startRow < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startRow), "`startRow` must be a non-negative value.");
            }

            if (startCol < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startCol), "`startCol` must be a non-negative value.");
            }

            if (startRow + rowCount > this.rowCount)
            {
                throw new ArgumentOutOfRangeException("`startRow + rowCount` must be less than or equal to `Vec<T>.RowCount`.");
            }

            if (startCol + colCount > this.colCount)
            {
                throw new ArgumentOutOfRangeException("`startCol + colCount` must be less than or equal to `Vec<T>.ColCount`.");
            }

            var stride = this.stride;
            var memory = this.memory.Slice(stride * startCol + startRow, stride * (colCount - 1) + rowCount);
            return new Mat<T>(rowCount, colCount, stride, memory);
        }

        public T this[int row, int col]
        {
            get => memory.Span[stride * col + row];
            set => memory.Span[stride * col + row] = value;
        }

        public int RowCount => rowCount;
        public int ColCount => colCount;
        public int Stride => stride;
        public Memory<T> Memory => memory;
        public RowList Rows => new RowList(ref this);
        public ColList Cols => new ColList(ref this);
    }
}
