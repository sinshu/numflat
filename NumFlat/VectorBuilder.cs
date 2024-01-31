using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides methods to create vectors.
    /// </summary>
    public static class VectorBuilder
    {
        /// <summary>
        /// Create a new vector from the specified elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="elements">
        /// The elements in the vector.
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
                throw new ArgumentException("The number of elements must be greater than or equal to one.");
            }

            return new Vec<T>(elements.ToArray());
        }
    }
}
