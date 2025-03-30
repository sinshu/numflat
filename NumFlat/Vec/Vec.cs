using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NumFlat
{
    /// <summary>
    /// Represents a vector.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the vector.
    /// </typeparam>
    [CollectionBuilder(typeof(VectorBuilder), nameof(VectorBuilder.Create))]
    [StructLayout(LayoutKind.Auto)]
    public readonly partial struct Vec<T> : IReadOnlyList<T>, IFormattable where T : unmanaged, INumberBase<T>
    {
        private readonly int count;
        private readonly int stride;
        private readonly Memory<T> memory;

        /// <summary>
        /// Creates a new vector.
        /// </summary>
        /// <param name="count">
        /// The number of elements in the vector.
        /// </param>
        /// <param name="stride">
        /// The stride of the elements.
        /// This value indicates the difference between the index of adjacent elements in the <paramref name="memory"/>.
        /// If the length of the <paramref name="memory"/> matches <paramref name="count"/>
        /// and the elements are arranged consecutively without gaps in the <paramref name="memory"/>,
        /// then this value is 1.
        /// </param>
        /// <param name="memory">
        /// The storage of the elements in the vector.
        /// The length of the <paramref name="memory"/> must match
        /// <c><paramref name="stride"/> * (<paramref name="count"/> - 1) + 1</c>.
        /// </param>
        /// <remarks>
        /// This constructor does not allocate heap memory.
        /// The given <paramref name="memory"/> is directly used as the storage of the vector.
        /// </remarks>
        public Vec(int count, int stride, Memory<T> memory)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "'count' must be a positive value.");
            }

            if (stride <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "'stride' must be a positive value.");
            }

            if (memory.Length != stride * (count - 1) + 1)
            {
                throw new ArgumentException("'memory.Length' must match 'stride * (count - 1) + 1'.");
            }

            this.count = count;
            this.stride = stride;
            this.memory = memory;
        }

        /// <summary>
        /// Creates a zero vector.
        /// </summary>
        /// <param name="count">
        /// The number of elements in the vector.
        /// </param>
        /// <remarks>
        /// This constructor allocates heap memory to store the elements in the vector.
        /// </remarks>
        public Vec(int count) : this(count, 1, new T[count])
        {
        }

        /// <summary>
        /// Creates a vector from specified elements.
        /// </summary>
        /// <param name="memory">
        /// The storage of the elements in the vector.
        /// </param>
        /// <remarks>
        /// This constructor does not allocate heap memory.
        /// The given <paramref name="memory"/> is directly used as the storage of the vector.
        /// </remarks>
        public Vec(Memory<T> memory) : this(memory.Length, 1, memory)
        {
        }

        /// <summary>
        /// Fills the elements of the vector with a specified value.
        /// </summary>
        /// <param name="value">
        /// The value to assign to each element of the vector.
        /// </param>
        public readonly void Fill(T value)
        {
            if (this.count == 0)
            {
                throw new InvalidOperationException("Method call against an empty vector is not allowed.");
            }

            var span = memory.Span;
            var position = 0;
            while (position < span.Length)
            {
                span[position] = value;
                position += stride;
            }
        }

        /// <summary>
        /// Zero-clears the elements of the vector.
        /// </summary>
        public readonly void Clear()
        {
            if (this.count == 0)
            {
                throw new InvalidOperationException("Method call against an empty vector is not allowed.");
            }

            Fill(default);
        }

        /// <summary>
        /// Gets a subvector from the vector.
        /// </summary>
        /// <param name="startIndex">
        /// The starting position of the subvector in the vector.
        /// </param>
        /// <param name="count">
        /// The number of elements of the subvector.
        /// </param>
        /// <returns>
        /// The specified subvector in the vector.
        /// </returns>
        /// <remarks>
        /// This method does not allocate heap memory.
        /// The returned subvector will be a view of the original vector.
        /// </remarks>
        public readonly Vec<T> Subvector(int startIndex, int count)
        {
            if (this.count == 0)
            {
                throw new InvalidOperationException("Method call against an empty vector is not allowed.");
            }

            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "'startIndex' must be a non-negative value.");
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "'count' must be a positive value.");
            }

            if (startIndex + count > this.count)
            {
                throw new ArgumentOutOfRangeException("'startIndex + count' must be less than or equal to the length of the source vector.");
            }

            var stride = this.stride;
            var memory = this.memory.Slice(stride * startIndex, stride * (count - 1) + 1);
            return new Vec<T>(count, stride, memory);
        }

        /// <summary>
        /// Copies the elements of the vector into another vector.
        /// </summary>
        /// <param name="destination">
        /// The destination vector.
        /// </param>
        /// <remarks>
        /// The length of the vectors must match.
        /// </remarks>
        public readonly void CopyTo(in Vec<T> destination)
        {
            if (this.count == 0)
            {
                throw new InvalidOperationException("Method call against an empty vector is not allowed.");
            }

            ThrowHelper.ThrowIfDifferentSize(this, destination);

            if (this.count == destination.count &&
                this.stride == destination.stride &&
                this.memory.Equals(destination.memory))
            {
                return;
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

        /// <summary>
        /// Copies the elements of the vector into a <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="destination">
        /// The destination <see cref="Span{T}"/>.
        /// </param>
        /// <remarks>
        /// The length of the <see cref="Span{T}"/> must match the length of the vector.
        /// </remarks>
        public readonly void CopyTo(Span<T> destination)
        {
            if (this.count == 0)
            {
                throw new InvalidOperationException("Method call against an empty vector is not allowed.");
            }

            if (destination.Length != this.count)
            {
                throw new ArgumentException("The length of the destination must match the length of the vector.");
            }

            var st = this.memory.Span;
            var pt = 0;
            var pd = 0;
            while (pd < destination.Length)
            {
                destination[pd] = st[pt];
                pt += this.stride;
                pd++;
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified position in the vector.
        /// </summary>
        /// <param name="index">
        /// The index of the element.
        /// </param>
        /// <returns>
        /// The specified element.
        /// </returns>
        public readonly ref T this[int index]
        {
            get
            {
                if (this.count == 0)
                {
                    throw new InvalidOperationException("Method call against an empty vector is not allowed.");
                }

                if ((uint)index >= this.count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the vector length.");
                }

                return ref this.memory.Span[stride * index];
            }
        }

        /// <summary>
        /// Gets or the element at the specified position in the vector.
        /// </summary>
        /// <param name="index">
        /// The index of the element.
        /// </param>
        /// <returns>
        /// The specified element.
        /// </returns>
        readonly T IReadOnlyList<T>.this[int index]
        {
            get
            {
                if (this.count == 0)
                {
                    throw new InvalidOperationException("Method call against an empty vector is not allowed.");
                }

                if ((uint)index >= this.count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the vector length.");
                }

                return this.memory.Span[stride * index];
            }
        }

        /// <summary>
        /// Gets a subvector from the vector.
        /// </summary>
        /// <param name="range">
        /// The range of the elements.
        /// </param>
        /// <returns>
        /// The specified subvector in the vector.
        /// </returns>
        /// <remarks>
        /// This method does not allocate heap memory.
        /// The returned subvector will be a view of the original vector.
        /// </remarks>
        public readonly Vec<T> this[Range range]
        {
            get
            {
                var (offset, length) = range.GetOffsetAndLength(count);
                return Subvector(offset, length);
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the vector is empty.
        /// </summary>
        public readonly bool IsEmpty => count == 0;

        /// <summary>
        /// The number of elements in the vector.
        /// </summary>
        public readonly int Count => count;

        /// <summary>
        /// Gets the stride value for the storage.
        /// </summary>
        public readonly int Stride => stride;

        /// <summary>
        /// Gets the storage of the matrix.
        /// </summary>
        public readonly Memory<T> Memory => memory;
    }
}
