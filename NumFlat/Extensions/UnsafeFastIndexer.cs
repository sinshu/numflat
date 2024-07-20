using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides unsafe fast indexer for <see cref="Vec{T}"/> and <see cref="Mat{T}"/>.
    /// </summary>
    public static class UnsafeFastIndexer
    {
        /// <summary>
        /// Gets an unsafe fast indexer.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="source">
        /// The source vector.
        /// </param>
        /// <returns>
        /// An unsafe fast indexer.
        /// </returns>
        public static Vec<T>.UnsafeFastIndexer GetUnsafeFastIndexer<T>(in this Vec<T> source) where T : unmanaged, INumberBase<T>
        {
            return new Vec<T>.UnsafeFastIndexer(source);
        }

        /// <summary>
        /// Gets an unsafe fast indexer.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="source">
        /// The source matrix.
        /// </param>
        /// <returns>
        /// An unsafe fast indexer.
        /// </returns>
        public static Mat<T>.UnsafeFastIndexer GetUnsafeFastIndexer<T>(in this Mat<T> source) where T : unmanaged, INumberBase<T>
        {
            return new Mat<T>.UnsafeFastIndexer(source);
        }
    }
}
