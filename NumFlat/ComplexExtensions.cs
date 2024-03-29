using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NumFlat
{
    /// <summary>
    /// Provides extension methods for the <see cref="Complex"/> type.
    /// </summary>
    public static class ComplexExtensions
    {
        /// <summary>
        /// Conjugates the complex number.
        /// </summary>
        /// <param name="value">
        /// The complex number to be conjugated.
        /// </param>
        /// <returns>
        /// The conjugated complex number.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Complex Conjugate(this Complex value)
        {
            return new Complex(value.Real, -value.Imaginary);
        }

        /// <summary>
        /// Conjugates the squared magnitude of the complex number.
        /// </summary>
        /// <param name="value">
        /// The target complex number.
        /// </param>
        /// <returns>
        /// The the squared magnitude.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double MagnitudeSquared(this Complex value)
        {
            return value.Real * value.Real + value.Imaginary * value.Imaginary;
        }
    }
}
