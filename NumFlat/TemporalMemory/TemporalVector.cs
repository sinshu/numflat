using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal ref struct TemporalVector<T> where T : unmanaged, INumberBase<T>
    {
        private IMemoryOwner<T>? owner;
        public readonly Vec<T> Item;

        public TemporalVector(int count)
        {
            owner = MemoryPool<T>.Shared.Rent(count);
            Item = new Vec<T>(owner.Memory.Slice(0, count));
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
