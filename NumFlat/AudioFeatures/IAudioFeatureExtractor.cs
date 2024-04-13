using System;

namespace NumFlat.AudioFeatures
{
    /// <summary>
    /// Provides common functionality across audio feature extraction methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAudioFeatureExtractor<T>
    {
        /// <summary>
        /// Transforms the source object to a feature vector.
        /// </summary>
        /// <param name="source">
        /// The source object.
        /// </param>
        /// <param name="destination">
        /// The destination of the feature vector.
        /// </param>
        public void Transform(in T source, in Vec<double> destination);

        /// <summary>
        /// Gets the length of feature vectors.
        /// </summary>
        public int FeatureLength { get; }
    }



    /// <summary>
    /// Provides common functionality across audio feature extraction methods.
    /// </summary>
    public static class AudioFeatureExtractor
    {
        /// <summary>
        /// Transforms the source object to a feature vector.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the source object.
        /// </typeparam>
        /// <param name="method">
        /// The feature extraction method.
        /// </param>
        /// <param name="source">
        /// The source object to be transformed.
        /// </param>
        /// <returns>
        /// The feature vector.
        /// </returns>
        public static Vec<double> Transform<T>(this IAudioFeatureExtractor<T> method, in T source)
        {
            var destination = new Vec<double>(method.FeatureLength);
            method.Transform(source, destination);
            return destination;
        }
    }
}
