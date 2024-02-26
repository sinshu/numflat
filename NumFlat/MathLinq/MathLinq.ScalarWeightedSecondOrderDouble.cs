using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat
{
    public static partial class MathLinq
    {
        /// <summary>
        /// Computes the weighted average of a sequence of values.
        /// </summary>
        /// <param name="xs">
        /// The source values.
        /// </param>
        /// <param name="weights">
        /// The weights of the source values.
        /// </param>
        /// <returns>
        /// The weighted average of the sequence of values.
        /// </returns>
        public static double Average(this IEnumerable<double> xs, IEnumerable<double> weights)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));

            var xSum = 0.0;
            var w1Sum = 0.0;
            using (var exs = xs.GetEnumerator())
            using (var eweights = weights.GetEnumerator())
            {
                while (true)
                {
                    var xsHasNext = exs.MoveNext();
                    var weightsHasNext = eweights.MoveNext();
                    if (xsHasNext != weightsHasNext)
                    {
                        throw new ArgumentException("The number of source values and weights must match.");
                    }

                    if (xsHasNext)
                    {
                        var x = exs.Current;
                        var w = eweights.Current;

                        if (w < 0)
                        {
                            throw new ArgumentException("Negative weight values are not allowed.");
                        }

                        xSum += w * x;
                        w1Sum += w;
                    }
                    else
                    {
                        break;
                    }
                }

                if (w1Sum <= 0)
                {
                    throw new ArgumentException("The number of source values is not sufficient.");
                }

                return xSum / w1Sum;
            }
        }

        /// <summary>
        /// Computes the weighted mean and variance from a sequence of values.
        /// </summary>
        /// <param name="xs">
        /// The source values.
        /// </param>
        /// <param name="weights">
        /// The weights of the source values.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The weighted mean and variance of the sequence of values.
        /// </returns>
        public static (double Mean, double Variance) MeanAndVariance(this IEnumerable<double> xs, IEnumerable<double> weights, int ddof = 1)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));

            var mean = xs.Average(weights);

            var dSum = 0.0;
            var w1Sum = 0.0;
            var w2Sum = 0.0;
            using (var exs = xs.GetEnumerator())
            using (var eweights = weights.GetEnumerator())
            {
                while (true)
                {
                    var xsHasNext = exs.MoveNext();
                    var weightsHasNext = eweights.MoveNext();
                    if (xsHasNext != weightsHasNext)
                    {
                        throw new ArgumentException("The number of source values and weights must match.");
                    }

                    if (xsHasNext)
                    {
                        var x = exs.Current;
                        var w = eweights.Current;

                        if (w < 0)
                        {
                            throw new ArgumentException("Negative weight values are not allowed.");
                        }

                        var d = x - mean;
                        dSum += w * d * d;
                        w1Sum += w;
                        w2Sum += w * w;
                    }
                    else
                    {
                        break;
                    }
                }

                var den = w1Sum - ddof * (w2Sum / w1Sum);
                if (den <= 0)
                {
                    throw new ArgumentException("The number of source vectors is not sufficient.");
                }

                return (mean, dSum / den);
            }
        }

        /// <summary>
        /// Computes the weighted mean and standard deviation from a sequence of values.
        /// </summary>
        /// <param name="xs">
        /// The source values.
        /// </param>
        /// <param name="weights">
        /// The weights of the source values.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The weighted mean and standard deviation of the sequence of values.
        /// </returns>
        public static (double Mean, double StandardDeviation) MeanAndStandardDeviation(this IEnumerable<double> xs, IEnumerable<double> weights, int ddof = 1)
        {
            var (mean, variance) = xs.MeanAndVariance(weights, ddof);
            return (mean, Math.Sqrt(variance));
        }

        /// <summary>
        /// Computes the weighted variance from a sequence of values.
        /// </summary>
        /// <param name="xs">
        /// The source values.
        /// </param>
        /// <param name="weights">
        /// The weights of the source values.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The weighted variance of the source values.
        /// </returns>
        public static double Variance(this IEnumerable<double> xs, IEnumerable<double> weights, int ddof = 1)
        {
            return xs.MeanAndVariance(weights, ddof).Variance;
        }

        /// <summary>
        /// Computes the weighted standard deviation from a sequence of values.
        /// </summary>
        /// <param name="xs">
        /// The source values.
        /// </param>
        /// <param name="weights">
        /// The weights of the source values.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The weighted standard deviation of the source values.
        /// </returns>
        public static double StandardDeviation(this IEnumerable<double> xs, IEnumerable<double> weights, int ddof = 1)
        {
            return Math.Sqrt(xs.MeanAndVariance(weights, ddof).Variance);
        }
    }
}
