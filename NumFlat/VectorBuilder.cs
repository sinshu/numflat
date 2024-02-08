using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides methods to create vectors.
    /// </summary>
    public static class VectorBuilder
    {
        /// <summary>
        /// Creates a new vector from the specified elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="elements">
        /// The elements for the new vector.
        /// </param>
        /// <returns>
        /// A new vector that contains the specified elements.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original storage.
        /// </remarks>
        public static Vec<T> Create<T>(ReadOnlySpan<T> elements) where T : unmanaged, INumberBase<T>
        {
            if (elements.Length == 0)
            {
                throw new ArgumentException("The sequence must contain at least one element.");
            }

            return new Vec<T>(elements.ToArray());
        }

        /// <summary>
        /// Creates a new vector which is filled with a specified value.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="count">
        /// The length of the vector.
        /// </param>
        /// <param name="value">
        /// The value to fill the new vector.
        /// </param>
        /// <returns>
        /// The new vector filled with the specified value.
        /// </returns>
        public static Vec<T> Fill<T>(int count, T value) where T : unmanaged, INumberBase<T>
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "The vector length must be greater than zero.");
            }

            var vec = new Vec<T>(count);
            vec.Fill(value);
            return vec;
        }

        /// <summary>
        /// Creates a new vector from a specified function.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="count">
        /// The length of the vector.
        /// </param>
        /// <param name="func">
        /// The function which generates the values for the new vector.
        /// The element index is given as an argument of the function.
        /// </param>
        /// <returns>
        /// The new vector filled with the specified value.
        /// </returns>
        public static Vec<T> FromFunc<T>(int count, Func<int, T> func) where T : unmanaged, INumberBase<T>
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "The vector length must be greater than zero.");
            }

            ThrowHelper.ThrowIfNull(func, nameof(func));

            var vec = new Vec<T>(count);
            var span = vec.Memory.Span;
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = func(i);
            }
            return vec;
        }

        /// <summary>
        /// Creates a new vector from the specified elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="elements">
        /// The elements for the new vector.
        /// </param>
        /// <returns>
        /// A new vector that contains the specified elements.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original storage.
        /// </remarks>
        public static Vec<T> ToVector<T>(this IEnumerable<T> elements) where T : unmanaged, INumberBase<T>
        {
            var array = elements.ToArray();
            if (array.Length == 0)
            {
                throw new ArgumentException("The sequence must contain at least one element.");
            }

            return new Vec<T>(array);
        }
    }
}
