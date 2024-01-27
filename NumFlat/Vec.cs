﻿using System;
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
        }

        public Vec(Memory<T> memory) : this(memory.Length, 1, memory)
        {
        }

        public void Fill(T value)
        {
            var span = memory.Span;
            var position = 0;
            while (position < span.Length)
            {
                span[position] = value;
                position += stride;
            }
        }

        public void Clear()
        {
            Fill(default);
        }

        public Vec<T> Subvector(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "The index must be a positive integer.");
            }

            if (index + count > this.count)
            {
                throw new ArgumentOutOfRangeException("`index + count` must be less than or equal to the length of the vector.");
            }

            var stride = this.stride;
            var memory = this.memory.Slice(stride * index, stride * (count - 1) + 1);
            return new Vec<T>(count, stride, memory);
        }

        public void CopyTo(Vec<T> destination)
        {
            if (destination.count != this.count)
            {
                throw new ArgumentException("The source and destination must be the same length.");
            }

            var st = this.memory.Span;
            var sd = destination.memory.Span;
            var pt = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = st[pt];
                pt += this.stride;
                pd += destination.stride;
            }
        }

        public T this[int index]
        {
            get
            {
                if ((uint)index >= count)
                {
                    throw new IndexOutOfRangeException("The index must be within the length of the vector.");
                }

                return memory.Span[stride * index];
            }

            set
            {
                if ((uint)index >= count)
                {
                    throw new IndexOutOfRangeException("The index must be within the length of the vector.");
                }

                memory.Span[stride * index] = value;
            }
        }

        public int Count => count;
        public int Stride => stride;
        public Memory<T> Memory => memory;
    }
}
