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
        /// An instance of <see cref="Vec{T}.RefEnumerator"/>.
        /// </returns>
        public RefEnumerator GetEnumerator() => new RefEnumerator(stride, memory.Span);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (stride == 1 && MemoryMarshal.TryGetArray<T>(memory, out var segment))
            {
                return segment.GetEnumerator();
            }

            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);



        /// <summary>
        /// Enumerates the elements in the vector.
        /// </summary>
        public ref struct RefEnumerator
        {
            private readonly int stride;
            private readonly Span<T> memory;
            private int position;

            internal RefEnumerator(int stride, Span<T> memory)
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



        /// <summary>
        /// Enumerates the elements in the vector.
        /// </summary>
        public class Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly int stride;
            private readonly Memory<T> memory;
            private int position;
            private T current;

            internal Enumerator(in Vec<T> vector)
            {
                this.stride = vector.stride;
                this.memory = vector.memory;
                this.position = -vector.stride;
                this.current = default;
            }

            /// <inheritdoc/>
            public void Dispose()
            {
            }

            /// <inheritdoc/>
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

            /// <inheritdoc/>
            public T Current => current;

            object? IEnumerator.Current
            {
                get => current;
            }

            void IEnumerator.Reset()
            {
                this.position = -stride;
                this.current = default;
            }
        }
    }
}
