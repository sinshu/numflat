using System;
using System.Buffers;
using System.Collections.Generic;
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
        public static void Mean(IEnumerable<Vec<double>> xs, Vec<double> destination)
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
        /// <param name="ddot">
        /// The delta degrees of freedom.
        /// </param>
        public static unsafe void Covariance(IEnumerable<Vec<double>> xs, Vec<double> mean, Mat<double> destination, int ddot)
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

            if (ddot < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var centeredLength = mean.Count;
            using var centeredBuffer = MemoryPool<double>.Shared.Rent(centeredLength);
            var centered = new Vec<double>(centeredBuffer.Memory.Slice(0, centeredLength));

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

            if (count - ddot <= 0)
            {
                throw new ArgumentException("The number of source vectors is not sufficient.");
            }

            Mat.Div(destination, count - ddot, destination);
        }

        /// <summary>
        /// Computes the mean vector and covariance matrix from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ddot">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The mean vector and covariance matrix.
        /// </returns>
        public static (Vec<double> Mean, Mat<double> Covariance) MeanAndCovariance(this IEnumerable<Vec<double>> xs, int ddot)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (ddot < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var mean = xs.Mean();
            var covariance = new Mat<double>(mean.Count, mean.Count);
            Covariance(xs, mean, covariance, ddot);
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
        /// Computes the covariance matrix from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ddot">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The covariance matrix.
        /// </returns>
        public static Mat<double> Covariance(this IEnumerable<Vec<double>> xs, int ddot)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndCovariance(xs, ddot).Covariance;
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
    }
}
