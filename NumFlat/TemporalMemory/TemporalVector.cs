using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal readonly ref struct TemporalVector<T> where T : unmanaged, INumberBase<T>
    {
        private readonly T[] buffer;
        public readonly Vec<T> Item;

        public TemporalVector(int count)
        {
            buffer = ArrayPool<T>.Shared.Rent(count);
            Item = new Vec<T>(buffer.AsMemory(0, count));
#if !RELEASE
            TemporalVector.Randomize<T>(buffer);
#endif
        }

        public void Dispose()
        {
#if !RELEASE
            TemporalVector.Randomize<T>(buffer);
#endif
            ArrayPool<T>.Shared.Return(buffer);
        }
    }



    internal static class TemporalVector
    {
        public static TemporalContiguousVector<T> EnsureContiguous<T>(in this Vec<T> original, bool useOriginalContent) where T : unmanaged, INumberBase<T>
        {
            return new TemporalContiguousVector<T>(original, useOriginalContent);
        }

#if !RELEASE
        public static void Randomize<T>(Span<T> buffer) where T : unmanaged, INumberBase<T>
        {
            var random = new Random(42);
            var span = System.Runtime.InteropServices.MemoryMarshal.Cast<T, byte>(buffer);
            random.NextBytes(span);
        }
#endif
    }
}
