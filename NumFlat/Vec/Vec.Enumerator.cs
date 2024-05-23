using System;
using System.Collections;
using System.Collections.Generic;

namespace NumFlat
{
    public partial struct Vec<T>
    {
        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);



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
    }
}
