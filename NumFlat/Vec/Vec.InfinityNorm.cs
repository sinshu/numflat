using System;
using System.Numerics;

namespace NumFlat
{
    public static partial class Vec
    {
        /// <summary>
        /// Computes the infinity norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The infinity norm.
        /// </returns>
        public static float InfinityNorm(in this Vec<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0F;
            foreach (var value in x)
            {
                var current = MathF.Abs(value);
                if (current > max)
                {
                    max = current;
                }
            }
            return max;
        }

        /// <summary>
        /// Computes the infinity norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The infinity norm.
        /// </returns>
        public static double InfinityNorm(in this Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0;
            foreach (var value in x)
            {
                var current = Math.Abs(value);
                if (current > max)
                {
                    max = current;
                }
            }
            return max;
        }

        /// <summary>
        /// Computes the infinity norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The infinity norm.
        /// </returns>
        public static double InfinityNorm(in this Vec<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0;
            foreach (var value in x)
            {
                var current = value.Magnitude;
                if (current > max)
                {
                    max = current;
                }
            }
            return max;
        }
    }
}
