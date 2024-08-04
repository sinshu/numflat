using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal readonly ref struct TemporalMatrix2<T> where T : unmanaged, INumberBase<T>
    {
        private readonly IMemoryOwner<T> owner;
        public readonly Mat<T> Item1;
        public readonly Mat<T> Item2;

        public TemporalMatrix2(int rowCount, int colCount)
        {
            var length = rowCount * colCount;
            owner = MemoryPool<T>.Shared.Rent(2 * length);
#if !RELEASE
            TemporalMatrix.Randomize(owner);
#endif
            Item1 = new Mat<T>(rowCount, colCount, rowCount, owner.Memory.Slice(0, length));
            Item2 = new Mat<T>(rowCount, colCount, rowCount, owner.Memory.Slice(length, length));
        }

        public void Dispose()
        {
#if !RELEASE
            TemporalMatrix.Randomize(owner);
#endif
            owner.Dispose();
        }
    }
}
