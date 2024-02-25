using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat
{
    public static partial class MathLinq
    {
        /// <summary>
        /// Computes the variance from a sequence of values.
        /// </summary>
        /// <param name="xs">
        /// The source values.
        /// </param>
        /// <param name="mean">
        /// The pre-computed mean of the source values.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The variance of the source values.
        /// </returns>
        public static double Variance(this IEnumerable<double> xs, double mean, int ddof = 1)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var sum = 0.0;
            var count = 0;
            foreach (var x in xs)
            {
                var d = x - mean;
                sum += d * d;
                count++;
            }

            if (count - ddof <= 0)
            {
                throw new ArgumentException("The number of source values is not sufficient.");
            }

            return sum / (count - ddof);
        }

        /// <summary>
        /// Computes the mean and variance from a sequence of values.
        /// </summary>
        /// <param name="xs">
        /// The source values.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The mean and variance.
        /// </returns>
        public static (double Mean, double Variance) MeanAndVariance(this IEnumerable<double> xs, int ddof = 1)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var mean = xs.Average();
            var variance = xs.Variance(mean, ddof);
            return (mean, variance);
        }
    }
}
