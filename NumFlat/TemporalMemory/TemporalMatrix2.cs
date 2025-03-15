using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal readonly ref struct TemporalMatrix2<T> where T : unmanaged, INumberBase<T>
    {
        private readonly T[] buffer;
        public readonly Mat<T> Item1;
        public readonly Mat<T> Item2;

        public TemporalMatrix2(int rowCount, int colCount)
        {
            var length = rowCount * colCount;
            buffer = ArrayPool<T>.Shared.Rent(2 * length);
#if !RELEASE
            TemporalMatrix.Randomize<T>(buffer);
#endif
            Item1 = new Mat<T>(rowCount, colCount, rowCount, buffer.AsMemory(0, length));
            Item2 = new Mat<T>(rowCount, colCount, rowCount, buffer.AsMemory(length, length));
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
