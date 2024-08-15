using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;

namespace NumFlat.SignalProcessing
{
    public static partial class SignalProcessing
    {
        /// <summary>
        /// Convolves a signal with an impulse response.
        /// </summary>
        /// <param name="signal">
        /// The input signal to be convolved.
        /// </param>
        /// <param name="impulseResponse">
        /// The impulse response to convolve.
        /// </param>
        /// <param name="destination">
        /// The destination where the convolution result is stored.
        /// </param>
        public static void Convolve(in this Vec<double> signal, in Vec<double> impulseResponse, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(signal, nameof(signal));
            ThrowHelper.ThrowIfEmpty(impulseResponse, nameof(impulseResponse));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            var fftLength = 2;
            while (fftLength < 2 * impulseResponse.Count)
            {
                fftLength *= 2;
            }
            while (fftLength < Math.Min(2 * signal.Count, 1024))
            {
                fftLength *= 2;
            }
            var rft = FourierTransform.GetRftInstance(fftLength);

            using var utmp = new TemporalVector2<double>(fftLength + 2);
            ref readonly var sigFrame = ref utmp.Item1;
            ref readonly var impFrame = ref utmp.Item2;

            impFrame.Clear();
            impulseResponse.CopyTo(impFrame.Memory.Span.Slice(0, impulseResponse.Count));
            var impSpectrum = rft.Forward(impFrame.Memory.Span);

            destination.Clear();

            for (var pos = 0; pos < signal.Count; pos += fftLength / 2)
            {
                signal.GetFrame(pos, sigFrame.Subvector(0, fftLength / 2));
                sigFrame.Subvector(fftLength / 2, fftLength / 2).Clear();
                var sigSpectrum = rft.Forward(sigFrame.Memory.Span);
                for (var i = 0; i < sigSpectrum.Length; i++)
                {
                    sigSpectrum[i] *= impSpectrum[i];
                }
                rft.Inverse(sigSpectrum);
                destination.OverlapAdd(pos, sigFrame);
            }
        }

        /// <summary>
        /// Convolves a signal with an impulse response.
        /// The length of the result is <c><paramref name="signal"/>.Count + <paramref name="impulseResponse"/>.Count - 1</c>.
        /// </summary>
        /// <param name="signal">
        /// The input signal to be convolved.
        /// </param>
        /// <param name="impulseResponse">
        /// The impulse response to convolve.
        /// </param>
        /// <returns>
        /// The convolution result.
        /// </returns>
        public static Vec<double> Convolve(in this Vec<double> signal, in Vec<double> impulseResponse)
        {
            ThrowHelper.ThrowIfEmpty(signal, nameof(signal));
            ThrowHelper.ThrowIfEmpty(impulseResponse, nameof(impulseResponse));

            var destination = new Vec<double>(signal.Count + impulseResponse.Count - 1);
            signal.Convolve(impulseResponse, destination);
            return destination;
        }
    }
}
