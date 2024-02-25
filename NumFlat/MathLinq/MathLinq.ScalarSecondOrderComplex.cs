using System;
using System.Collections.Generic;
using System.Numerics;

namespace NumFlat
{
    public static partial class MathLinq
    {
        /// <summary>
        /// Computes the average of a sequence of values.
        /// </summary>
        /// <param name="xs">
        /// The source values.
        /// </param>
        /// <returns>
        /// The average of the sequence of values.
        /// </returns>
        public static Complex Average(this IEnumerable<Complex> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            var sum = Complex.Zero;
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

            return sum / count;
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
        public static (Complex Mean, double Variance) MeanAndVariance(this IEnumerable<Complex> xs, int ddof = 1)
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
                sum += d.MagnitudeSquared();
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
        public static (Complex Mean, double StandardDeviation) MeanAndStandardDeviation(this IEnumerable<Complex> xs, int ddof = 1)
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
        public static double Variance(this IEnumerable<Complex> xs, int ddof = 1)
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
        public static double StandardDeviation(this IEnumerable<Complex> xs, int ddof = 1)
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
        /// <remarks>
        /// The values in <paramref name="ys"/> will be conjugated.
        /// </remarks>
        public static Complex Covariance(this IEnumerable<Complex> xs, IEnumerable<Complex> ys, int ddof = 1)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(ys, nameof(ys));

            var xMean = xs.Average();
            var yMean = ys.Average();

            var sum = Complex.Zero;
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
                        sum += (x - xMean) * (y - yMean).Conjugate();
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
