using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NumFlat
{
    public partial struct Vec<T>
    {
        /// <summary>
        /// Gets an enumerator.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="Vec{T}.Enumerator"/>.
        /// </returns>
        public Enumerator GetEnumerator() => new Enumerator(stride, memory.Span);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (MemoryMarshal.TryGetArray<T>(memory, out var segment))
            {
                if (stride == 1)
                {
                    return segment.GetEnumerator();
                }
                else
                {
                    return new ArraySegmentEnumerator(stride, segment);
                }
            }

            return new MemoryEnumerator(stride, memory);
        }

        IEnumerator IEnumerable.GetEnumerator() => new MemoryEnumerator(stride, memory);



        /// <summary>
        /// Enumerates the elements in the vector.
        /// </summary>
        public ref struct Enumerator
        {
            private readonly int stride;
            private readonly Span<T> memory;
            private int position;

            internal Enumerator(int stride, Span<T> memory)
            {
                this.stride = stride;
                this.memory = memory;
                this.position = -stride;
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
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Gets the current value.
            /// </summary>
            public ref T Current => ref memory[position];
        }



        private class MemoryEnumerator : IEnumerator<T>, IEnumerator
        {
            private readonly int stride;
            private readonly Memory<T> memory;
            private int position;
            private T current;

            internal MemoryEnumerator(int stride, Memory<T> memory)
            {
                this.stride = stride;
                this.memory = memory;
                this.position = -stride;
                this.current = default;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                position += stride;
                if (position < memory.Length)
                {
                    current = memory.Span[position];
                    return true;
                }
                else
                {
                    current = default;
                    return false;
                }
            }

            public T Current => current;

            object? IEnumerator.Current => current;

            void IEnumerator.Reset()
            {
                this.position = -stride;
                this.current = default;
            }
        }



        private class ArraySegmentEnumerator : IEnumerator<T>, IEnumerator
        {
            private readonly int stride;
            private readonly ArraySegment<T> memory;
            private int position;
            private T current;

            internal ArraySegmentEnumerator(int stride, ArraySegment<T> memory)
            {
                this.stride = stride;
                this.memory = memory;
                this.position = -stride;
                this.current = default;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                position += stride;
                if (position < memory.Count)
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

            public T Current => current;

            object? IEnumerator.Current => current;

            void IEnumerator.Reset()
            {
                this.position = -stride;
                this.current = default;
            }
        }
    }
}
