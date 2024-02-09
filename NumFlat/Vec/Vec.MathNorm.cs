using System;
using System.Numerics;
using OpenBlasSharp;

namespace NumFlat
{
    public static partial class Vec
    {
        /// <summary>
        /// Computes the L2 norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The L2 norm.
        /// </returns>
        public static unsafe float L2Norm(in this Vec<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            fixed (float* px = x.Memory.Span)
            {
                return Blas.Snrm2(x.Count, px, x.Stride);
            }
        }

        /// <summary>
        /// Computes the L2 norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The L2 norm.
        /// </returns>
        public static unsafe double L2Norm(in this Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            fixed (double* px = x.Memory.Span)
            {
                return Blas.Dnrm2(x.Count, px, x.Stride);
            }
        }

        /// <summary>
        /// Computes the L2 norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The L2 norm.
        /// </returns>
        public static unsafe double L2Norm(in this Vec<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            fixed (Complex* px = x.Memory.Span)
            {
                return Blas.Dznrm2(x.Count, px, x.Stride);
            }
        }

        /// <summary>
        /// Computes the L1 norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The L1 norm.
        /// </returns>
        public static float L1Norm(in this Vec<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var norm = 0.0F;
            foreach (var value in x)
            {
                norm += MathF.Abs(value);
            }
            return norm;
        }

        /// <summary>
        /// Computes the L1 norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The L1 norm.
        /// </returns>
        public static double L1Norm(in this Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var norm = 0.0;
            foreach (var value in x)
            {
                norm += Math.Abs(value);
            }
            return norm;
        }

        /// <summary>
        /// Computes the L1 norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The L1 norm.
        /// </returns>
        public static double L1Norm(in this Vec<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var norm = 0.0;
            foreach (var value in x)
            {
                norm += value.Magnitude;
            }
            return norm;
        }

        /// <summary>
        /// Computes the infinity norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The infinity norm.
        /// </returns>
        public static float InfinityNorm(in this Vec<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0F;
            foreach (var value in x)
            {
                var current = MathF.Abs(value);
                if (current > max)
                {
                    max = current;
                }
            }
            return max;
        }

        /// <summary>
        /// Computes the infinity norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The infinity norm.
        /// </returns>
        public static double InfinityNorm(in this Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0;
            foreach (var value in x)
            {
                var current = Math.Abs(value);
                if (current > max)
                {
                    max = current;
                }
            }
            return max;
        }

        /// <summary>
        /// Computes the infinity norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <returns>
        /// The infinity norm.
        /// </returns>
        public static double InfinityNorm(in this Vec<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0;
            foreach (var value in x)
            {
                var current = value.Magnitude;
                if (current > max)
                {
                    max = current;
                }
            }
            return max;
        }

        /// <summary>
        /// Normalize the vector.
        /// </summary>
        /// <param name="x">
        /// The vector to be normalized.
        /// </param>
        /// <param name="destination">
        /// The destination of the normalized vector.
        /// </param>
        /// <remarks>
        /// The L2 norm of the destination vector will be 1.
        /// </remarks>
        public static void Noramlize(in Vec<float> x, in Vec<float> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            var norm = x.L2Norm();
            Vec.Div(x, norm, destination);
        }

        /// <summary>
        /// Normalize the vector.
        /// </summary>
        /// <param name="x">
        /// The vector to be normalized.
        /// </param>
        /// <param name="destination">
        /// The destination of the normalized vector.
        /// </param>
        /// <remarks>
        /// The L2 norm of the destination vector will be 1.
        /// </remarks>
        public static void Noramlize(in Vec<double> x, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            var norm = x.L2Norm();
            Vec.Div(x, norm, destination);
        }

        /// <summary>
        /// Normalize the vector.
        /// </summary>
        /// <param name="x">
        /// The vector to be normalized.
        /// </param>
        /// <param name="destination">
        /// The destination of the normalized vector.
        /// </param>
        /// <remarks>
        /// The L2 norm of the destination vector will be 1.
        /// </remarks>
        public static void Noramlize(in Vec<Complex> x, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            var norm = x.L2Norm();
            Vec.Div(x, norm, destination);
        }
    }
}
