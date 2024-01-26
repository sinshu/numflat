using System;
using System.Numerics;

namespace NumFlat
{
    public struct Vec<T> where T : INumberBase<T>
    {
        private int count;
        private int stride;
        private Memory<T> memory;

        public Vec(int count, int stride, Memory<T> memory)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "The length of the vector must be a positive integer.");
            }

            if (stride <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "The stride value must be a positive integer.");
            }

            if (memory.Length != stride * (count - 1) + 1)
            {
                throw new ArgumentException("The length of the memory must match stride * (count - 1) + 1.");
            }

            this.count = count;
            this.stride = stride;
            this.memory = memory;
        }

        public Vec(int count) : this(count, 1, new T[count])
        {
        }

        public T this[int index]
        {
            get => memory.Span[stride * index];
            set => memory.Span[stride * index] = value;
        }

        public int Count => count;
        public int Stride => stride;
        public Memory<T> Memory => memory;
    }
}
