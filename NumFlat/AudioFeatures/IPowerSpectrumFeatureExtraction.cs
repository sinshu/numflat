using System;

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
        /// <param name="spectrum">
        /// The source power spectrum to be transformed.
        /// </param>
        /// <param name="destination">
        /// The destination of the feature vector.
        /// </param>
        public void Transform(in Vec<double> spectrum, in Vec<double> destination);

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
        /// Transforms the source object to a feature vector.
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
            var destination = new Vec<double>(method.FeatureLength);
            method.Transform(source, destination);
            return destination;
        }
    }
}
