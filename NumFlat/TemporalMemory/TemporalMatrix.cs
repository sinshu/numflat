using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal readonly ref struct TemporalMatrix<T> where T : unmanaged, INumberBase<T>
    {
        private readonly T[] buffer;
        public readonly Mat<T> Item;

        public TemporalMatrix(int rowCount, int colCount)
        {
            var length = rowCount * colCount;
            buffer = ArrayPool<T>.Shared.Rent(length);
#if !RELEASE
            TemporalMatrix.Randomize<T>(buffer);
#endif
            Item = new Mat<T>(rowCount, colCount, rowCount, buffer.AsMemory(0, length));
        }

        public void Dispose()
        {
#if !RELEASE
            TemporalMatrix.Randomize<T>(buffer);
#endif
            ArrayPool<T>.Shared.Return(buffer);
        }
    }



    internal static class TemporalMatrix
    {
        public static TemporalMatrix<T> CopyFrom<T>(in Mat<T> source) where T : unmanaged, INumberBase<T>
        {
            var tmp = new TemporalMatrix<T>(source.RowCount, source.ColCount);
            source.CopyTo(tmp.Item);
            return tmp;
        }

#if !RELEASE
        public static void Randomize<T>(Span<T> buffer) where T : unmanaged, INumberBase<T>
        {
            var random = new Random(42);
            var span = System.Runtime.InteropServices.MemoryMarshal.Cast<T, byte>(buffer);
            random.NextBytes(span);
        }
#endif
    }
}
