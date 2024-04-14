using System;
using System.Collections.Generic;

namespace NumFlat.AudioFeatures
{
    /// <summary>
    /// Provides a filter bank feature extractor.
    /// </summary>
    public sealed class FilterBank : IPowerSpectrumFeatureExtraction
    {
        private int sampleRate;
        private int fftLength;
        private double minFrequency;
        private double maxFrequency;
        private FrequencyScale scale;

        private TriangularFilter[] filters;

        /// <summary>
        /// Initializes a filter bank feature extractor.
        /// </summary>
        /// <param name="sampleRate">
        /// The sample rate of the source signal.
        /// </param>
        /// <param name="fftLength">
        /// The FFT length used for analysis.
        /// </param>
        /// <param name="minFrequency">
        /// The minimum frequency.
        /// </param>
        /// <param name="maxFrequency">
        /// The maximum frequency.
        /// </param>
        /// <param name="filterCount">
        /// The number of filters.
        /// </param>
        /// <param name="scale">
        /// The frequency scale to be used.
        /// </param>
        public FilterBank(int sampleRate, int fftLength, double minFrequency, double maxFrequency, int filterCount, FrequencyScale scale)
        {
            if (sampleRate <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sampleRate), "The sample rate must be a positive value.");
            }

            if (fftLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fftLength), "The FFT length must be a positive value.");
            }

            if (minFrequency < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minFrequency), "The frequency value must be a non-negative value.");
            }

            if (maxFrequency < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxFrequency), "The frequency value must be a non-negative value.");
            }

            this.sampleRate = sampleRate;
            this.fftLength = fftLength;
            this.minFrequency = minFrequency;
            this.maxFrequency = maxFrequency;
            this.scale = scale;

            double[] frequencies;
            switch (scale)
            {
                case FrequencyScale.Linear:
                    frequencies = GetFrequenciesLinear(minFrequency, maxFrequency, filterCount + 2);
                    break;
                case FrequencyScale.Mel:
                    frequencies = GetFrequenciesMel(minFrequency, maxFrequency, filterCount + 2);
                    break;
                default:
                    throw new ArgumentException("Invalid enum value.", nameof(scale));
            }

            var filters = new TriangularFilter[filterCount];
            for (var i = 0; i < filters.Length; i++)
            {
                var lower = frequencies[i];
                var center = frequencies[i + 1];
                var upper = frequencies[i + 2];
                filters[i] = new TriangularFilter(sampleRate, fftLength, lower, center, upper);
            }

            this.filters = filters;
        }

        private static double[] GetFrequenciesLinear(double min, double max, int count)
        {
            var frequencies = new double[count];
            for (var i = 0; i < count; i++)
            {
                frequencies[i] = min + (max - min) * i / (count - 1);
            }
            return frequencies;
        }

        private static double[] GetFrequenciesMel(double min, double max, int count)
        {
            var melMin = LinearToMel(min);
            var melMax = LinearToMel(max);
            var frequencies = GetFrequenciesLinear(melMin, melMax, count);
            for (var i = 0; i < frequencies.Length; i++)
            {
                frequencies[i] = MelToLinear(frequencies[i]);
            }
            return frequencies;
        }

        private static double LinearToMel(double x)
        {
            return 1127 * Math.Log(1 + x / 700);
        }

        private static double MelToLinear(double x)
        {
            return 700 * (Math.Exp(x / 1127) - 1);
        }

        /// <inheritdoc/>
        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            PowerSpectrumFeatureExtraction.ThrowIfInvalidSize(this, source, destination, nameof(source), nameof(destination));

            var fd = destination.GetUnsafeFastIndexer();
            for (var i = 0; i < filters.Length; i++)
            {
                fd[i] = filters[i].GetValue(source);
            }
        }

        /// <inheritdoc/>
        public int SampleRate => sampleRate;

        /// <inheritdoc/>
        public int FftLength => fftLength;

        /// <summary>
        /// Gets the minimum frequency.
        /// </summary>
        public double MinFrequency => minFrequency;

        /// <summary>
        /// Gets the maximum frequency.
        /// </summary>
        public double MaxFrequency => maxFrequency;

        /// <summary>
        /// Gets the frequency scale.
        /// </summary>
        public FrequencyScale Scale => scale;

        /// <summary>
        /// Gets the filters for feature extraction.
        /// </summary>
        public IReadOnlyList<TriangularFilter> Filters => filters;

        /// <inheritdoc/>
        public int FeatureLength => filters.Length;
    }
}
