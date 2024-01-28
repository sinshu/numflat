using System;
using System.Linq;
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

            if (rowCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), "`rowCount` must be a positive value.");
            }

            if (colCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(colCount), "`colCount` must be a positive value.");
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

        public void CopyTo(Mat<T> destination)
        {
            if (destination.rowCount != this.rowCount)
            {
                throw new ArgumentException("`Mat<T>.RowCount` and `destination.RowCount` must be the same value.");
            }

            if (destination.colCount != this.colCount)
            {
                throw new ArgumentException("`Mat<T>.ColCount` and `destination.ColCount` must be the same value.");
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

        public T[,] ToArray()
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
