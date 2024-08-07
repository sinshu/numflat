﻿using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal readonly ref struct TemporalVector3<T> where T : unmanaged, INumberBase<T>
    {
        private readonly IMemoryOwner<T> owner;
        public readonly Vec<T> Item1;
        public readonly Vec<T> Item2;
        public readonly Vec<T> Item3;

        public TemporalVector3(int count)
        {
            owner = MemoryPool<T>.Shared.Rent(3 * count);
#if !RELEASE
            TemporalVector.Randomize(owner);
#endif
            Item1 = new Vec<T>(owner.Memory.Slice(0 * count, count));
            Item2 = new Vec<T>(owner.Memory.Slice(1 * count, count));
            Item3 = new Vec<T>(owner.Memory.Slice(2 * count, count));
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
