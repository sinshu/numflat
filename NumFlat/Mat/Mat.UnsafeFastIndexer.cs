using System;
using System.Collections;
using System.Collections.Generic;

namespace NumFlat
{
    public partial struct Mat<T>
    {
        internal ref struct UnsafeFastIndexer
        {
            private readonly int stride;
            private readonly Span<T> memory;

            internal UnsafeFastIndexer(in Mat<T> source)
            {
                this.stride = source.stride;
                this.memory = source.memory.Span;
            }

            public ref T this[int row, int col] => ref memory[stride * col + row];
        }
    }
}
