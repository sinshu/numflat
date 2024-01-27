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
                throw new ArgumentOutOfRangeException(nameof(rowCount), "The number of rows must be a positive integer.");
            }

            if (colCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), "The number of columns must be a positive integer.");
            }

            if (stride <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stride), "The stride must be a positive integer.");
            }

            if (memory.Length != stride * (colCount - 1) + rowCount)
            {
                throw new ArgumentException("The length of the memory must match `stride * (colCount - 1) + rowCount`.");
            }

            this.rowCount = rowCount;
            this.colCount = colCount;
            this.stride = stride;
            this.memory = memory;
        }

        public int RowCount => rowCount;
        public int ColCount => colCount;
        public int Stride => stride;
        public Memory<T> Memory => memory;
    }
}
