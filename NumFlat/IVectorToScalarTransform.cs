using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides common functionality across vector-to-scalar transform methods.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in vectors.
    /// </typeparam>
    public interface IVectorToScalarTransform<T> where T : unmanaged, INumberBase<T>
    {
        /// <summary>
        /// Transforms the source vector.
        /// </summary>
        /// <param name="source">
        /// The source vector to be transformed.
        /// </param>
        /// <returns>
        /// The transformed value.
        /// </returns>
        public T Transform(in Vec<T> source);

        /// <summary>
        /// Gets the required length of a source vector.
        /// </summary>
        public int SourceDimension { get; }
    }



    /// <summary>
    /// Provides common functionality across vector-to-scalar transform methods.
    /// </summary>
    public static class VectorToScalarTransform
    {
        internal static void ThrowIfInvalidSize<T>(IVectorToScalarTransform<T> method, in Vec<T> source, string name) where T : unmanaged, INumberBase<T>
        {
            if (source.Count != method.SourceDimension)
            {
                throw new ArgumentException($"The transform requires the length of the source vector to be {method.SourceDimension}, but was {source.Count}.", name);
            }
        }
    }
}
