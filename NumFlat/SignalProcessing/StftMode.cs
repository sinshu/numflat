using System;

namespace NumFlat.SignalProcessing
{
    /// <summary>
    /// Specifies how the STFT handles signal boundaries.
    /// </summary>
    public enum StftMode
    {
        /// <summary>
        /// Computes the STFT using only frames that fit entirely within the input signal.
        /// No zero-padding is required, but samples near the beginning and end of the signal
        /// may not be fully represented.
        /// </summary>
        Analysis,

        /// <summary>
        /// Computes the STFT using frames that fully cover the input signal.
        /// This enables perfect reconstruction by the inverse STFT, but requires zero-padding
        /// near the signal boundaries, which may affect the spectral characteristics of the
        /// first and last frames.
        /// </summary>
        Synthesis,
    }
}
