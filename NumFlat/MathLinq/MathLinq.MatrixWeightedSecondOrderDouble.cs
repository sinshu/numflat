using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NumFlat
{
    public static partial class MathLinq
    {
        /// <summary>
        /// Computes the mean matrix from a sequence of matrices and their weights.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="weights">
        /// The weights of the source matrices.
        /// </param>
        /// <param name="destination">
        /// The destination of the mean matrix.
        /// </param>
        public static void Mean(IEnumerable<Mat<double>> xs, IEnumerable<double> weights, in Mat<double> destination)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            destination.Clear();

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
                        throw new ArgumentException("The number of source matrices and weights must match.");
                    }

                    if (xsHasNext)
                    {
                        var x = exs.Current;
                        var w = eweights.Current;

                        if (x.RowCount != destination.RowCount || x.ColCount != destination.ColCount)
                        {
                            throw new ArgumentException("All the source matrices must have the same dimensions as the destination.");
                        }

                        if (w < 0)
                        {
                            throw new ArgumentException("Negative weight values are not allowed.");
                        }

                        AccumulateWeightedMean(x, w, destination);
                        w1Sum += w;
                    }
                    else
                    {
                        break;
                    }
                }

                if (w1Sum <= 0)
                {
                    throw new ArgumentException("The number of source matrices is not sufficient.");
                }

                destination.DivInplace(w1Sum);
            }
        }

        /// <summary>
        /// Computes the mean matrix from a sequence of matrices and their weights.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="weights">
        /// The weights of the source matrices.
        /// </param>
        /// <returns>
        /// The mean matrix.
        /// </returns>
        public static Mat<double> Mean(this IEnumerable<Mat<double>> xs, IEnumerable<double> weights)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));

            var destination = new Mat<double>();

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
                        throw new ArgumentException("The number of matrices and weights must match.");
                    }

                    if (xsHasNext)
                    {
                        var x = exs.Current;
                        var w = eweights.Current;

                        if (destination.RowCount == 0)
                        {
                            if (x.RowCount == 0 || x.ColCount == 0)
                            {
                                new ArgumentException("Empty matrices are not allowed.");
                            }

                            destination = new Mat<double>(x.RowCount, x.ColCount);
                        }

                        if (x.RowCount != destination.RowCount || x.ColCount != destination.ColCount)
                        {
                            throw new ArgumentException("All the matrices must have the same dimensions.");
                        }

                        if (w < 0)
                        {
                            throw new ArgumentException("Negative weight values are not allowed.");
                        }

                        AccumulateWeightedMean(x, w, destination);
                        w1Sum += w;
                    }
                    else
                    {
                        break;
                    }
                }

            }

            if (w1Sum <= 0)
            {
                throw new ArgumentException("The number of source matrices is not sufficient.");
            }

            destination.DivInplace(w1Sum);

            return destination;
        }

        private static void AccumulateWeightedMean(in Mat<double> x, double w, in Mat<double> destination)
        {
            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] += w * sx[px];
                    px++;
                    pd++;
                }
                ox += x.Stride;
                od += destination.Stride;
            }
        }
    }
}
