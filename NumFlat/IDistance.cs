using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides a distance measure between two objects.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the first object.
    /// </typeparam>
    /// <typeparam name="U">
    /// The type of the second object.
    /// </typeparam>
    public interface IDistance<T, U>
    {
        /// <summary>
        /// Calculates the distance between two objects.
        /// </summary>
        /// <param name="x">
        /// The first object.
        /// </param>
        /// <param name="y">
        /// The second object.
        /// </param>
        /// <returns>
        /// The calculated distance.
        /// </returns>
        public double GetDistance(T x, U y);
    }
}
