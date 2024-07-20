using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal readonly ref struct TemporalMatrix3<T> where T : unmanaged, INumberBase<T>
    {
        private readonly IMemoryOwner<T> owner;
        public readonly Mat<T> Item1;
        public readonly Mat<T> Item2;
        public readonly Mat<T> Item3;

        public TemporalMatrix3(int rowCount, int colCount)
        {
            var length = rowCount * colCount;
            owner = MemoryPool<T>.Shared.Rent(3 * length);
#if !RELEASE
            TemporalMatrix.Randomize(owner);
#endif
            Item1 = new Mat<T>(rowCount, colCount, rowCount, owner.Memory.Slice(0 * length, length));
            Item2 = new Mat<T>(rowCount, colCount, rowCount, owner.Memory.Slice(1 * length, length));
            Item3 = new Mat<T>(rowCount, colCount, rowCount, owner.Memory.Slice(2 * length, length));
        }

        public void Dispose()
        {
            owner.Dispose();
        }
    }
}
