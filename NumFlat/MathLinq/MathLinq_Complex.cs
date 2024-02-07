﻿using System;
using System.Buffers;
using System.Collections.Generic;
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
        public static void Mean(IEnumerable<Vec<Complex>> xs, Vec<Complex> destination)
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
        public static unsafe void Covariance(IEnumerable<Vec<Complex>> xs, Vec<Complex> mean, Mat<Complex> destination, int ddof)
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

            var centeredLength = mean.Count;
            using var centeredBuffer = MemoryPool<Complex>.Shared.Rent(centeredLength);
            var centered = new Vec<Complex>(centeredBuffer.Memory.Slice(0, centeredLength));

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
                    Blas.Zgerc(
                        Order.ColMajor,
                        destination.RowCount, destination.ColCount,
                        &one,
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
        public static (Vec<Complex> Mean, Mat<Complex> Covariance) MeanAndCovariance(this IEnumerable<Vec<Complex>> xs, int ddof)
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
        /// Computes the mean vector and covariance matrix from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <returns>
        /// The mean vector and covariance matrix.
        /// </returns>
        public static (Vec<Complex> Mean, Mat<Complex> Covariance) MeanAndCovariance(this IEnumerable<Vec<Complex>> xs)
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
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The covariance matrix.
        /// </returns>
        public static Mat<Complex> Covariance(this IEnumerable<Vec<Complex>> xs, int ddof)
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
        public static Mat<Complex> Covariance(this IEnumerable<Vec<Complex>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndCovariance(xs, 1).Covariance;
        }
    }
}
