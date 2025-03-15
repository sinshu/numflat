using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal readonly ref struct TemporalMatrix3<T> where T : unmanaged, INumberBase<T>
    {
        private readonly T[] buffer;
        public readonly Mat<T> Item1;
        public readonly Mat<T> Item2;
        public readonly Mat<T> Item3;

        public TemporalMatrix3(int rowCount, int colCount)
        {
            var length = rowCount * colCount;
            buffer = ArrayPool<T>.Shared.Rent(3 * length);
#if !RELEASE
            TemporalMatrix.Randomize<T>(buffer);
#endif
            Item1 = new Mat<T>(rowCount, colCount, rowCount, buffer.AsMemory(0 * length, length));
            Item2 = new Mat<T>(rowCount, colCount, rowCount, buffer.AsMemory(1 * length, length));
            Item3 = new Mat<T>(rowCount, colCount, rowCount, buffer.AsMemory(2 * length, length));
        }

        public void Dispose()
        {
#if !RELEASE
            TemporalMatrix.Randomize<T>(buffer);
#endif
            ArrayPool<T>.Shared.Return(buffer);
        }
    }
}
