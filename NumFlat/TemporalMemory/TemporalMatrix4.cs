using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal readonly ref struct TemporalMatrix4<T> where T : unmanaged, INumberBase<T>
    {
        private readonly IMemoryOwner<T> owner;
        public readonly Mat<T> Item1;
        public readonly Mat<T> Item2;
        public readonly Mat<T> Item3;
        public readonly Mat<T> Item4;

        public TemporalMatrix4(int rowCount, int colCount)
        {
            var length = rowCount * colCount;
            owner = MemoryPool<T>.Shared.Rent(4 * length);
#if !RELEASE
            TemporalMatrix.Randomize(owner);
#endif
            Item1 = new Mat<T>(rowCount, colCount, rowCount, owner.Memory.Slice(0 * length, length));
            Item2 = new Mat<T>(rowCount, colCount, rowCount, owner.Memory.Slice(1 * length, length));
            Item3 = new Mat<T>(rowCount, colCount, rowCount, owner.Memory.Slice(2 * length, length));
            Item4 = new Mat<T>(rowCount, colCount, rowCount, owner.Memory.Slice(3 * length, length));
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
