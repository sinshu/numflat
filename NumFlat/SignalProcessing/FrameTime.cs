using System;

namespace NumFlat.SignalProcessing
{
    /// <summary>
    /// Represents the time position of a frame within the original signal in seconds.
    /// </summary>
    public struct FrameTime
    {
        private double start;
        private double end;

        internal FrameTime(double start, double end)
        {
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Gets the start time (in seconds) of the frame in the source signal.
        /// </summary>
        public double Start => start;

        /// <summary>
        /// Gets the end time (in seconds) of the frame in the source signal.
        /// </summary>
        public double End => end;

        /// <summary>
        /// Gets the center time (in seconds) of the frame in the source signal.
        /// Computed as the average of <see cref="Start"/> and <see cref="End"/>.
        /// </summary>
        public double Center => 0.5 * (start + end);
    }
}
