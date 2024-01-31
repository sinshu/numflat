﻿using OpenBlasSharp;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NumFlat
{
    public static partial class MathLinq
    {
        /// <summary>
        /// Computes the mean vector from a vector set.
        /// </summary>
        /// <param name="xs">
        /// The vector set.
        /// </param>
        /// <param name="destination">
        /// The destination of the mean vector.
        /// </param>
        public static void Mean(this IEnumerable<Vec<Complex>> xs, Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            destination.Clear();

            var count = 0;
            foreach (var x in xs)
            {
                if (x.Count != destination.Count)
                {
                    throw new ArgumentException("All the vectors in 'xs' must be the same length as 'destination'.");
                }

                Vec.Add(destination, x, destination);
                count++;
            }

            if (count == 0)
            {
                throw new ArgumentException("'xs' must contain at least one vector.");
            }

            Vec.Div(destination, count, destination);
        }

        /// <summary>
        /// Computes the mean vector from a vector set.
        /// </summary>
        /// <param name="xs">
        /// The vector set.
        /// </param>
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
                        new ArgumentException("Zero-length vectors are not allowed.");
                    }

                    destination = new Vec<Complex>(x.Count);
                }

                if (x.Count != destination.Count)
                {
                    throw new ArgumentException("All the vectors in 'xs' must be the same length.");
                }

                Vec.Add(destination, x, destination);
                count++;
            }

            if (count == 0)
            {
                throw new ArgumentException("'xs' must contain at least one vector.");
            }

            Vec.Div(destination, count, destination);

            return destination;
        }

        /// <summary>
        /// Computes the covariance matrix from a vector set.
        /// </summary>
        /// <param name="xs">
        /// The vector set.
        /// </param>
        /// <param name="mean">
        /// The pre-computed mean vector of the vector set.
        /// </param>
        /// <param name="destination">
        /// The destination of the covariance matrix.
        /// </param>
        /// <param name="ddot">
        /// The delta degrees of freedom.
        /// </param>
        public static unsafe void Covariance(this IEnumerable<Vec<Complex>> xs, Vec<Complex> mean, Mat<Complex> destination, int ddot)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(ref mean, nameof(mean));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            if (destination.RowCount != mean.Count)
            {
                throw new ArgumentException("'destination.RowCount' must match 'mean.Count'.");
            }

            if (destination.ColCount != mean.Count)
            {
                throw new ArgumentException("'destination.ColCount' must match 'mean.Count'.");
            }

            if (ddot < 0)
            {
                throw new ArgumentException("'ddot' must be a non-negative integer.");
            }

            var one = Complex.One;

            destination.Clear();

            var count = 0;
            var buffer = ArrayPool<Complex>.Shared.Rent(mean.Count);
            try
            {
                Memory<Complex> memory = buffer;
                var centered = new Vec<Complex>(memory.Slice(0, mean.Count));
                fixed (Complex* pd = destination.Memory.Span)
                fixed (Complex* pc = centered.Memory.Span)
                {
                    foreach (var x in xs)
                    {
                        if (x.Count != mean.Count)
                        {
                            throw new ArgumentException("All the vectors in 'xs' must be the same length as 'mean'.");
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
            }
            finally
            {
                ArrayPool<Complex>.Shared.Return(buffer);
            }

            if (count - ddot <= 0)
            {
                throw new ArgumentException("The number of vectors in 'xs' is not sufficient.");
            }

            Mat.Div(destination, count - ddot, destination);
        }

        /// <summary>
        /// Computes the mean vector and covariance matrix from a vector set.
        /// </summary>
        /// <param name="xs">
        /// The vector set.
        /// </param>
        /// <param name="ddot">
        /// The delta degrees of freedom.
        /// </param>
        public static (Vec<Complex> Mean, Mat<Complex> Covariance) MeanAndCovariance(this IEnumerable<Vec<Complex>> xs, int ddot)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (ddot < 0)
            {
                throw new ArgumentException("'ddot' must be a non-negative integer.");
            }

            var mean = xs.Mean();
            var covariance = new Mat<Complex>(mean.Count, mean.Count);
            Covariance(xs, mean, covariance, ddot);
            return (mean, covariance);
        }

        /// <summary>
        /// Computes the mean vector and covariance matrix from a vector set.
        /// </summary>
        /// <param name="xs">
        /// The vector set.
        /// </param>
        public static (Vec<Complex> Mean, Mat<Complex> Covariance) MeanAndCovariance(this IEnumerable<Vec<Complex>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndCovariance(xs, 1);
        }

        /// <summary>
        /// Computes the mean vector and covariance matrix from a vector set.
        /// </summary>
        /// <param name="xs">
        /// The vector set.
        /// </param>
        /// <param name="ddot">
        /// The delta degrees of freedom.
        /// </param>
        public static Mat<Complex> Covariance(this IEnumerable<Vec<Complex>> xs, int ddot)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndCovariance(xs, ddot).Covariance;
        }

        /// <summary>
        /// Computes the mean vector and covariance matrix from a vector set.
        /// </summary>
        /// <param name="xs">
        /// The vector set.
        /// </param>
        public static Mat<Complex> Covariance(this IEnumerable<Vec<Complex>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return MeanAndCovariance(xs, 1).Covariance;
        }
    }
}