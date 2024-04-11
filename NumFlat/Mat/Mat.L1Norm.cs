using System;
using System.Numerics;

namespace NumFlat
{
    public static partial class Mat
    {
        /// <summary>
        /// Computes the L1 norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The L1 norm.
        /// </returns>
        public static float L1Norm(in this Mat<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0F;
            foreach (var col in x.Cols)
            {
                var norm = col.L1Norm();
                if (norm > max)
                {
                    max = norm;
                }
            }
            return max;
        }

        /// <summary>
        /// Computes the L1 norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The L1 norm.
        /// </returns>
        public static double L1Norm(in this Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0;
            foreach (var col in x.Cols)
            {
                var norm = col.L1Norm();
                if (norm > max)
                {
                    max = norm;
                }
            }
            return max;
        }

        /// <summary>
        /// Computes the L1 norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The L1 norm.
        /// </returns>
        public static double L1Norm(in this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0;
            foreach (var col in x.Cols)
            {
                var norm = col.L1Norm();
                if (norm > max)
                {
                    max = norm;
                }
            }
            return max;
        }
    }
}
