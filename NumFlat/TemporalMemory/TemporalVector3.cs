﻿using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal readonly ref struct TemporalVector3<T> where T : unmanaged, INumberBase<T>
    {
        private readonly T[] buffer;
        public readonly Vec<T> Item1;
        public readonly Vec<T> Item2;
        public readonly Vec<T> Item3;

        public TemporalVector3(int count)
        {
            buffer = ArrayPool<T>.Shared.Rent(3 * count);
#if !RELEASE
            TemporalVector.Randomize<T>(buffer);
#endif
            Item1 = new Vec<T>(buffer.AsMemory(0 * count, count));
            Item2 = new Vec<T>(buffer.AsMemory(1 * count, count));
            Item3 = new Vec<T>(buffer.AsMemory(2 * count, count));
        }

        public void Dispose()
        {
#if !RELEASE
            TemporalVector.Randomize<T>(buffer);
#endif
            ArrayPool<T>.Shared.Return(buffer);
        }
    }
}
