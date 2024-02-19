using System;
using System.Collections;
using System.Collections.Generic;

namespace NumFlat
{
    public partial struct Vec<T>
    {
        internal ref struct UnsafeFastIndexer
        {
            private readonly int stride;
            private readonly Span<T> memory;

            internal UnsafeFastIndexer(in Vec<T> source)
            {
                this.stride = source.stride;
                this.memory = source.memory.Span;
            }

            public ref T this[int index] => ref memory[stride * index];
        }
    }
}
