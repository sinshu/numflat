using System;
using System.Numerics;

namespace NumFlat
{
    public static partial class Mat
    {
        /// <summary>
        /// Computes the infinity norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The infinity norm.
        /// </returns>
        public static float InfinityNorm(in this Mat<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0F;
            foreach (var row in x.Rows)
            {
                var norm = row.L1Norm();
                if (norm > max)
                {
                    max = norm;
                }
            }
            return max;
        }

        /// <summary>
        /// Computes the infinity norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The infinity norm.
        /// </returns>
        public static double InfinityNorm(in this Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0;
            foreach (var row in x.Rows)
            {
                var norm = row.L1Norm();
                if (norm > max)
                {
                    max = norm;
                }
            }
            return max;
        }

        /// <summary>
        /// Computes the infinity norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The infinity norm.
        /// </returns>
        public static double InfinityNorm(in this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0;
            foreach (var row in x.Rows)
            {
                var norm = row.L1Norm();
                if (norm > max)
                {
                    max = norm;
                }
            }
            return max;
        }
    }
}
