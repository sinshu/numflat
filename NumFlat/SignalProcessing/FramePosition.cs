using System;

namespace NumFlat.SignalProcessing
{
    /// <summary>
    /// Represents the position of a frame within the original signal.
    /// </summary>
    public struct FramePosition
    {
        private int start;
        private int end;

        internal FramePosition(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Gets the start index (inclusive) of the frame in the source signal.
        /// </summary>
        public int Start => start;

        /// <summary>
        /// Gets the end index (exclusive) of the frame in the source signal.
        /// </summary>
        public int End => end;

        /// <summary>
        /// Gets the center index of the frame in the source signal.
        /// Computed as the average of <see cref="Start"/> and <see cref="End"/>, rounded down.
        /// </summary>
        public int Center => (start + end) / 2;
    }
}
