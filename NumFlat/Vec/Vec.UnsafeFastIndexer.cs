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

            /// <summary>
            /// Gets an enumerator.
            /// </summary>
            /// <returns>
            /// An instance of <see cref="Vec{T}.FastEnumerator"/>.
            /// </returns>
            public FastEnumerator GetEnumerator() => new FastEnumerator(stride, memory);
        }



        /// <summary>
        /// Provides fast enumerator for <see cref="Vec{T}"/>.
        /// </summary>
        public ref struct FastEnumerator
        {
            private readonly int stride;
            private readonly Span<T> memory;
            private int position;
            private T current;

            internal FastEnumerator(int stride, Span<T> memory)
            {
                this.stride = stride;
                this.memory = memory;
                this.position = -stride;
                this.current = default;
            }

            /// <summary>
            /// Stops the enumerator.
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// Advances the enumerator.
            /// </summary>
            /// <returns>
            /// True if the enumerator has the next value.
            /// </returns>
            public bool MoveNext()
            {
                position += stride;
                if (position < memory.Length)
                {
                    current = memory[position];
                    return true;
                }
                else
                {
                    current = default;
                    return false;
                }
            }

            /// <summary>
            /// Gets the current value.
            /// </summary>
            public T Current => current;
        }
    }
}
