using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenBlasSharp;

namespace NumFlat
{
    public static partial class MathLinq
    {
        /// <summary>
        /// Computes the mean vector from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="destination">
        /// The destination of the mean vector.
        /// </param>
        public static void Mean(IEnumerable<Vec<double>> xs, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            destination.Clear();

            var count = 0;
            foreach (var x in xs)
            {
                if (x.Count != destination.Count)
                {
                    throw new ArgumentException("All the source vectors must have the same length as the destination.");
                }

                Vec.Add(destination, x, destination);
                count++;
            }

            if (count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one vector.");
            }

            Vec.Div(destination, count, destination);
        }

        /// <summary>
        /// Computes the mean vector from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <returns>
        /// The mean vector.
        /// </returns>
        public static Vec<double> Mean(this IEnumerable<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            var destination = new Vec<double>();

            var count = 0;
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

                Vec.Add(destination, x, destination);
                count++;
            }

            if (count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one vector.");
            }

            Vec.Div(destination, count, destination);

            return destination;
        }

        /// <summary>
        /// Computes the pointwise variance from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="mean">
        /// The pre-computed mean vector of the source vectors.
        /// </param>
        /// <param name="destination">
        /// The destination of the pointwise variance.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        public static void Variance(IEnumerable<Vec<double>> xs, in Vec<double> mean, in Vec<double> destination, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(mean, nameof(mean));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.Count != mean.Count)
            {
                throw new ArgumentException("The length of the destination must match the length of the mean vector.");
            }

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            destination.Clear();
            var count = 0;

            foreach (var x in xs)
            {
                if (x.Count != mean.Count)
                {
                    throw new ArgumentException("All the source vectors must have the same length as the mean vector.");
                }

                AccumulateVariance(x, mean, destination);
                count++;
            }

            if (count - ddof <= 0)
            {
                throw new ArgumentException("The number of source vectors is not sufficient.");
            }

            Vec.Div(destination, count - ddof, destination);
        }

        /// <summary>
        /// Computes the covariance matrix from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
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
        public static unsafe void Covariance(IEnumerable<Vec<double>> xs, in Vec<double> mean, in Mat<double> destination, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
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
            var count = 0;

            fixed (double* pd = destination.Memory.Span)
            fixed (double* pc = centered.Memory.Span)
            {
                foreach (var x in xs)
                {
                    if (x.Count != mean.Count)
                    {
                        throw new ArgumentException("All the source vectors must have the same length as the mean vector.");
                    }

                    Vec.Sub(x, mean, centered);
                    Blas.Dger(
                        Order.ColMajor,
                        destination.RowCount, destination.ColCount,
                        1.0,
                        pc, centered.Stride,
                        pc, centered.Stride,
                        pd, destination.Stride);
                    count++;
                }
            }

            if (count - ddof <= 0)
            {
                throw new ArgumentException("The number of source vectors is not sufficient.");
            }

            Mat.Div(destination, count - ddof, destination);
        }

        /// <summary>
        /// Computes the mean vector and pointwise variance from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The mean vector and pointwise variance.
        /// </returns>
        public static (Vec<double> Mean, Vec<double> Variance) MeanAndVariance(this IEnumerable<Vec<double>> xs, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var mean = xs.Mean();
            var variance = new Vec<double>(mean.Count);
            Variance(xs, mean, variance, ddof);
            return (mean, variance);
        }

        /// <summary>
        /// Computes the mean vector and pointwise variance from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <returns>
        /// The mean vector and pointwise variance.
        /// </returns>
        public static (Vec<double> Mean, Vec<double> Variance) MeanAndVariance(this IEnumerable<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndVariance(xs, 1);
        }

        /// <summary>
        /// Computes the mean vector and covariance matrix from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The mean vector and covariance matrix.
        /// </returns>
        public static (Vec<double> Mean, Mat<double> Covariance) MeanAndCovariance(this IEnumerable<Vec<double>> xs, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var mean = xs.Mean();
            var covariance = new Mat<double>(mean.Count, mean.Count);
            Covariance(xs, mean, covariance, ddof);
            return (mean, covariance);
        }

        /// <summary>
        /// Computes the mean vector and covariance matrix from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <returns>
        /// The mean vector and covariance matrix.
        /// </returns>
        public static (Vec<double> Mean, Mat<double> Covariance) MeanAndCovariance(this IEnumerable<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndCovariance(xs, 1);
        }

        /// <summary>
        /// Computes the mean vector and pointwise standard deviation from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The mean vector and pointwise standard deviation.
        /// </returns>
        public static (Vec<double> Mean, Vec<double> StandardDeviation) MeanAndStandardDeviation(this IEnumerable<Vec<double>> xs, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var (mean, tmp) = xs.MeanAndVariance(ddof);
            Debug.Assert(tmp.Stride == 1);
            var span = tmp.Memory.Span;
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = Math.Sqrt(span[i]);
            }
            return (mean, tmp);
        }

        /// <summary>
        /// Computes the mean vector and pointwise standard deviation from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <returns>
        /// The mean vector and pointwise standard deviation.
        /// </returns>
        public static (Vec<double> Mean, Vec<double> StandardDeviation) MeanAndStandardDeviation(this IEnumerable<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndStandardDeviation(xs, 1);
        }

        /// <summary>
        /// Computes the pointwise variance from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The pointwise variance.
        /// </returns>
        public static Vec<double> Variance(this IEnumerable<Vec<double>> xs, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndVariance(xs, ddof).Variance;
        }

        /// <summary>
        /// Computes the pointwise variance from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <returns>
        /// The pointwise variance.
        /// </returns>
        public static Vec<double> Variance(this IEnumerable<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndVariance(xs, 1).Variance;
        }

        /// <summary>
        /// Computes the covariance matrix from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The covariance matrix.
        /// </returns>
        public static Mat<double> Covariance(this IEnumerable<Vec<double>> xs, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndCovariance(xs, ddof).Covariance;
        }

        /// <summary>
        /// Computes the covariance matrix from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <returns>
        /// The covariance matrix.
        /// </returns>
        public static Mat<double> Covariance(this IEnumerable<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndCovariance(xs, 1).Covariance;
        }

        /// <summary>
        /// Computes the pointwise standard deviation from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The pointwise standard deviation.
        /// </returns>
        public static Vec<double> StandardDeviation(this IEnumerable<Vec<double>> xs, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndStandardDeviation(xs, ddof).StandardDeviation;
        }

        /// <summary>
        /// Computes the pointwise standard deviation from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <returns>
        /// The pointwise standard deviation.
        /// </returns>
        public static Vec<double> StandardDeviation(this IEnumerable<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndStandardDeviation(xs).StandardDeviation;
        }

        private static void AccumulateVariance(in Vec<double> x, in Vec<double> mean, in Vec<double> destination)
        {
            var sx = x.Memory.Span;
            var sm = mean.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var pm = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                var delta = sx[px] - sm[pm];
                sd[pd] += delta * delta;
                px += x.Stride;
                pm += mean.Stride;
                pd += destination.Stride;
            }
        }
    }
}
