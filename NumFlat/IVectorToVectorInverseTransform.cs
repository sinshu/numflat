using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides common functionality across vector-to-vector inverse transform methods.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in vectors.
    /// </typeparam>
    public interface IVectorToVectorInverseTransform<T> : IVectorToVectorTransform<T> where T : unmanaged, INumberBase<T>
    {
        /// <summary>
        /// Inverse transforms the source vector.
        /// </summary>
        /// <param name="source">
        /// The source vector to be inverse transformed.
        /// </param>
        /// <param name="destination">
        /// The destination of the inverse transformed vector.
        /// </param>
        public void InverseTransform(in Vec<T> source, in Vec<T> destination);
    }



    /// <summary>
    /// Provides common functionality across vector-to-vector inverse transform methods.
    /// </summary>
    public static class VectorToVectorInverseTransform
    {
        /// <summary>
        /// Inverse transforms the source vector.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="method">
        /// The inverse transform method.
        /// </param>
        /// <param name="source">
        /// The source vector to be inverse transformed.
        /// </param>
        /// <returns>
        /// The inverse transformed vector.
        /// </returns>
        public static Vec<T> InverseTransform<T>(this IVectorToVectorInverseTransform<T> method, in Vec<T> source) where T : unmanaged, INumberBase<T>
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowIfInvalidSize(method, source, nameof(source));

            var destination = new Vec<T>(method.SourceDimension);
            method.InverseTransform(source, destination);
            return destination;
        }

        internal static void ThrowIfInvalidSize<T>(IVectorToVectorInverseTransform<T> method, in Vec<T> source, string name) where T : unmanaged, INumberBase<T>
        {
            if (source.Count != method.DestinationDimension)
            {
                throw new ArgumentException($"The inverse transform requires the length of the source vector to be {method.DestinationDimension}, but was {source.Count}.", name);
            }
        }

        internal static void ThrowIfInvalidSize<T>(IVectorToVectorInverseTransform<T> method, in Vec<T> source, in Vec<T> destination, string sourceName, string destinationName) where T : unmanaged, INumberBase<T>
        {
            if (source.Count != method.DestinationDimension)
            {
                throw new ArgumentException($"The inverse transform requires the length of the source vector to be {method.DestinationDimension}, but was {source.Count}.", sourceName);
            }

            if (destination.Count != method.SourceDimension)
            {
                throw new ArgumentException($"The inverse transform requires the length of the destination vector to be {method.SourceDimension}, but was {destination.Count}.", destinationName);
            }
        }
    }
}
