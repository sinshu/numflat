using System;
using System.Numerics;

namespace NumFlat.AudioFeatures
{
    /// <summary>
    /// Provides common functionality across power spectrum feature extraction methods.
    /// </summary>
    public interface IPowerSpectrumFeatureExtraction
    {
        /// <summary>
        /// Transforms a power spectrum to a feature vector.
        /// </summary>
        /// <param name="source">
        /// The source power spectrum to be transformed.
        /// The length of the spectrum must match the FFT length or the half of the FFT length plus one.
        /// </param>
        /// <param name="destination">
        /// The destination of the feature vector.
        /// </param>
        public void Transform(in Vec<double> source, in Vec<double> destination);

        /// <summary>
        /// Gets the sample rate of the source signal.
        /// </summary>
        public int SampleRate { get; }

        /// <summary>
        /// Gets the FFT length used for analysis.
        /// </summary>
        public int FftLength { get; }

        /// <summary>
        /// Gets the length of feature vectors.
        /// </summary>
        public int FeatureLength { get; }
    }



    /// <summary>
    /// Provides common functionality across power spectrum feature extraction methods.
    /// </summary>
    public static class PowerSpectrumFeatureExtraction
    {
        /// <summary>
        /// Transforms a power spectrum to a feature vector.
        /// </summary>
        /// <param name="method">
        /// The feature extraction method.
        /// </param>
        /// <param name="source">
        /// The source power spectrum to be transformed.
        /// </param>
        /// <returns>
        /// The feature vector.
        /// </returns>
        public static Vec<double> Transform(this IPowerSpectrumFeatureExtraction method, in Vec<double> source)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowIfInvalidSize(method.FftLength, source, nameof(source));

            var destination = new Vec<double>(method.FeatureLength);
            method.Transform(source, destination);
            return destination;
        }

        internal static void ThrowIfInvalidSize(IPowerSpectrumFeatureExtraction method, in Vec<double> source, in Vec<double> destination, string sourceName, string destinationName)
        {
            ThrowIfInvalidSize(method.FftLength, source, sourceName);

            if (destination.Count != method.FeatureLength)
            {
                throw new ArgumentException($"The feature extraction requires the length of the destination vector to be {method.FeatureLength}, but was {destination.Count}.", destinationName);
            }
        }

        internal static void ThrowIfInvalidSize(int fftLength, in Vec<double> source, string name)
        {
            var expected1 = fftLength;
            var expected2 = fftLength / 2 + 1;
            if (source.Count != expected1 && source.Count != expected2)
            {
                throw new ArgumentException($"The feature extraction requires the length of the spectrum to be {expected1} (FFT length) or {expected2} (half of the FFT length plus one), but was {source.Count}.", name);
            }
        }
    }
}
