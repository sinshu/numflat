﻿using System;

namespace NumFlat.SignalProcessing
{
    /// <summary>
    /// Contains STFT-related information.
    /// </summary>
    public class StftInfo
    {
        private readonly Vec<double> window;
        private readonly int firstFramePosition;
        private readonly int frameShift;
        private readonly int signalLength;

        /// <summary>
        /// Creates a new instance of <see cref="StftInfo"/>.
        /// </summary>
        /// <param name="window">
        /// The window function.
        /// </param>
        /// <param name="firstFramePosition">
        /// The position of the first frame.
        /// </param>
        /// <param name="frameShift">
        /// The frame shift.
        /// </param>
        /// <param name="signalLength">
        /// The length of the time domain signal.
        /// </param>
        public StftInfo(in Vec<double> window, int firstFramePosition, int frameShift, int signalLength)
        {
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

            if (signalLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalLength), "The signal length must be a positive value.");
            }

            this.window = window;
            this.firstFramePosition = firstFramePosition;
            this.frameShift = frameShift;
            this.signalLength = signalLength;
        }

        /// <summary>
        /// Gets the position of a frame by its index.
        /// </summary>
        /// <param name="frameIndex">
        /// The index of the frame.
        /// </param>
        /// <returns>
        /// The position of the frame.
        /// </returns>
        public int GetFramePosition(int frameIndex)
        {
            return firstFramePosition + frameIndex * FrameShift;
        }

        /// <summary>
        /// Gets the time of a frame by its index.
        /// </summary>
        /// <param name="sampleRate">
        /// The sample rate of the source signal.
        /// </param>
        /// <param name="frameIndex">
        /// The index of the frame.
        /// </param>
        /// <returns>
        /// The time of the frame.
        /// </returns>
        public double GetFrameTime(int sampleRate, int frameIndex)
        {
            return (double)GetFramePosition(frameIndex) / sampleRate;
        }

        /// <summary>
        /// Gets the frequency by its index;
        /// </summary>
        /// <param name="sampleRate">
        /// The sample rate of the source signal.
        /// </param>
        /// <param name="frequencyBinIndex">
        /// The index of the frequency bin.
        /// </param>
        /// <returns>
        /// The specified frequency;
        /// </returns>
        public double GetFrequency(int sampleRate, int frequencyBinIndex)
        {
            return (double)(sampleRate * frequencyBinIndex) / window.Count;
        }

        /// <summary>
        /// Gets the window function.
        /// </summary>
        public ref readonly Vec<double> Window => ref window;

        /// <summary>
        /// Gets te position of the first frame.
        /// </summary>
        public int FirstFramePosition => firstFramePosition;

        /// <summary>
        /// Gets the frame shift.
        /// </summary>
        public int FrameShift => frameShift;

        /// <summary>
        /// Gets the length of the time domain signal.
        /// </summary>
        public int SignalLength => signalLength;
    }
}
