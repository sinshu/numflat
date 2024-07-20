using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides extension methods for the <see cref="Random"/> class.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns a random value generated from the standard Gaussian distribution.
        /// </summary>
        /// <param name="random">
        /// An instance of the <see cref="Random"/> class.
        /// </param>
        /// <returns>
        /// A random value generated from the standard Gaussian distribution.
        /// </returns>
        public static double NextGaussian(this Random random)
        {
            double u, v, s;
            do
            {
                u = 2 * random.NextDouble() - 1;
                v = 2 * random.NextDouble() - 1;
                s = u * u + v * v;
            }
            while (s >= 1 || s == 0);

            s = Math.Sqrt(-2 * Math.Log(s) / s);
            return u * s;
        }

        /// <summary>
        /// Returns a random complex value generated from the standard Gaussian distribution.
        /// </summary>
        /// <param name="random">
        /// An instance of the <see cref="Random"/> class.
        /// </param>
        /// <returns>
        /// A random complex value generated from the standard Gaussian distribution.
        /// </returns>
        public static Complex NextComplexGaussian(this Random random)
        {
            double u, v, s;
            do
            {
                u = 2 * random.NextDouble() - 1;
                v = 2 * random.NextDouble() - 1;
                s = u * u + v * v;
            }
            while (s >= 1 || s == 0);

            s = Math.Sqrt(-2 * Math.Log(s) / s);
            return new(u * s, v * s);
        }
    }
}
