using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NumFlat
{
    public static partial class MathLinq
    {
        /// <summary>
        /// Computes the mean matrix from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="destination">
        /// The destination of the mean matrix.
        /// </param>
        public static void Mean(IEnumerable<Mat<double>> xs, in Mat<double> destination)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            destination.Clear();

            var count = 0;
            foreach (var x in xs)
            {
                if (x.RowCount != destination.RowCount || x.ColCount != destination.ColCount)
                {
                    throw new ArgumentException("All the source matrices must have the same length as the destination.");
                }

                Mat.Add(destination, x, destination);
                count++;
            }

            if (count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one matrix.");
            }

            Mat.Div(destination, count, destination);
        }

        /// <summary>
        /// Computes the mean matrix from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <returns>
        /// The mean matrix.
        /// </returns>
        public static Mat<double> Mean(this IEnumerable<Mat<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            var destination = new Mat<double>();

            var count = 0;
            foreach (var x in xs)
            {
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

                Mat.Add(destination, x, destination);
                count++;
            }

            if (count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one matrix.");
            }

            Mat.Div(destination, count, destination);

            return destination;
        }

        /// <summary>
        /// Computes the pointwise variance from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
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
        public static void Variance(IEnumerable<Mat<double>> xs, in Mat<double> mean, in Mat<double> destination, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
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
            var count = 0;

            foreach (var x in xs)
            {
                if (x.RowCount != mean.RowCount || x.ColCount != mean.ColCount)
                {
                    throw new ArgumentException("All the source matrices must have the same dimensions as the mean matrix.");
                }

                AccumulateVariance(x, mean, destination);
                count++;
            }

            if (count - ddof <= 0)
            {
                throw new ArgumentException("The number of source matrices is not sufficient.");
            }

            Mat.Div(destination, count - ddof, destination);
        }

        /// <summary>
        /// Computes the mean matrix and pointwise variance from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The mean matrix and pointwise variance.
        /// </returns>
        public static (Mat<double> Mean, Mat<double> Variance) MeanAndVariance(this IEnumerable<Mat<double>> xs, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var mean = xs.Mean();
            var variance = new Mat<double>(mean.RowCount, mean.ColCount);
            Variance(xs, mean, variance, ddof);
            return (mean, variance);
        }

        /// <summary>
        /// Computes the mean matrix and pointwise variance from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <returns>
        /// The mean matrix and pointwise variance.
        /// </returns>
        public static (Mat<double> Mean, Mat<double> Variance) MeanAndVariance(this IEnumerable<Mat<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndVariance(xs, 1);
        }

        /// <summary>
        /// Computes the mean matrix and pointwise standard deviation from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The mean matrix and pointwise standard deviation.
        /// </returns>
        public static (Mat<double> Mean, Mat<double> StandardDeviation) MeanAndStandardDeviation(this IEnumerable<Mat<double>> xs, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var (mean, tmp) = xs.MeanAndVariance(ddof);
            Debug.Assert(tmp.Stride == tmp.RowCount);
            var span = tmp.Memory.Span;
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = Math.Sqrt(span[i]);
            }
            return (mean, tmp);
        }

        /// <summary>
        /// Computes the mean matrix and pointwise standard deviation from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <returns>
        /// The mean matrix and pointwise standard deviation.
        /// </returns>
        public static (Mat<double> Mean, Mat<double> StandardDeviation) MeanAndStandardDeviation(this IEnumerable<Mat<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndStandardDeviation(xs, 1);
        }

        /// <summary>
        /// Computes the pointwise variance from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The pointwise variance.
        /// </returns>
        public static Mat<double> Variance(this IEnumerable<Mat<double>> xs, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndVariance(xs, ddof).Variance;
        }

        /// <summary>
        /// Computes the pointwise variance from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <returns>
        /// The pointwise variance.
        /// </returns>
        public static Mat<double> Variance(this IEnumerable<Mat<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndVariance(xs, 1).Variance;
        }

        /// <summary>
        /// Computes the pointwise standard deviation from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The pointwise standard deviation.
        /// </returns>
        public static Mat<double> StandardDeviation(this IEnumerable<Mat<double>> xs, int ddof)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndStandardDeviation(xs, ddof).StandardDeviation;
        }

        /// <summary>
        /// Computes the pointwise standard deviation from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <returns>
        /// The pointwise standard deviation.
        /// </returns>
        public static Mat<double> StandardDeviation(this IEnumerable<Mat<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndStandardDeviation(xs).StandardDeviation;
        }

        private static void AccumulateVariance(in Mat<double> x, in Mat<double> mean, in Mat<double> destination)
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
                    sd[pd] += delta * delta;
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
