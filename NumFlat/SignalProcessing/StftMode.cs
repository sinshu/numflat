using System;

namespace NumFlat.SignalProcessing
{
    /// <summary>
    /// Specifies an STFT mode.
    /// </summary>
    public enum StftMode
    {
        /// <summary>
        /// Uses only frames within the signal, potentially losing information near its boundaries.
        /// </summary>
        Analysis,

        /// <summary>
        /// Adds frames with zeros outside the signal for reconstruction,
        /// potentially affecting frequency characteristics near its boundaries.
        /// </summary>
        Synthesis,
    }
}
