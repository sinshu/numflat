using System;

namespace NumFlat.AudioFeatures
{
    /// <summary>
    /// Represents a frequency domain triangular filter for audio feature extraction.
    /// </summary>
    public sealed class TriangularFilter
    {
        private readonly int sampleRate;
        private readonly int fftLength;
        private readonly double lowerFrequency;
        private readonly double centerFrequency;
        private readonly double upperFrequency;

        private readonly int frequencyBinStartIndex;
        private readonly Vec<double> coefficients;

        /// <summary>
        /// Initializes a new triangular filter.
        /// </summary>
        /// <param name="sampleRate">
        /// The sample rate of the source signal.
        /// </param>
        /// <param name="fftLength">
        /// The FFT length used for analysis.
        /// </param>
        /// <param name="lowerFrequency">
        /// The lower frequency of the filter.
        /// </param>
        /// <param name="centerFrequency">
        /// The center frequency of the filter.
        /// </param>
        /// <param name="upperFrequency">
        /// The upper frequemcy of the filter.
        /// </param>
        public TriangularFilter(int sampleRate, int fftLength, double lowerFrequency, double centerFrequency, double upperFrequency)
        {
            if (sampleRate <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sampleRate), "The sample rate must be a positive value.");
            }

            if (fftLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fftLength), "The FFT length must be a positive value.");
            }

            if (lowerFrequency < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lowerFrequency), "The frequency value must be a non-negative value.");
            }

            if (centerFrequency < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(centerFrequency), "The frequency value must be a non-negative value.");
            }

            if (upperFrequency < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(upperFrequency), "The frequency value must be a non-negative value.");
            }

            if (lowerFrequency > centerFrequency)
            {
                throw new ArgumentException("The lower frequency must be smaller than or equal to the center frequency.");
            }

            if (upperFrequency < centerFrequency)
            {
                throw new ArgumentException("The upper frequency must be greater than or equal to the center frequency.");
            }

            this.sampleRate = sampleRate;
            this.fftLength = fftLength;
            this.lowerFrequency = lowerFrequency;
            this.centerFrequency = centerFrequency;
            this.upperFrequency = upperFrequency;

            var lowerW = lowerFrequency / sampleRate * fftLength;
            var centerW = centerFrequency / sampleRate * fftLength;
            var upperW = upperFrequency / sampleRate * fftLength;

            var frequencyBinStartIndex = (int)Math.Floor(lowerW);
            var frequencyBinEndIndex = (int)Math.Ceiling(upperW);
            var frequencyBinCount = frequencyBinEndIndex - frequencyBinStartIndex;

            if (frequencyBinEndIndex > fftLength / 2 + 1)
            {
                throw new ArgumentException("The upper frequency must be within the Nyquist frequency.");
            }

            var useLowerPart = centerW - lowerW > 0;
            var useUpperPart = upperW - centerW > 0;

            var coefficients = new Vec<double>(frequencyBinCount);
            var fc = coefficients.GetUnsafeFastIndexer();
            for (var i = 0; i < coefficients.Count; i++)
            {
                var w = frequencyBinStartIndex + i;

                if (useLowerPart && w <= (int)centerW)
                {
                    var x1 = Math.Max(w, lowerW);
                    var x2 = Math.Min(w + 1, centerW);
                    var y1 = (x1 - lowerW) / (centerW - lowerW);
                    var y2 = (x2 - lowerW) / (centerW - lowerW);
                    fc[i] += (y1 + y2) * (x2 - x1) / 2;
                }

                if (useUpperPart && w >= (int)centerW)
                {
                    var x1 = Math.Max(w, centerW);
                    var x2 = Math.Min(w + 1, upperW);
                    var y1 = (upperW - x1) / (upperW - centerW);
                    var y2 = (upperW - x2) / (upperW - centerW);
                    fc[i] += (y1 + y2) * (x2 - x1) / 2;
                }
            }

            this.frequencyBinStartIndex = frequencyBinStartIndex;
            this.coefficients = coefficients;
        }

        /// <summary>
        /// Gets the feature value from a spectrum.
        /// </summary>
        /// <param name="spectrum">
        /// The target spectrum.
        /// </param>
        /// <returns>
        /// The feature value.
        /// </returns>
        public double GetValue(in Vec<double> spectrum)
        {
            ThrowHelper.ThrowIfEmpty(spectrum, nameof(spectrum));
            PowerSpectrumFeatureExtraction.ThrowIfInvalidSize(fftLength, spectrum, nameof(spectrum));

            var ss = spectrum.Subvector(frequencyBinStartIndex, coefficients.Count).Memory.Span;
            var sc = coefficients.Memory.Span;
            var ps = 0;
            var pc = 0;
            var sum = 0.0;
            while (ps < ss.Length)
            {
                sum += ss[ps] * sc[pc];
                ps += spectrum.Stride;
                pc += coefficients.Stride;
            }
            return sum;
        }

        /// <summary>
        /// The sample rate of the source signal.
        /// </summary>
        public int SampleRate => sampleRate;

        /// <summary>
        /// The FFT length used for analysis.
        /// </summary>
        public int FftLength => fftLength;

        /// <summary>
        /// The lower frequency of the filter.
        /// </summary>
        public double LowerFrequency => lowerFrequency;

        /// <summary>
        /// The center frequency of the filter.
        /// </summary>
        public double CenterFrequency => centerFrequency;

        /// <summary>
        /// The upper frequency of the filter.
        /// </summary>
        public double UpperFrequency => upperFrequency;

        /// <summary>
        /// The frequency bin index corresponding to the starting point (lower frequency) of the filter.
        /// </summary>
        public int FrequencyBinStartIndex => frequencyBinStartIndex;

        /// <summary>
        /// The coefficients of the filter.
        /// </summary>
        public ref readonly Vec<double> Coefficients => ref coefficients;
    }
}
