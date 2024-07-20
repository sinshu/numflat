using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal readonly ref struct TemporalVector<T> where T : unmanaged, INumberBase<T>
    {
        private readonly IMemoryOwner<T> owner;
        public readonly Vec<T> Item;

        public TemporalVector(int count)
        {
            owner = MemoryPool<T>.Shared.Rent(count);
            Item = new Vec<T>(owner.Memory.Slice(0, count));
#if !RELEASE
            TemporalVector.Randomize(owner);
#endif
        }

        public void Dispose()
        {
            owner.Dispose();
        }
    }



    internal static class TemporalVector
    {
        public static TemporalContiguousVector<T> EnsureContiguous<T>(in this Vec<T> original, bool useOriginalContent) where T : unmanaged, INumberBase<T>
        {
            return new TemporalContiguousVector<T>(original, useOriginalContent);
        }

#if !RELEASE
        public static void Randomize<T>(IMemoryOwner<T> owner) where T : unmanaged, INumberBase<T>
        {
            var random = new Random(42);
            var span = System.Runtime.InteropServices.MemoryMarshal.Cast<T, byte>(owner.Memory.Span);
            random.NextBytes(span);
        }
#endif
    }
}
