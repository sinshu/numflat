using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal ref struct TemporalContiguousVector<T> where T : unmanaged, INumberBase<T>
    {
        private readonly ref readonly Vec<T> original;
        private readonly T[]? buffer;
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
                buffer = ArrayPool<T>.Shared.Rent(original.Count);
                Item = new Vec<T>(buffer.AsMemory(0, original.Count));
                if (useOriginalContent)
                {
                    original.CopyTo(Item);
                }
            }
        }

        public void Dispose()
        {
            if (buffer != null)
            {
                Item.CopyTo(original);
                ArrayPool<T>.Shared.Return(buffer);
            }
        }
    }
}
