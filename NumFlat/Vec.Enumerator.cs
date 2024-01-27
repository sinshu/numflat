using System;
using System.Collections;
using System.Collections.Generic;

namespace NumFlat
{
    public partial struct Vec<T>
    {
        public IEnumerator<T> GetEnumerator() => new Enumerator(ref this);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();



        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly int stride;
            private readonly Memory<T> memory;
            private int position;
            private T current;

            internal Enumerator(ref Vec<T> vector)
            {
                this.stride = vector.stride;
                this.memory = vector.memory;
                this.position = -vector.stride;
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
