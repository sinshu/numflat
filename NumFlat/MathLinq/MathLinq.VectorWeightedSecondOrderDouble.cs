using System;
using System.Collections.Generic;
using OpenBlasSharp;

namespace NumFlat
{
    public static partial class MathLinq
    {
        /// <summary>
        /// Computes the mean vector from a sequence of vectors and their weights.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="weights">
        /// The weights of the source vectors.
        /// </param>
        /// <param name="destination">
        /// The destination of the mean vector.
        /// </param>
        public static void Mean(IEnumerable<Vec<double>> xs, IEnumerable<double> weights, in Vec<double> destination)
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

                        if (w < 0)
                        {
                            throw new ArgumentException("Negative weight values are not allowed.");
                        }

                        AccumulateMean(x, w, destination);
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

                Vec.Div(destination, w1Sum, destination);
            }
        }

        /// <summary>
        /// Computes the mean vector from a sequence of vectors and their weights.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="weights">
        /// The weights of the source vectors.
        /// </param>
        /// <returns>
        /// The mean vector.
        /// </returns>
        public static unsafe Vec<double> Mean(this IEnumerable<Vec<double>> xs, IEnumerable<double> weights)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));

            var destination = new Vec<double>();

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

                        if (w < 0)
                        {
                            throw new ArgumentException("Negative weight values are not allowed.");
                        }

                        AccumulateMean(x, w, destination);
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
                throw new ArgumentException("The number of source vectors is not sufficient.");
            }

            Vec.Div(destination, w1Sum, destination);

            return destination;
        }

        /// <summary>
        /// Computes the covariance matrix from a sequence of vectors and their weights.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="weights">
        /// The weights of the source vectors.
        /// </param>
        /// <param name="mean">
        /// The pre-computed mean vector of the source vectors.
        /// </param>
        /// <param name="destination">
        /// The destination of the covariance matrix.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        public static unsafe void Covariance(IEnumerable<Vec<double>> xs, IEnumerable<double> weights, in Vec<double> mean, in Mat<double> destination, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));
            ThrowHelper.ThrowIfEmpty(mean, nameof(mean));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.RowCount != mean.Count)
            {
                throw new ArgumentException("The number of rows of the destination must match the length of the mean vector.");
            }

            if (destination.ColCount != mean.Count)
            {
                throw new ArgumentException("The number of columns of the destination must match the length of the mean vector.");
            }

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            using var ucentered = new TemporalVector<double>(mean.Count);
            ref readonly var centered = ref ucentered.Item;

            destination.Clear();

            var w1Sum = 0.0;
            var w2Sum = 0.0;
            using (var exs = xs.GetEnumerator())
            using (var eweights = weights.GetEnumerator())
            {
                fixed (double* pd = destination.Memory.Span)
                fixed (double* pc = centered.Memory.Span)
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

                            if (x.Count != mean.Count)
                            {
                                throw new ArgumentException("All the source vectors must have the same length as the mean vector.");
                            }

                            if (w < 0)
                            {
                                throw new ArgumentException("Negative weight values are not allowed.");
                            }

                            Vec.Sub(x, mean, centered);
                            Blas.Dsyr(
                                Order.ColMajor,
                                Uplo.Lower,
                                destination.RowCount,
                                w,
                                pc, centered.Stride,
                                pd, destination.Stride);
                            w1Sum += w;
                            w2Sum += w * w;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                for (var col = 1; col < destination.ColCount; col++)
                {
                    for (var row = 0; row < col; row++)
                    {
                        destination[row, col] = destination[col, row];
                    }
                }

                var den = w1Sum - ddof * (w2Sum / w1Sum);
                if (den <= 0)
                {
                    throw new ArgumentException("The number of source vectors is not sufficient.");
                }

                Mat.Div(destination, den, destination);
            }
        }

        private static void AccumulateMean(in Vec<double> x, double w, in Vec<double> destination)
        {
            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] += w * sx[px];
                px += x.Stride;
                pd += destination.Stride;
            }
        }
    }
}
