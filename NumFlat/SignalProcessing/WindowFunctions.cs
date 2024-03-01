using System;

namespace NumFlat.SignalProcessing
{
    /// <summary>
    /// Provides window functions.
    /// </summary>
    public static class WindowFunctions
    {
        /// <summary>
        /// Creates a Hann window.
        /// </summary>
        /// <param name="length">
        /// The length of the window.
        /// </param>
        /// <returns>
        /// The window with the specified length.
        /// </returns>
        public static Vec<double> Hann(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "The length must be a positive value.");
            }

            var window = new double[length];
            for (var i = 0; i < window.Length; i++)
            {
                var theta = 2 * Math.PI * i / length;
                window[i] = 0.5 - 0.5 * Math.Cos(theta);
            }
            return new Vec<double>(window);
        }

        /// <summary>
        /// Creates a square-root Hann window.
        /// </summary>
        /// <param name="length">
        /// The length of the window.
        /// </param>
        /// <returns>
        /// The window with the specified length.
        /// </returns>
        public static Vec<double> SquareRootHann(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "The length must be a positive value.");
            }

            var window = new double[length];
            for (var i = 0; i < window.Length; i++)
            {
                var theta = 2 * Math.PI * i / length;
                window[i] = Math.Sqrt(0.5 - 0.5 * Math.Cos(theta));
            }
            return new Vec<double>(window);
        }

        /// <summary>
        /// Creates a Hamming window.
        /// </summary>
        /// <param name="length">
        /// The length of the window.
        /// </param>
        /// <returns>
        /// The window with the specified length.
        /// </returns>
        public static Vec<double> Hamming(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "The length must be a positive value.");
            }

            var window = new double[length];
            for (var i = 0; i < window.Length; i++)
            {
                var theta = 2 * Math.PI * i / length;
                window[i] = 0.54 - 0.46 * Math.Cos(theta);
            }
            return new Vec<double>(window);
        }
    }
}
