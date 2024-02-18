using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides common functionality across vector-to-vector transform methods.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the matrix.
    /// </typeparam>
    public interface IVectorToVectorTransform<T> where T : unmanaged, INumberBase<T>
    {
        /// <summary>
        /// Transforms the source vector.
        /// </summary>
        /// <param name="source">
        /// The source vector to be transformed.
        /// </param>
        /// <param name="destination">
        /// The destination of the transformed vector.
        /// </param>
        public void Transform(in Vec<T> source, in Vec<T> destination);

        /// <summary>
        /// The required length of a source vector.
        /// </summary>
        public int SourceVectorLength { get; }

        /// <summary>
        /// The required length of a destination vector.
        /// </summary>
        public int DestinationVectorLength { get; }
    }



    /// <summary>
    /// Provides common functionality across vector-to-vector transform methods.
    /// </summary>
    public static class VectorToVectorTransform
    {
        /// <summary>
        /// Transforms the source vector.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="method">
        /// The transform method.
        /// </param>
        /// <param name="source">
        /// The source vector to be transformed.
        /// </param>
        /// <returns>
        /// The transformed vector.
        /// </returns>
        public static Vec<T> Transform<T>(this IVectorToVectorTransform<T> method, in Vec<T> source) where T : unmanaged, INumberBase<T>
        {
            ThrowIfInvalidSize(method, source, nameof(source));

            var destination = new Vec<T>(method.DestinationVectorLength);
            method.Transform(source, destination);
            return destination;
        }

        internal static void ThrowIfInvalidSize<T>(IVectorToVectorTransform<T> method, in Vec<T> source, string name) where T : unmanaged, INumberBase<T>
        {
            if (source.Count != method.SourceVectorLength)
            {
                throw new ArgumentException($"The transform requires the length of the source vector to be {method.SourceVectorLength}, but was {source.Count}.", name);
            }
        }

        internal static void ThrowIfInvalidSize<T>(IVectorToVectorTransform<T> method, in Vec<T> source, in Vec<T> destination, string sourceName, string destinationName) where T : unmanaged, INumberBase<T>
        {
            if (source.Count != method.SourceVectorLength)
            {
                throw new ArgumentException($"The transform requires the length of the source vector to be {method.SourceVectorLength}, but was {source.Count}.", sourceName);
            }

            if (destination.Count != method.DestinationVectorLength)
            {
                throw new ArgumentException($"The transform requires the length of the destination vector to be {method.DestinationVectorLength}, but was {destination.Count}.", destinationName);
            }
        }
    }
}
