using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal ref struct TemporalVector2<T> where T : unmanaged, INumberBase<T>
    {
        private IMemoryOwner<T>? owner;
        public readonly Vec<T> Item1;
        public readonly Vec<T> Item2;

        public TemporalVector2(int count)
        {
            owner = MemoryPool<T>.Shared.Rent(2 * count);
            Item1 = new Vec<T>(owner.Memory.Slice(0, count));
            Item2 = new Vec<T>(owner.Memory.Slice(count, count));
        }

        public void Dispose()
        {
            if (owner != null)
            {
                owner.Dispose();
                owner = null;
            }
        }
    }
}
