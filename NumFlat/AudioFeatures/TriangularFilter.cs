using System;

namespace NumFlat.AudioFeatures
{
    public sealed class TriangularFilter
    {
        private readonly int sampleRate;
        private readonly int fftLength;
        private readonly double lowerFrequency;
        private readonly double centerFrequency;
        private readonly double upperFrequency;

        private readonly int startFrequencyBinIndex;
        private readonly Vec<double> coefficients;

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

            var startFrequencyBinIndex = (int)Math.Floor(lowerW);
            var endFrequencyBinIndex = (int)Math.Ceiling(upperW);
            var frequencyBinCount = endFrequencyBinIndex - startFrequencyBinIndex;

            var useLowerPart = centerW - lowerW > 0;
            var useUpperPart = upperW - centerW > 0;

            var coefficients = new Vec<double>(frequencyBinCount);
            var fc = coefficients.GetUnsafeFastIndexer();
            for (var i = 0; i < coefficients.Count; i++)
            {
                var w = startFrequencyBinIndex + i;

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

            this.startFrequencyBinIndex = startFrequencyBinIndex;
            this.coefficients = coefficients;
        }

        public int SampleRate => sampleRate;
        public int FftLength => fftLength;
        public double LowerFrequency => lowerFrequency;
        public double CenterFrequency => centerFrequency;
        public double UpperFrequency => upperFrequency;
        public int StartFrequencyBinIndex => startFrequencyBinIndex;
        public ref readonly Vec<double> Coefficients => ref coefficients;
    }
}
