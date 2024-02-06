using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal ref struct TemporalArray<T>
    {
        private IMemoryOwner<T>? owner;

        public TemporalArray(int length)
        {
            owner = MemoryPool<T>.Shared.Rent(length);
        }

        public Span<T> Span => owner!.Memory.Span;

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
