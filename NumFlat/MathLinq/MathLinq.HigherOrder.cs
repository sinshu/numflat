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

            var (mean, n) = xs.MeanAndCount();

            var m2 = xs.Moment(2, mean);
            if (m2 <= 1.0E-14) // np.finfo(np.float64).resolution * 10
            {
                throw new ArgumentException("The variance of the values is too small.");
            }

            var m3 = xs.Moment(3, mean);

            if (unbiased)
            {
                if (n <= 2)
                {
                    throw new ArgumentException("The number of source values is not sufficient.");
                }
                return Math.Sqrt((n - 1) * n) / (n - 2) * m3 / Math.Pow(m2, 1.5);
            }
            else
            {
                return m3 / Math.Pow(m2, 1.5);
            }
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

            var (mean, n) = xs.MeanAndCount();

            var m2 = xs.Moment(2, mean);
            if (m2 <= 1.0E-14) // np.finfo(np.float64).resolution * 10
            {
                throw new ArgumentException("The variance of the values is too small.");
            }

            var m4 = xs.Moment(4, mean);

            if (unbiased)
            {
                if (n <= 3)
                {
                    throw new ArgumentException("The number of source values is not sufficient.");
                }
                return 1.0 / (n - 2) / (n - 3) * ((n * n - 1) * m4 / (m2 * m2) - 3 * (n - 1) * (n - 1));
            }
            else
            {
                return m4 / (m2 * m2) - 3;
            }
        }

        private static (double Mean, int Count) MeanAndCount(this IEnumerable<double> xs)
        {
            var sum = 0.0;
            var count = 0;
            foreach (var x in xs)
            {
                sum += x;
                count++;
            }

            if (count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one value.");
            }

            return (sum / count, count);
        }

        private static double Moment(this IEnumerable<double> xs, int moment, double mean)
        {
            var sum = 0.0;
            var count = 0;
            foreach (var x in xs)
            {
                sum += Math.Pow(x - mean, moment);
                count++;
            }

            return sum / count;
        }
    }
}
