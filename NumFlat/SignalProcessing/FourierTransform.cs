using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
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

        private static void ThrowIfInvalidLength(int length)
        {
            if (length < 1)
            {
                throw new ArgumentException("The FFT length must be greater than or equal to one.");
            }

            if ((length & (length - 1)) != 0)
            {
                throw new ArgumentException($"The FFT length must be a power of two, but was {length}.");
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

        /// <summary>
        /// Computes the forward Fourier transform.
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
        /// Computes the inverse Fourier transform.
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
        /// Computes the forward Fourier transform.
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
        /// Computes the inverse Fourier transform.
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
        /// Computes the forward Fourier transform in-place.
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
        /// Computes the inverse Fourier transform in-place.
        /// </summary>
        /// <param name="target">
        /// The target vector to be inverse transformed.
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

        /// <summary>
        /// Computes the forward real Fourier transform.
        /// </summary>
        /// <param name="source">
        /// The source vector to be transformed.
        /// </param>
        /// <param name="destination">
        /// The destination of the transformed vector.
        /// </param>
        /// <remarks>
        /// The length of <paramref name="source"/> must be a power of two.
        /// The length of <paramref name="destination"/> must match <c>source.Count / 2 + 1</c>.
        /// Normalization is only done during the inverse transform.
        /// </remarks>
        public static void Rfft(in Vec<double> source, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (source.Count < 2)
            {
                throw new ArgumentException("The source length must be greater than or equal to two.", nameof(source));
            }

            if ((source.Count & (source.Count - 1)) != 0)
            {
                throw new ArgumentException($"The source length must be a power of two, but was {source.Count}.", nameof(source));
            }

            if (destination.Count != source.Count / 2 + 1)
            {
                throw new ArgumentException("'destination.Count' must match 'source.Count / 2 + 1'.");
            }

            using var utmp = destination.EnsureContiguous(false);
            var tmp = MemoryMarshal.Cast<Complex, double>(utmp.Item.Memory.Span);

            source.CopyTo(tmp.Slice(0, source.Count));
            GetRftInstance(source.Count).Forward(tmp);
        }

        /// <summary>
        /// Computes the inverse real Fourier transform.
        /// </summary>
        /// <param name="source">
        /// The source vector to be inverse transformed.
        /// </param>
        /// <param name="destination">
        /// The destination of the inverse transformed vector.
        /// </param>
        /// <remarks>
        /// The length of <paramref name="source"/> must be a power of two plus one.
        /// The length of <paramref name="destination"/> must match <c>2 * (source.Count - 1)</c>.
        /// Normalization is only done during the inverse transform.
        /// </remarks>
        public static void Irfft(in Vec<Complex> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (source.Count < 2)
            {
                throw new ArgumentException("The source length must be greater than or equal to two.", nameof(source));
            }

            var scm1 = source.Count - 1;
            if ((scm1 & (scm1 - 1)) != 0)
            {
                throw new ArgumentException($"The source length must be a power of two plus one, but was {source.Count}.", nameof(source));
            }

            if (destination.Count != 2 * (source.Count - 1))
            {
                throw new ArgumentException("'destination.Count' must match '2 * (source.Count - 1)'.");
            }

            using var utmp = new TemporalArray<Complex>(source.Count);
            var tmp = utmp.Item;

            source.CopyTo(tmp);
            var casted = GetRftInstance(2 * (source.Count - 1)).Inverse(tmp);

            var i = 0;
            foreach (ref var value in destination)
            {
                value = casted[i];
                i++;
            }
        }

        /// <summary>
        /// Computes the forward real Fourier transform.
        /// </summary>
        /// <param name="source">
        /// The source vector to be transformed.
        /// </param>
        /// <returns>
        /// The transformed vector excluding the complex conjugate part.
        /// </returns>
        /// <remarks>
        /// The length of <paramref name="source"/> must be a power of two.
        /// Normalization is only done during the inverse transform.
        /// </remarks>
        public static Vec<Complex> Rfft(in this Vec<double> source)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));

            if (source.Count < 2)
            {
                throw new ArgumentException("The source length must be greater than or equal to two.", nameof(source));
            }

            if ((source.Count & (source.Count - 1)) != 0)
            {
                throw new ArgumentException($"The source length must be a power of two, but was {source.Count}.", nameof(source));
            }

            var destination = new Vec<Complex>(source.Count / 2 + 1);
            Rfft(source, destination);
            return destination;
        }

        /// <summary>
        /// Computes the inverse real Fourier transform.
        /// </summary>
        /// <param name="source">
        /// The source vector excluding the complex conjugate part to be inverse transformed.
        /// </param>
        /// <returns>
        /// The inverse transformed vector.
        /// </returns>
        /// <remarks>
        /// The length of <paramref name="source"/> must be a power of two plus one.
        /// Normalization is only done during the inverse transform.
        /// </remarks>
        public static Vec<double> Irfft(in this Vec<Complex> source)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));

            if (source.Count < 2)
            {
                throw new ArgumentException("The source length must be greater than or equal to two.", nameof(source));
            }

            var scm1 = source.Count - 1;
            if ((scm1 & (scm1 - 1)) != 0)
            {
                throw new ArgumentException($"The source length must be a power of two plus one, but was {source.Count}.", nameof(source));
            }

            var destination = new Vec<double>(2 * (source.Count - 1));
            Irfft(source, destination);
            return destination;
        }

        /// <summary>
        /// Computes the spectrogram from a time-domain signal using the short-time Fourier transform (STFT).
        /// </summary>
        /// <param name="source">
        /// The source signal to be transformed.
        /// </param>
        /// <param name="window">
        /// The window function to be applied to frames.
        /// </param>
        /// <param name="frameShift">
        /// The frame shift.
        /// </param>
        /// <param name="mode">
        /// The STFT mode.
        /// </param>
        /// <returns>
        /// The spectrogram of the source signal and the information of the transformation.
        /// </returns>
        /// <remarks>
        /// Since the real Fourier transform is used to compute the spectrogram,
        /// frequency components higher than the Nyquist frequency are omitted.
        /// </remarks>
        public static (Vec<Complex>[] Spectrogram, StftInfo Info) Stft(in this Vec<double> source, in Vec<double> window, int frameShift, StftMode mode = StftMode.Analysis)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(window, nameof(window));

            if (window.Count < 2)
            {
                throw new ArgumentException("The window Length must be greater than or equal to two.", nameof(window));
            }

            if ((window.Count & (window.Count - 1)) != 0)
            {
                throw new ArgumentException($"The window length must be a power of two.", nameof(window));
            }

            if (frameShift <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(frameShift), "The frame shift must be a positive value.");
            }

            if (window.Count % frameShift != 0)
            {
                throw new ArgumentException("The window length must be divisible by the frame shift.");
            }

            int firstFramePosition;
            int frameCount;
            if (mode == StftMode.Analysis)
            {
                firstFramePosition = 0;
                frameCount = (source.Count - window.Count) / frameShift;
            }
            else if (mode == StftMode.Synthesis)
            {
                ThrowIfReconstructionIsNotPossible(window, frameShift);
                firstFramePosition = frameShift - window.Count;
                frameCount = (source.Count - firstFramePosition + frameShift - 1) / frameShift;
            }
            else
            {
                throw new ArgumentException("Invalid enum value.", nameof(mode));
            }

            if (frameCount == 0)
            {
                throw new ArgumentException("The length of the source signal is too short.");
            }

            var rft = GetRftInstance(window.Count);
            var position = firstFramePosition;
            var spectrogram = new Vec<Complex>[frameCount];
            for (var i = 0; i < spectrogram.Length; i++)
            {
                var spectrum = new Vec<Complex>(window.Count / 2 + 1);
                var span = spectrum.Memory.Span;
                source.GetWindowedFrame(position, window, MemoryMarshal.Cast<Complex, double>(span.Slice(0, window.Count / 2)));
                rft.Forward(MemoryMarshal.Cast<Complex, double>(span));
                spectrogram[i] = spectrum;
                position += frameShift;
            }

            var info = new StftInfo(window, firstFramePosition, frameShift, source.Count);

            return (spectrogram, info);
        }

        /// <summary>
        /// Reconstructs the time-domain signal from a spectrogram using the inverse short-time Fourier transform (ISTFT).
        /// </summary>
        /// <param name="spectrogram">
        /// The spectrogram to be inverse transformed.
        /// </param>
        /// <param name="info">
        /// The settings of the STFT used for the original transformation.
        /// </param>
        /// <returns>
        /// The time-domain signal reconstructed from the spectrogram.
        /// </returns>
        public static Vec<double> Istft(this IEnumerable<Vec<Complex>> spectrogram, StftInfo info)
        {
            ThrowHelper.ThrowIfNull(spectrogram, nameof(spectrogram));
            ThrowHelper.ThrowIfNull(info, nameof(info));

            using var uframe = new TemporalVector<double>(info.Window.Count + 2);
            ref readonly var frame = ref uframe.Item;
            var sf = frame.Memory.Span;

            var rft = FourierTransform.GetRftInstance(info.Window.Count);
            var position = info.FirstFramePosition;
            var destination = new Vec<double>(info.SignalLength);
            foreach (var spectrum in spectrogram.ThrowIfEmptyOrDifferentSize(info.Window.Count / 2 + 1, nameof(spectrogram)))
            {
                var csf = MemoryMarshal.Cast<double, Complex>(sf);
                spectrum.CopyTo(csf);
                rft.Inverse(csf);
                destination.WindowedOverlapAdd(position, info.Window, frame.Subvector(0, info.Window.Count));
                position += info.FrameShift;
            }

            destination.MulInplace(1 / GetWindowGain(info.Window, info.FrameShift));

            return destination;
        }

        private static void GetWindowedFrame(in this Vec<double> source, int start, in Vec<double> window, Span<double> destination)
        {
            var srcStart = start;
            var dstStart = 0;
            var copyLength = destination.Length;

            if (srcStart < 0)
            {
                var trim = -srcStart;
                srcStart += trim;
                dstStart += trim;
                copyLength -= trim;
            }

            if (srcStart + copyLength > source.Count)
            {
                var trim = srcStart + copyLength - source.Count;
                copyLength -= trim;
            }

            if (copyLength < destination.Length)
            {
                destination.Clear();
            }

            if (copyLength > 0)
            {
                var src = source.Subvector(srcStart, copyLength);
                var win = window.Subvector(dstStart, copyLength);
                var dst = destination.Slice(dstStart, copyLength);
                var ss = src.Memory.Span;
                var sw = win.Memory.Span;
                var sd = dst;
                var ps = 0;
                var pw = 0;
                var pd = 0;
                while (pd < sd.Length)
                {
                    sd[pd] = sw[pw] * ss[ps];
                    ps += src.Stride;
                    pw += win.Stride;
                    pd++;
                }
            }
        }

        private static void ThrowIfReconstructionIsNotPossible(in Vec<double> window, int frameShift)
        {
            var fw = window.GetUnsafeFastIndexer();
            var min = double.MaxValue;
            var max = double.MinValue;
            for (var i = 0; i < frameShift; i++)
            {
                var height = 0.0;
                for (var j = i; j < window.Count; j += frameShift)
                {
                    var value = fw[j];
                    height += value * value;
                }
                if (height < min)
                {
                    min = height;
                }
                if (height > max)
                {
                    max = height;
                }
            }

            if (max - min > 1.0E-14) // np.finfo(np.float64).resolution * 10
            {
                throw new ArgumentException("Signal reconstruction is not possible with the specified STFT settings.");
            }
        }

        private static double GetWindowGain(in Vec<double> window, int frameShift)
        {
            var sum = 0.0;
            foreach (var value in window)
            {
                sum += value * value;
            }
            return sum / frameShift;
        }
    }
}
