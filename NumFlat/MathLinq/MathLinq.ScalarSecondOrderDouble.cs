using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat
{
    public static partial class MathLinq
    {
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

            var variance = sum / (count - ddof);

            return (mean, variance);
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
        public static (double Mean, double StandardDeviation) MeanAndStandardDeviation(this IEnumerable<double> xs, int ddof = 1)
        {
            var (mean, variance) = xs.MeanAndVariance(ddof);
            return (mean, Math.Sqrt(variance));
        }

        /// <summary>
        /// Computes the variance from a sequence of values.
        /// </summary>
        /// <param name="xs">
        /// The source values.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The variance of the source values.
        /// </returns>
        public static double Variance(this IEnumerable<double> xs, int ddof = 1)
        {
            return xs.MeanAndVariance(ddof).Variance;
        }

        /// <summary>
        /// Computes the standard deviation from a sequence of values.
        /// </summary>
        /// <param name="xs">
        /// The source values.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The standard deviation of the source values.
        /// </returns>
        public static double StandardDeviation(this IEnumerable<double> xs, int ddof = 1)
        {
            return Math.Sqrt(xs.MeanAndVariance(ddof).Variance);
        }

        /// <summary>
        /// Computes the covariance from two sequences of values.
        /// </summary>
        /// <param name="xs">
        /// The first sequence of values.
        /// </param>
        /// <param name="ys">
        /// The second sequence of values.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The covariance of the two sequences of values.
        /// </returns>
        public static double Covariance(this IEnumerable<double> xs, IEnumerable<double> ys, int ddof = 1)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(ys, nameof(ys));

            var xMean = xs.Average();
            var yMean = ys.Average();

            var sum = 0.0;
            var count = 0;
            using (var exs = xs.GetEnumerator())
            using (var eys = ys.GetEnumerator())
            {
                while (true)
                {
                    var xsHasNext = exs.MoveNext();
                    var ysHasNext = eys.MoveNext();
                    if (xsHasNext != ysHasNext)
                    {
                        throw new ArgumentException("The number of source values must match.");
                    }

                    if (xsHasNext)
                    {
                        var x = exs.Current;
                        var y = eys.Current;
                        sum += (x - xMean) * (y - yMean);
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (count - ddof <= 0)
            {
                throw new ArgumentException("The number of source values is not sufficient.");
            }

            return sum / (count - ddof);
        }
    }
}
