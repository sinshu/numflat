using System;
using System.Numerics;

namespace NumFlat
{
    public static partial class Vec
    {
        /// <summary>
        /// Computes the L1 norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The L1 norm.
        /// </returns>
        public static float L1Norm(in this Vec<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var norm = 0.0F;
            foreach (var value in x.GetUnsafeFastIndexer())
            {
                norm += MathF.Abs(value);
            }
            return norm;
        }

        /// <summary>
        /// Computes the L1 norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The L1 norm.
        /// </returns>
        public static double L1Norm(in this Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var norm = 0.0;
            foreach (var value in x.GetUnsafeFastIndexer())
            {
                norm += Math.Abs(value);
            }
            return norm;
        }

        /// <summary>
        /// Computes the L1 norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The L1 norm.
        /// </returns>
        public static double L1Norm(in this Vec<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var norm = 0.0;
            foreach (var value in x.GetUnsafeFastIndexer())
            {
                norm += value.Magnitude;
            }
            return norm;
        }
    }
}
