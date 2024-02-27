using System;
using System.Numerics;

namespace NumFlat.SignalProcessing
{
    /// <summary>
    /// Provides signal processing functionality.
    /// </summary>
    public static class SignalProcessing
    {
        /// <summary>
        /// Gets a frame from a source signal.
        /// </summary>
        /// <param name="source">
        /// The source signal.
        /// </param>
        /// <param name="start">
        /// The starting position of the frame.
        /// </param>
        /// <param name="destination">
        /// The destination of the frame.
        /// </param>
        public static void GetFrame(in this Vec<double> source, int start, in Vec<double> destination)
        {
            throw new NotImplementedException();
        }

        public static Vec<double> GetFrame(in this Vec<double> source, int start, int length)
        {
            throw new NotImplementedException();
        }

        public static void GetWindowedFrame(in this Vec<double> source, int start, in Vec<double> window, in Vec<double> destination)
        {
            throw new NotImplementedException();
        }

        public static Vec<double> GetWindowedFrame(in this Vec<double> source, int start, in Vec<double> window)
        {
            throw new NotImplementedException();
        }

        public static void GetFrameAsComplex(in this Vec<double> source, int start, in Vec<Complex> destination)
        {
            throw new NotImplementedException();
        }

        public static Vec<Complex> GetFrameAsComplex(in this Vec<double> source, int start, int length)
        {
            throw new NotImplementedException();
        }

        public static void GetWindowedFrameAsComplex(in this Vec<double> source, int start, in Vec<double> window, in Vec<Complex> destination)
        {
            throw new NotImplementedException();
        }

        public static Vec<Complex> GetWindowedFrameAsComplex(in this Vec<double> source, int start, in Vec<double> window)
        {
            throw new NotImplementedException();
        }
    }
}
