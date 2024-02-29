using System;

namespace NumFlat.SignalProcessing
{
    /// <summary>
    /// Specifies an STFT mode.
    /// </summary>
    public enum StftMode
    {
        /// <summary>
        /// Computes the STFT for analysis purposes.
        /// Since all short-time frames fit within the length of the original signal,
        /// zero-filling has no effect.
        /// However, information at the beginning and end of the original signal may be lost.
        /// </summary>
        Analysis,

        /// <summary>
        /// Computes the STFT for synthesis purposes.
        /// Short-time frames completely cover the entire original signal, allowing for perfect reconstruction.
        /// However, zero-filling may disturb the frequency response of the beginning and ending short-time frames.
        /// </summary>
        Synthesis,
    }
}
