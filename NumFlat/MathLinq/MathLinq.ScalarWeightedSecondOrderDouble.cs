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
                    throw new ArgumentException("The number of source vectors is not sufficient.");
                }

                return xSum / w1Sum;
            }
        }
    }
}
