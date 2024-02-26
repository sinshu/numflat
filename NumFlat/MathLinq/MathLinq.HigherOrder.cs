using System;
using System.Collections.Generic;

namespace NumFlat
{
    public static partial class MathLinq
    {
        /// <summary>
        /// Computes the skewness of a sequence of values.
        /// </summary>
        /// <param name="xs">
        /// The source values.
        /// </param>
        /// <param name="unbiased">
        /// True for an unbiased estimator, false for a biased estimator.
        /// </param>
        /// <returns>
        /// The skewness of the sequence of values.
        /// </returns>
        public static double Skewness(this IEnumerable<double> xs, bool unbiased = true)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Computes the kurtosis of a sequence of values.
        /// </summary>
        /// <param name="xs">
        /// The source values.
        /// </param>
        /// <param name="unbiased">
        /// True for an unbiased estimator, false for a biased estimator.
        /// </param>
        /// <returns>
        /// The kurtosis of the sequence of values.
        /// </returns>
        public static double Kurtosis(this IEnumerable<double> xs, bool unbiased = true)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            throw new NotImplementedException();
        }
    }
}
