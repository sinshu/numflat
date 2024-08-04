using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal readonly ref struct TemporalVector2<T> where T : unmanaged, INumberBase<T>
    {
        private readonly IMemoryOwner<T> owner;
        public readonly Vec<T> Item1;
        public readonly Vec<T> Item2;

        public TemporalVector2(int count)
        {
            owner = MemoryPool<T>.Shared.Rent(2 * count);
#if !RELEASE
            TemporalVector.Randomize(owner);
#endif
            Item1 = new Vec<T>(owner.Memory.Slice(0, count));
            Item2 = new Vec<T>(owner.Memory.Slice(count, count));
        }

        public void Dispose()
        {
#if !RELEASE
            TemporalVector.Randomize(owner);
#endif
            owner.Dispose();
        }
    }
}
