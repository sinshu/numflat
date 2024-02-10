using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
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
        public static void Mean(IEnumerable<Vec<Complex>> xs, in Vec<Complex> destination)
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
        public static Vec<Complex> Mean(this IEnumerable<Vec<Complex>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            var destination = new Vec<Complex>();

            var count = 0;
            foreach (var x in xs)
            {
                if (destination.Count == 0)
                {
                    if (x.Count == 0)
                    {
                        new ArgumentException("Empty vectors are not allowed.");
                    }

                    destination = new Vec<Complex>(x.Count);
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
        public static void Variance(IEnumerable<Vec<Complex>> xs, in Vec<Complex> mean, in Vec<double> destination, int ddof = 1)
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
        public static unsafe void Covariance(IEnumerable<Vec<Complex>> xs, in Vec<Complex> mean, in Mat<Complex> destination, int ddof = 1)
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

            var one = Complex.One;

            using var ucentered = new TemporalVector<Complex>(mean.Count);
            ref readonly var centered = ref ucentered.Item;

            destination.Clear();
            var count = 0;

            fixed (Complex* pd = destination.Memory.Span)
            fixed (Complex* pc = centered.Memory.Span)
            {
                foreach (var x in xs)
                {
                    if (x.Count != mean.Count)
                    {
                        throw new ArgumentException("All the source vectors must have the same length as the mean vector.");
                    }

                    Vec.Sub(x, mean, centered);
                    Blas.Zher(
                        Order.ColMajor,
                        Uplo.Lower,
                        destination.RowCount,
                        1.0,
                        pc, centered.Stride,
                        pd, destination.Stride);
                    count++;
                }
            }

            for (var col = 1; col < destination.ColCount; col++)
            {
                for (var row = 0; row < col; row++)
                {
                    destination[row, col] = destination[col, row].Conjugate();
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
        public static (Vec<Complex> Mean, Vec<double> Variance) MeanAndVariance(this IEnumerable<Vec<Complex>> xs, int ddof = 1)
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
        public static (Vec<Complex> Mean, Mat<Complex> Covariance) MeanAndCovariance(this IEnumerable<Vec<Complex>> xs, int ddof = 1)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var mean = xs.Mean();
            var covariance = new Mat<Complex>(mean.Count, mean.Count);
            Covariance(xs, mean, covariance, ddof);
            return (mean, covariance);
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
        public static (Vec<Complex> Mean, Vec<double> StandardDeviation) MeanAndStandardDeviation(this IEnumerable<Vec<Complex>> xs, int ddof = 1)
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
        public static Vec<double> Variance(this IEnumerable<Vec<Complex>> xs, int ddof = 1)
        {
            return MeanAndVariance(xs, ddof).Variance;
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
        public static Mat<Complex> Covariance(this IEnumerable<Vec<Complex>> xs, int ddof = 1)
        {
            return MeanAndCovariance(xs, ddof).Covariance;
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
        public static Vec<double> StandardDeviation(this IEnumerable<Vec<Complex>> xs, int ddof = 1)
        {
            return MeanAndStandardDeviation(xs, ddof).StandardDeviation;
        }

        private static void AccumulateVariance(in Vec<Complex> x, in Vec<Complex> mean, in Vec<double> destination)
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
                sd[pd] += delta.MagnitudeSquared();
                px += x.Stride;
                pm += mean.Stride;
                pd += destination.Stride;
            }
        }
    }
}
