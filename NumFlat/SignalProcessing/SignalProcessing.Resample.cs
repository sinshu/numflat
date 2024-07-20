using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;

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
                throw new ArgumentOutOfRangeException(nameof(a), "The quality factor of the Lanczos resamplin must be greater than or equal to one.");
            }

            using var usrc = source.EnsureContiguous(true);
            ref readonly var src = ref usrc.Item;

            using var udst = destination.EnsureContiguous(false);
            ref readonly var dst = ref udst.Item;

            ResampleCore(src.Memory.Span, dst.Memory.Span, p, q, a);
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
                throw new ArgumentOutOfRangeException(nameof(a), "The quality factor of the Lanczos resamplin must be greater than or equal to one.");
            }

            var length = (int)Math.Ceiling((double)source.Count * p / q);
            var destination = new Vec<double>(length);
            Resample(source, destination, p, q, a);
            return destination;
        }

        private static void ResampleCore(ReadOnlySpan<double> source, Span<double> destination, int p, int q, int a)
        {
            if (p > q)
            {
                Upsample(source, destination, p, q, a);
            }
            else if (q > p)
            {
                Downsample(source, destination, p, q, a);
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

        private static void Upsample(ReadOnlySpan<double> source, Span<double> destination, int p, int q, int a)
        {
            var qdp = (double)q / p;
            for (var i = 0; i < destination.Length; i++)
            {
                var position = i * qdp;
                destination[i] = NaiveResample(source, position, 1, a);
            }
        }

        private static void Downsample(ReadOnlySpan<double> source, Span<double> destination, int p, int q, int a)
        {
            var pdq = (double)p / q;
            var qdp = (double)q / p;
            for (var i = 0; i < destination.Length; i++)
            {
                var position = i * qdp;
                destination[i] = pdq * NaiveResample(source, position, qdp, a);
            }
        }

        private static double NaiveResample(ReadOnlySpan<double> source, double position, double sincFactor, int a)
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
    }
}
