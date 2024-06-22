using System;

namespace NumFlat
{
    public partial struct Vec<T>
    {
        /// <summary>
        /// Provides unsafe fast indexer for <see cref="Vec{T}"/>.
        /// </summary>
        public ref struct UnsafeFastIndexer
        {
            private readonly int stride;
            private readonly Span<T> memory;

            /// <summary>
            /// Creates a new unsafe fast indexer.
            /// </summary>
            /// <param name="source">
            /// The source vector.
            /// </param>
            public UnsafeFastIndexer(in Vec<T> source)
            {
                this.stride = source.stride;
                this.memory = source.memory.Span;
            }

            /// <summary>
            /// Gets the element at the specified position in the vector.
            /// </summary>
            /// <param name="index">
            /// The index of the element.
            /// </param>
            /// <returns>
            /// The specified element.
            /// </returns>
            public ref T this[int index] => ref memory[stride * index];
        }
    }
}
