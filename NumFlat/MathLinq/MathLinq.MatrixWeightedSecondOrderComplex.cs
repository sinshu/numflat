﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

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
        public static void Mean(IEnumerable<Mat<Complex>> xs, IEnumerable<double> weights, in Mat<Complex> destination)
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

                        AccumulateWeightedSum(x, w, destination);
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
        public static Mat<Complex> Mean(this IEnumerable<Mat<Complex>> xs, IEnumerable<double> weights)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));

            var destination = new Mat<Complex>();

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
                                throw new ArgumentException("Empty matrices are not allowed.");
                            }

                            destination = new Mat<Complex>(x.RowCount, x.ColCount);
                        }

                        if (x.RowCount != destination.RowCount || x.ColCount != destination.ColCount)
                        {
                            throw new ArgumentException("All the matrices must have the same dimensions.");
                        }

                        if (w < 0)
                        {
                            throw new ArgumentException("Negative weight values are not allowed.");
                        }

                        AccumulateWeightedSum(x, w, destination);
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

        /// <summary>
        /// Computes the pointwise variance from a sequence of matrices and their weights.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="weights">
        /// The weights of the source matrices.
        /// </param>
        /// <param name="mean">
        /// The pre-computed mean matrix of the source matrices.
        /// </param>
        /// <param name="destination">
        /// The destination of the pointwise variance.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        public static void Variance(IEnumerable<Mat<Complex>> xs, IEnumerable<double> weights, in Mat<Complex> mean, in Mat<double> destination, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));
            ThrowHelper.ThrowIfEmpty(mean, nameof(mean));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.RowCount != mean.RowCount || destination.ColCount != mean.ColCount)
            {
                throw new ArgumentException("The dimensions of the destination must match the dimensions of the mean matrix.");
            }

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            destination.Clear();

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
                        throw new ArgumentException("The number of source matrices and weights must match.");
                    }

                    if (xsHasNext)
                    {
                        var x = exs.Current;
                        var w = eweights.Current;

                        if (x.RowCount != mean.RowCount || x.ColCount != mean.ColCount)
                        {
                            throw new ArgumentException("All the source matrices must have the same dimensions as the mean matrix.");
                        }

                        if (w < 0)
                        {
                            throw new ArgumentException("Negative weight values are not allowed.");
                        }

                        AccumulateWeightedVariance(x, w, mean, destination);
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
                    throw new ArgumentException("The number of source matrices is not sufficient.");
                }

                destination.DivInplace(den);
            }
        }

        /// <summary>
        /// Computes the mean matrix and pointwise variance from a sequence of matrices and their weights.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="weights">
        /// The weights of the source matrices.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The mean matrix and pointwise variance.
        /// </returns>
        public static (Mat<Complex> Mean, Mat<double> Variance) MeanAndVariance(this IEnumerable<Mat<Complex>> xs, IEnumerable<double> weights, int ddof = 1)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var mean = xs.Mean(weights);
            var variance = new Mat<double>(mean.RowCount, mean.ColCount);
            Variance(xs, weights, mean, variance, ddof);
            return (mean, variance);
        }

        /// <summary>
        /// Computes the mean matrix and pointwise standard deviation from a sequence of matrices and their weights.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="weights">
        /// The weights of the source matrices.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The mean matrix and pointwise standard deviation.
        /// </returns>
        public static (Mat<Complex> Mean, Mat<double> StandardDeviation) MeanAndStandardDeviation(this IEnumerable<Mat<Complex>> xs, IEnumerable<double> weights, int ddof = 1)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var (mean, tmp) = xs.MeanAndVariance(weights, ddof);
            Debug.Assert(tmp.Stride == tmp.RowCount);
            var span = tmp.Memory.Span;
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = Math.Sqrt(span[i]);
            }
            return (mean, tmp);
        }

        /// <summary>
        /// Computes the mean matrix and pointwise variance from a sequence of matrices and their weights.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="weights">
        /// The weights of the source matrices.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The pointwise variance.
        /// </returns>
        public static Mat<double> Variance(this IEnumerable<Mat<Complex>> xs, IEnumerable<double> weights, int ddof = 1)
        {
            return MeanAndVariance(xs, weights, ddof).Variance;
        }

        /// <summary>
        /// Computes the mean matrix and pointwise standard deviation from a sequence of matrices and their weights.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="weights">
        /// The weights of the source matrices.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The pointwise standard deviation.
        /// </returns>
        public static Mat<double> StandardDeviation(this IEnumerable<Mat<Complex>> xs, IEnumerable<double> weights, int ddof = 1)
        {
            return MeanAndStandardDeviation(xs, weights, ddof).StandardDeviation;
        }

        private static void AccumulateWeightedVariance(in Mat<Complex> x, double w, in Mat<Complex> mean, in Mat<double> destination)
        {
            var sx = x.Memory.Span;
            var sm = mean.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var om = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var pm = om;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    var delta = sx[px] - sm[pm];
                    sd[pd] += w * delta.MagnitudeSquared();
                    px++;
                    pm++;
                    pd++;
                }
                ox += x.Stride;
                om += mean.Stride;
                od += destination.Stride;
            }
        }
    }
}
