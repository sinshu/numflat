using System;
using System.Collections.Generic;

namespace NumFlat
{
    public static partial class MathLinq
    {
        /// <summary>
        /// Computes the vector sum from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="destination">
        /// The destination of the vector sum.
        /// </param>
        public static void Sum(IEnumerable<Vec<double>> xs, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            destination.Clear();

            foreach (var x in xs)
            {
                if (x.Count != destination.Count)
                {
                    throw new ArgumentException("All the source vectors must have the same length as the destination.");
                }

                destination.AddInplace(x);
            }
        }

        /// <summary>
        /// Computes the vector sum from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <returns>
        /// The vector sum.
        /// </returns>
        public static Vec<double> Sum(this IEnumerable<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            var destination = new Vec<double>();

            foreach (var x in xs)
            {
                if (destination.Count == 0)
                {
                    if (x.Count == 0)
                    {
                        new ArgumentException("Empty vectors are not allowed.");
                    }

                    destination = new Vec<double>(x.Count);
                }

                if (x.Count != destination.Count)
                {
                    throw new ArgumentException("All the vectors must have the same length.");
                }

                destination.AddInplace(x);
            }

            if (destination.Count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one vector.");
            }

            return destination;
        }

        /// <summary>
        /// Computes the weighted vector sum from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="weights">
        /// The weights of the source vectors.
        /// </param>
        /// <param name="destination">
        /// The destination of the weighted vector sum.
        /// </param>
        public static void Sum(IEnumerable<Vec<double>> xs, IEnumerable<double> weights, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            destination.Clear();

            using (var exs = xs.GetEnumerator())
            using (var eweights = weights.GetEnumerator())
            {
                while (true)
                {
                    var xsHasNext = exs.MoveNext();
                    var weightsHasNext = eweights.MoveNext();
                    if (xsHasNext != weightsHasNext)
                    {
                        throw new ArgumentException("The number of source vectors and weights must match.");
                    }

                    if (xsHasNext)
                    {
                        var x = exs.Current;
                        var w = eweights.Current;

                        if (x.Count != destination.Count)
                        {
                            throw new ArgumentException("All the source vectors must have the same length as the destination.");
                        }

                        AccumulateWeightedSum(x, w, destination);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Computes the weighted vector sum from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="weights">
        /// The weights of the source vectors.
        /// </param>
        /// <returns>
        /// The weighted vector sum.
        /// </returns>
        public static Vec<double> Sum(this IEnumerable<Vec<double>> xs, IEnumerable<double> weights)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));

            var destination = new Vec<double>();

            using (var exs = xs.GetEnumerator())
            using (var eweights = weights.GetEnumerator())
            {

                while (true)
                {
                    var xsHasNext = exs.MoveNext();
                    var weightsHasNext = eweights.MoveNext();
                    if (xsHasNext != weightsHasNext)
                    {
                        throw new ArgumentException("The number of vectors and weights must match.");
                    }

                    if (xsHasNext)
                    {
                        var x = exs.Current;
                        var w = eweights.Current;

                        if (destination.Count == 0)
                        {
                            if (x.Count == 0)
                            {
                                new ArgumentException("Empty vectors are not allowed.");
                            }

                            destination = new Vec<double>(x.Count);
                        }

                        if (x.Count != destination.Count)
                        {
                            throw new ArgumentException("All the vectors must have the same length.");
                        }

                        AccumulateWeightedSum(x, w, destination);
                    }
                    else
                    {
                        break;
                    }
                }

            }

            if (destination.Count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one vector.");
            }

            return destination;
        }
    }
}
