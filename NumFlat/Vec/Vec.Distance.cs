using System;
using System.Numerics;

namespace NumFlat
{
    public static partial class Vec
    {
        /// <summary>
        /// Computes the squared Euclidean distance between two vectors.
        /// </summary>
        /// <param name="x">
        /// The first vector.
        /// </param>
        /// <param name="y">
        /// The second vector.
        /// </param>
        /// <returns>
        /// The squared Euclidean distance.
        /// </returns>
        public static float DistanceSquared(in this Vec<float> x, in Vec<float> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var px = 0;
            var py = 0;
            var sum = 0.0F;
            while (px < sx.Length)
            {
                var d = sx[px] - sy[py];
                sum += d * d;
                px += x.Stride;
                py += y.Stride;
            }
            return sum;
        }

        /// <summary>
        /// Computes the squared Euclidean distance between two vectors.
        /// </summary>
        /// <param name="x">
        /// The first vector.
        /// </param>
        /// <param name="y">
        /// The second vector.
        /// </param>
        /// <returns>
        /// The squared Euclidean distance.
        /// </returns>
        public static double DistanceSquared(in this Vec<double> x, in Vec<double> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var px = 0;
            var py = 0;
            var sum = 0.0;
            while (px < sx.Length)
            {
                var d = sx[px] - sy[py];
                sum += d * d;
                px += x.Stride;
                py += y.Stride;
            }
            return sum;
        }

        /// <summary>
        /// Computes the squared Euclidean distance between two vectors.
        /// </summary>
        /// <param name="x">
        /// The first vector.
        /// </param>
        /// <param name="y">
        /// The second vector.
        /// </param>
        /// <returns>
        /// The squared Euclidean distance.
        /// </returns>
        public static double DistanceSquared(in this Vec<Complex> x, in Vec<Complex> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var px = 0;
            var py = 0;
            var sum = 0.0;
            while (px < sx.Length)
            {
                var d = sx[px] - sy[py];
                sum += d.MagnitudeSquared();
                px += x.Stride;
                py += y.Stride;
            }
            return sum;
        }

        /// <summary>
        /// Computes the Euclidean distance between two vectors.
        /// </summary>
        /// <param name="x">
        /// The first vector.
        /// </param>
        /// <param name="y">
        /// The second vector.
        /// </param>
        /// <returns>
        /// The Euclidean distance.
        /// </returns>
        public static float Distance(in this Vec<float> x, in Vec<float> y)
        {
            return MathF.Sqrt(DistanceSquared(x, y));
        }

        /// <summary>
        /// Computes the Euclidean distance between two vectors.
        /// </summary>
        /// <param name="x">
        /// The first vector.
        /// </param>
        /// <param name="y">
        /// The second vector.
        /// </param>
        /// <returns>
        /// The Euclidean distance.
        /// </returns>
        public static double Distance(in this Vec<double> x, in Vec<double> y)
        {
            return Math.Sqrt(DistanceSquared(x, y));
        }

        /// <summary>
        /// Computes the Euclidean distance between two vectors.
        /// </summary>
        /// <param name="x">
        /// The first vector.
        /// </param>
        /// <param name="y">
        /// The second vector.
        /// </param>
        /// <returns>
        /// The Euclidean distance.
        /// </returns>
        public static double Distance(in this Vec<Complex> x, in Vec<Complex> y)
        {
            return Math.Sqrt(DistanceSquared(x, y));
        }
    }
}
