using System;
using System.Collections;
using System.Collections.Generic;

namespace NumFlat
{
    public partial struct Mat<T>
    {
        /// <summary>
        /// Provides unsafe fast indexer for <see cref="Mat{T}"/>.
        /// </summary>
        public ref struct UnsafeFastIndexer
        {
            private readonly int stride;
            private readonly Span<T> memory;

            /// <summary>
            /// Creates a new unsafe fast indexer.
            /// </summary>
            /// <param name="source">
            /// The source matrix.
            /// </param>
            public UnsafeFastIndexer(in Mat<T> source)
            {
                this.stride = source.stride;
                this.memory = source.memory.Span;
            }

            /// <summary>
            /// Gets the element at the specified position in the matrix.
            /// </summary>
            /// <param name="row">
            /// The row index of the element.
            /// </param>
            /// <param name="col">
            /// The column index of the element.
            /// </param>
            /// <returns>
            /// The specified element.
            /// </returns>
            public ref T this[int row, int col] => ref memory[stride * col + row];
        }
    }
}
