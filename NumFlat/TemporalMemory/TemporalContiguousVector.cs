using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal ref struct TemporalContiguousVector<T> where T : unmanaged, INumberBase<T>
    {
        private readonly ref readonly Vec<T> original;
        private readonly IMemoryOwner<T>? owner;
        public readonly Vec<T> Item;

        public TemporalContiguousVector(in Vec<T> original, bool useOriginalContent)
        {
            if (original.Stride == 1)
            {
                Item = original;
            }
            else
            {
                this.original = ref original;
                owner = MemoryPool<T>.Shared.Rent(original.Count);
                Item = new Vec<T>(owner.Memory.Slice(0, original.Count));
                if (useOriginalContent)
                {
                    original.CopyTo(Item);
                }
            }
        }

        public void Dispose()
        {
            if (owner != null)
            {
                Item.CopyTo(original);
                owner.Dispose();
            }
        }
    }



    internal static class TemporalContiguousVector
    {
        public static TemporalContiguousVector<T> EnsureContiguous<T>(in this Vec<T> original, bool useOriginalContent) where T : unmanaged, INumberBase<T>
        {
            return new TemporalContiguousVector<T>(original, useOriginalContent);
        }
    }
}
