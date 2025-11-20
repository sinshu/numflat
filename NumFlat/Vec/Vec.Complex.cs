using System;
using System.Numerics;

namespace NumFlat
{
    public static partial class Vec
    {
        /// <summary>
        /// Conjugates the complex vector.
        /// </summary>
        /// <param name="x">
        /// The complex vector to be conjugated.
        /// </param>
        /// <param name="destination">
        /// The destination of the conjugation.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Conjugate(in Vec<Complex> x, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px].Conjugate();
                px += x.Stride;
                pd += destination.Stride;
            }
        }

        /// <summary>
        /// Extracts the real part of each element in the complex vector.
        /// </summary>
        /// <param name="x">
        /// The complex vector.
        /// </param>
        /// <param name="destination">
        /// The destination of the real parts.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Real(in Vec<Complex> x, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.Count != x.Count)
            {
                throw new ArgumentException("'destination.Count' must match 'x.Count'.");
            }

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px].Real;
                px += x.Stride;
                pd += destination.Stride;
            }
        }

        /// <summary>
        /// Extracts the imaginary part of each element in the complex vector.
        /// </summary>
        /// <param name="x">
        /// The complex vector.
        /// </param>
        /// <param name="destination">
        /// The destination of the imaginary parts.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Imaginary(in Vec<Complex> x, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.Count != x.Count)
            {
                throw new ArgumentException("'destination.Count' must match 'x.Count'.");
            }

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px].Imaginary;
                px += x.Stride;
                pd += destination.Stride;
            }
        }
    }
}
