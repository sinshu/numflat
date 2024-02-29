using System;
using System.Collections.Generic;
using System.Numerics;
using FftFlat;

namespace NumFlat.SignalProcessing
{
    /// <summary>
    /// Provides the Fourier transform.
    /// </summary>
    public static class FourierTransform
    {
        [ThreadStatic]
        private static Dictionary<int, FastFourierTransform>? fftCache;

        [ThreadStatic]
        private static Dictionary<int, RealFourierTransform>? rftCache;

        /// <summary>
        /// Compute the forward Fourier transform.
        /// </summary>
        /// <param name="source">
        /// The source vector to be transformed.
        /// </param>
        /// <param name="destination">
        /// The destination of the transformed vector.
        /// </param>
        /// <remarks>
        /// The FFT length must be a power of two.
        /// Normalization is only done during the inverse transform.
        /// </remarks>
        public static void Fft(in Vec<Complex> source, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(source, destination);
            ThrowIfInvalidLength(source.Count);

            using var utmp = destination.EnsureContiguous(false);
            ref readonly var tmp = ref utmp.Item;
            source.CopyTo(tmp);

            GetFftInstance(tmp.Count).Forward(tmp.Memory.Span);
        }

        /// <summary>
        /// Compute the inverse Fourier transform.
        /// </summary>
        /// <param name="source">
        /// The source vector to be inverse transformed.
        /// </param>
        /// <param name="destination">
        /// The destination of the inverse transformed vector.
        /// </param>
        /// <remarks>
        /// The FFT length must be a power of two.
        /// Normalization is only done during the inverse transform.
        /// </remarks>
        public static void Ifft(in Vec<Complex> source, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(source, destination);
            ThrowIfInvalidLength(source.Count);

            using var utmp = destination.EnsureContiguous(false);
            ref readonly var tmp = ref utmp.Item;
            source.CopyTo(tmp);

            GetFftInstance(tmp.Count).Inverse(tmp.Memory.Span);
        }

        /// <summary>
        /// Compute the forward Fourier transform.
        /// </summary>
        /// <param name="source">
        /// The source vector to be transformed.
        /// </param>
        /// <returns>
        /// The transformed vector.
        /// </returns>
        /// <remarks>
        /// The FFT length must be a power of two.
        /// Normalization is only done during the inverse transform.
        /// </remarks>
        public static Vec<Complex> Fft(in this Vec<Complex> source)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowIfInvalidLength(source.Count);

            var dst = new Vec<Complex>(source.Count);
            source.CopyTo(dst);

            GetFftInstance(dst.Count).Forward(dst.Memory.Span);

            return dst;
        }

        /// <summary>
        /// Compute the inverse Fourier transform.
        /// </summary>
        /// <param name="source">
        /// The source vector to be inverse transformed.
        /// </param>
        /// <returns>
        /// The inverse transformed vector.
        /// </returns>
        /// <remarks>
        /// The FFT length must be a power of two.
        /// Normalization is only done during the inverse transform.
        /// </remarks>
        public static Vec<Complex> Ifft(in this Vec<Complex> source)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowIfInvalidLength(source.Count);

            var dst = new Vec<Complex>(source.Count);
            source.CopyTo(dst);

            GetFftInstance(dst.Count).Inverse(dst.Memory.Span);

            return dst;
        }

        /// <summary>
        /// Compute the forward Fourier transform in-place.
        /// </summary>
        /// <param name="target">
        /// The target vector to be transformed.
        /// </param>
        /// <remarks>
        /// The FFT length must be a power of two.
        /// Normalization is only done during the inverse transform.
        /// </remarks>
        public static void FftInplace(in this Vec<Complex> target)
        {
            ThrowHelper.ThrowIfEmpty(target, nameof(target));
            ThrowIfInvalidLength(target.Count);

            using var utmp = target.EnsureContiguous(true);
            ref readonly var tmp = ref utmp.Item;

            GetFftInstance(tmp.Count).Forward(tmp.Memory.Span);
        }

        /// <summary>
        /// Compute the forward Fourier transform in-place.
        /// </summary>
        /// <param name="target">
        /// The target vector to be transformed.
        /// </param>
        /// <remarks>
        /// The FFT length must be a power of two.
        /// Normalization is only done during the inverse transform.
        /// </remarks>
        public static void IfftInplace(in this Vec<Complex> target)
        {
            ThrowHelper.ThrowIfEmpty(target, nameof(target));
            ThrowIfInvalidLength(target.Count);

            using var utmp = target.EnsureContiguous(true);
            ref readonly var tmp = ref utmp.Item;

            GetFftInstance(tmp.Count).Inverse(tmp.Memory.Span);
        }

        private static void ThrowIfInvalidLength(int length)
        {
            if (length < 1)
            {
                throw new ArgumentException("The FFT Length must be greater than or equal to one.");
            }

            if ((length & (length - 1)) != 0)
            {
                throw new ArgumentException($"The FFT length must be a power of two, but was '{length}'.");
            }
        }

        internal static FastFourierTransform GetFftInstance(int length)
        {
            if (fftCache == null)
            {
                fftCache = new Dictionary<int, FastFourierTransform>();
            }

            FastFourierTransform? fft;
            if (!fftCache.TryGetValue(length, out fft))
            {
                fft = new FastFourierTransform(length);
                fftCache.Add(length, fft);
            }

            return fft;
        }

        internal static RealFourierTransform GetRftInstance(int length)
        {
            if (rftCache == null)
            {
                rftCache = new Dictionary<int, RealFourierTransform>();
            }

            RealFourierTransform? rft;
            if (!rftCache.TryGetValue(length, out rft))
            {
                rft = new RealFourierTransform(length);
                rftCache.Add(length, rft);
            }

            return rft;
        }
    }
}
