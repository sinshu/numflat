using System;
using System.Collections;
using System.Collections.Generic;

namespace NumFlat
{
    public partial struct Vec<T>
    {
        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();



        /// <summary>
        /// Enumerates the elements in the vector.
        /// </summary>
        public struct Enumerator : IEnumerator<T>, IEnumerator
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



        /// <summary>
        /// Provides fast enumeration.
        /// </summary>
        public ref struct FastEnumerable
        {
            ref readonly Vec<T> vector;

            /// <summary>
            /// Initializes a new enumeration.
            /// </summary>
            /// <param name="vector">
            /// The vector to be enumerated.
            /// </param>
            public FastEnumerable(in Vec<T> vector)
            {
                this.vector = ref vector;
            }

            /// <summary>
            /// Gets an enumerator.
            /// </summary>
            /// <returns>
            /// A new enumerator.
            /// </returns>
            public FastEnumerator GetEnumerator() => new FastEnumerator(vector);
        }



        /// <summary>
        /// Enumerates the elements in the vector.
        /// </summary>
        public ref struct FastEnumerator
        {
            private readonly int stride;
            private readonly Span<T> memory;
            private int position;
            private T current;

            internal FastEnumerator(in Vec<T> vector)
            {
                this.stride = vector.stride;
                this.memory = vector.memory.Span;
                this.position = -vector.stride;
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
