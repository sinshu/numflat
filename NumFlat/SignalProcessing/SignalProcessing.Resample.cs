using System;
using System.Runtime.CompilerServices;

namespace NumFlat.SignalProcessing
{
    public static partial class SignalProcessing
    {
        /// <summary>
        /// The given sequence is resampled using sinc function interpolation.
        /// The interpolation process is performed using Lanczos resampling.
        /// </summary>
        /// <param name="source">
        /// The source signal to be resampled.
        /// </param>
        /// <param name="destination">
        /// The destination of the resampled signal.
        /// </param>
        /// <param name="p">
        /// The upsampling factor.
        /// </param>
        /// <param name="q">
        /// The downsampling factor.
        /// </param>
        /// <param name="a">
        /// Specifies the quality of the Lanczos resampling.
        /// Higher values result in higher quality but require more processing time.
        /// </param>
        public static void Resample(in Vec<double> source, in Vec<double> destination, int p, int q, int a = 10)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            ValidateAndReduceResamplingFactors(ref p, ref q, a);

            ResampleCore(source, destination, p, q, a);
        }

        /// <summary>
        /// The given sequence is resampled using sinc function interpolation.
        /// The interpolation process is performed using Lanczos resampling.
        /// </summary>
        /// <param name="source">
        /// The source signal to be resampled.
        /// </param>
        /// <param name="p">
        /// The upsampling factor.
        /// </param>
        /// <param name="q">
        /// The downsampling factor.
        /// </param>
        /// <param name="a">
        /// Specifies the quality of the Lanczos resampling.
        /// Higher values result in higher quality but require more processing time.
        /// </param>
        /// <returns>
        /// The resampled signal.
        /// </returns>
        public static Vec<double> Resample(in this Vec<double> source, int p, int q, int a = 10)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));

            ValidateAndReduceResamplingFactors(ref p, ref q, a);

            var length = (int)Math.Ceiling((double)source.Count * p / q);
            var destination = new Vec<double>(length);
            ResampleCore(source, destination, p, q, a);
            return destination;
        }

        private static void ValidateAndReduceResamplingFactors(ref int p, ref int q, int a)
        {
            if (p < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(p), "The upsampling factor must be greater than or equal to one.");
            }

            if (q < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(q), "The downsampling factor must be greater than or equal to one.");
            }

            if (a < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(a), "The quality factor of the Lanczos resampling must be greater than or equal to one.");
            }

            var greatestCommonDivisor = Special.GreatestCommonDivisor(p, q);
            p /= greatestCommonDivisor;
            q /= greatestCommonDivisor;
        }

        private static void ResampleCore(in Vec<double> source, in Vec<double> destination, int p, int q, int a)
        {
            using var usrc = source.EnsureContiguous(true);
            ref readonly var src = ref usrc.Item;

            using var udst = destination.EnsureContiguous(false);
            ref readonly var dst = ref udst.Item;

            if (q == 1 && p > 1)
            {
                FastUpsample(src, dst, p, a);
            }
            else if (p == 1 && q > 1)
            {
                FastDownsample(src, dst, q, a);
            }
            else
            {
                NaiveResample(src.Memory.Span, dst.Memory.Span, p, q, a);
            }
        }

        private static void FastUpsample(in Vec<double> source, in Vec<double> destination, int p, int a)
        {
            // The Lanczos window is zero at a * p samples from its center, so the zero endpoints can be omitted.
            var kernelRadius = checked(a * p - 1);
            var kernelLength = checked(2 * kernelRadius + 1);

            using var ukernel = new TemporalVector<double>(kernelLength);
            ref readonly var kernel = ref ukernel.Item;
            FillLanczosKernel(kernel, p, a, 1.0);

            // Insert p - 1 zeros between adjacent input samples. No trailing zeros are needed after the last sample.
            var expandedLength = checked((source.Count - 1) * p + 1);
            using var uexpanded = new TemporalVector<double>(expandedLength);
            ref readonly var expanded = ref uexpanded.Item;
            expanded.Clear();

            var fsource = source.GetUnsafeFastIndexer();
            var fexpanded = expanded.GetUnsafeFastIndexer();
            for (var i = 0; i < source.Count; i++)
            {
                fexpanded[i * p] = fsource[i];
            }

            // A full convolution contains kernelRadius samples before output position zero.
            var naturalConvolutionLength = checked(expandedLength + kernelLength - 1);

            // Only the part at and after the kernel center corresponds to positions requested by Resample.
            var availableOutputLength = naturalConvolutionLength - kernelRadius;

            // A caller may provide a destination that extends beyond the finite convolution result.
            var copyLength = Math.Min(destination.Count, availableOutputLength);

            // Convolve only far enough to cover the centered samples that will actually be copied.
            var convolutionLength = checked(kernelRadius + copyLength);

            using var uconvolution = new TemporalVector<double>(convolutionLength);
            ref readonly var convolution = ref uconvolution.Item;
            expanded.Convolve(kernel, convolution);

            // Start from an all-zero destination so an overlong destination keeps the zero-extension semantics.
            destination.Clear();

            // Skip the causal prefix introduced by storing the zero-centered kernel in an ordinary vector.
            var centered = convolution.Subvector(kernelRadius, copyLength);

            // Copying through Vec handles the destination layout without using its checked indexer in a loop.
            centered.CopyTo(destination.Subvector(0, copyLength));
        }

        private static void FastDownsample(in Vec<double> source, in Vec<double> destination, int q, int a)
        {
            // The anti-aliasing filter reaches its first zero at a * q samples from the center.
            var kernelRadius = checked(a * q - 1);
            var kernelLength = checked(2 * kernelRadius + 1);

            using var ukernel = new TemporalVector<double>(kernelLength);
            ref readonly var kernel = ref ukernel.Item;
            FillLanczosKernel(kernel, q, a, 1.0 / q);

            // The last nonzero convolution sample lies kernelRadius positions after the final source sample.
            var lastNaturalPosition = checked(source.Count + kernelRadius - 1);

            // Count how many q-spaced positions, beginning at zero, still intersect that finite result.
            var availableOutputLength = checked(lastNaturalPosition / q + 1);

            // A caller may request fewer samples, or may extend the destination into the all-zero tail.
            var copyLength = Math.Min(destination.Count, availableOutputLength);

            // The centered sample for output i is stored at kernelRadius + i * q in the causal convolution.
            var lastConvolutionIndex = checked(kernelRadius + (copyLength - 1) * q);

            // Convolve only through that final sample; later convolution values are never observed.
            var convolutionLength = checked(lastConvolutionIndex + 1);

            using var uconvolution = new TemporalVector<double>(convolutionLength);
            ref readonly var convolution = ref uconvolution.Item;
            source.Convolve(kernel, convolution);

            // Clear first so samples beyond the finite convolution remain exactly zero.
            destination.Clear();

            // These fast indexers avoid the relatively expensive checked Vec indexer in the decimation loop.
            var fconvolution = convolution.GetUnsafeFastIndexer();
            var fdestination = destination.GetUnsafeFastIndexer();
            for (var i = 0; i < copyLength; i++)
            {
                // Adding kernelRadius converts the zero-centered filter coordinate to the causal convolution index.
                var convolutionIndex = kernelRadius + i * q;

                // Taking every q-th centered sample performs the decimation after low-pass filtering.
                fdestination[i] = fconvolution[convolutionIndex];
            }
        }

        private static void NaiveResample(ReadOnlySpan<double> source, Span<double> destination, int p, int q, int a)
        {
            if (p > q)
            {
                NaiveUpsample(source, destination, p, q, a);
            }
            else if (q > p)
            {
                NaiveDownsample(source, destination, p, q, a);
            }
            else
            {
                if (destination.Length <= source.Length)
                {
                    source.Slice(0, destination.Length).CopyTo(destination);
                }
                else
                {
                    source.CopyTo(destination.Slice(0, source.Length));
                    destination.Slice(source.Length).Clear();
                }
            }
        }

        private static void NaiveUpsample(ReadOnlySpan<double> source, Span<double> destination, int p, int q, int a)
        {
            var qdp = (double)q / p;
            for (var i = 0; i < destination.Length; i++)
            {
                var position = i * qdp;
                destination[i] = NaiveResampleSinglePoint(source, position, 1, a);
            }
        }

        private static void NaiveDownsample(ReadOnlySpan<double> source, Span<double> destination, int p, int q, int a)
        {
            var pdq = (double)p / q;
            var qdp = (double)q / p;
            for (var i = 0; i < destination.Length; i++)
            {
                var position = i * qdp;
                destination[i] = pdq * NaiveResampleSinglePoint(source, position, qdp, a);
            }
        }

        private static double NaiveResampleSinglePoint(ReadOnlySpan<double> source, double position, double sincFactor, int a)
        {
            var left = (int)Math.Floor(position - a * sincFactor) + 1;
            var right = (int)Math.Ceiling(position + a * sincFactor);

            if (left < 0)
            {
                left = 0;
            }
            if (right > source.Length)
            {
                right = source.Length;
            }

            var u = Math.PI / sincFactor;
            var v = u / a;

            var sum = 0.0;
            for (var i = left; i < right; i++)
            {
                var x = i - position;
                sum += source[i] * Sinc(u * x) * Sinc(v * x);
            }
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Sinc(double x)
        {
            if (Math.Abs(x) < 1.0E-15)
            {
                return 1.0;
            }
            else
            {
                return Math.Sin(x) / x;
            }
        }

        private static void FillLanczosKernel(in Vec<double> kernel, int sampleScale, int a, double gain)
        {
            var radius = kernel.Count / 2;
            var sincScale = Math.PI / sampleScale;
            var windowScale = sincScale / a;
            var fkernel = kernel.GetUnsafeFastIndexer();

            for (var i = 0; i < kernel.Count; i++)
            {
                var distance = i - radius;
                fkernel[i] = gain * Sinc(sincScale * distance) * Sinc(windowScale * distance);
            }
        }
    }
}
