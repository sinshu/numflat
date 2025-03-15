using System;
using System.Buffers;

namespace NumFlat
{
    internal readonly ref struct TemporalArray<T> where T : unmanaged
    {
        private readonly T[] buffer;
        public readonly Span<T> Item;

        public TemporalArray(int count)
        {
            buffer = ArrayPool<T>.Shared.Rent(count);
            Item = buffer.AsSpan(0, count);
#if !RELEASE
            TemporalArray.Randomize<T>(buffer);
#endif
        }

        public void Dispose()
        {
#if !RELEASE
            TemporalArray.Randomize<T>(buffer);
#endif
            ArrayPool<T>.Shared.Return(buffer);
        }
    }



    internal static class TemporalArray
    {
#if !RELEASE
        public static void Randomize<T>(Span<T> buffer) where T : unmanaged
        {
            var random = new Random(42);
            var span = System.Runtime.InteropServices.MemoryMarshal.Cast<T, byte>(buffer);
            random.NextBytes(span);
        }
#endif
    }
}
