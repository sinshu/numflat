using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace NumFlat
{
    public partial struct Vec<T> : IReadOnlyList<T> where T : unmanaged, INumberBase<T>
    {
        private readonly int count;
        private readonly int stride;
        private readonly Memory<T> memory;

        public Vec(int count, int stride, Memory<T> memory)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "The length of the vector must be a positive integer.");
            }

            if (stride <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "The stride must be a positive integer.");
            }

            if (memory.Length != stride * (count - 1) + 1)
            {
                throw new ArgumentException("The length of the memory must match `stride * (count - 1) + 1`.");
            }

            this.count = count;
            this.stride = stride;
            this.memory = memory;
        }

        public Vec(int count) : this(count, 1, new T[count])
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "The length of the vector must be a positive integer.");
            }
        }

        public Vec<T> Subvector(int index, int count)
        {
            var stride = this.stride;
            var memory = this.memory.Slice(stride * index, stride * (count - 1) + 1);
            return new Vec<T>(count, stride, memory);
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

        public T this[int index]
        {
            get => memory.Span[stride * index];
            set => memory.Span[stride * index] = value;
        }

        public int Count => count;
        public int Stride => stride;
        public Memory<T> Memory => memory;



        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly int stride;
            private readonly Memory<T> memory;
            private int position;
            private T current;

            internal Enumerator(Vec<T> vector)
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
