using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides extension methods for '<see cref="Complex"/>'.
    /// </summary>
    public static class ComplexExtension
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
        public static Complex Conjugate(this Complex value)
        {
            return new Complex(value.Real, -value.Imaginary);
        }
    }
}
