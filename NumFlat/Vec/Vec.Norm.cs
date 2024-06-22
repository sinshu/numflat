using System;
using System.Numerics;
using MatFlat;

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
        public static unsafe float Norm(in this Vec<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            fixed (float* px = x.Memory.Span)
            {
                return Blas.L2Norm(x.Count, px, x.Stride);
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
        public static unsafe double Norm(in this Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            fixed (double* px = x.Memory.Span)
            {
                return Blas.L2Norm(x.Count, px, x.Stride);
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
        public static unsafe double Norm(in this Vec<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            fixed (Complex* px = x.Memory.Span)
            {
                return Blas.L2Norm(x.Count, px, x.Stride);
            }
        }

        /// <summary>
        /// Computes the Lp norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <returns>
        /// The Lp norm.
        /// </returns>
        public static float Norm(in this Vec<float> x, float p)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            if (p < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(p), "'p' must be greater than or equal to one.");
            }

            var norm = 0.0F;
            foreach (var value in x)
            {
                norm += MathF.Pow(MathF.Abs(value), p);
            }
            return MathF.Pow(norm, 1.0F / p);
        }

        /// <summary>
        /// Computes the Lp norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <returns>
        /// The Lp norm.
        /// </returns>
        public static double Norm(in this Vec<double> x, double p)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            if (p < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(p), "'p' must be greater than or equal to one.");
            }

            var norm = 0.0;
            foreach (var value in x)
            {
                norm += Math.Pow(Math.Abs(value), p);
            }
            return Math.Pow(norm, 1.0 / p);
        }

        /// <summary>
        /// Computes the Lp norm.
        /// </summary>
        /// <param name="x">
        /// The targer vector.
        /// </param>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <returns>
        /// The Lp norm.
        /// </returns>
        public static double Norm(in this Vec<Complex> x, double p)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            if (p < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(p), "'p' must be greater than or equal to one.");
            }

            var norm = 0.0;
            foreach (var value in x)
            {
                norm += Math.Pow(value.Magnitude, p);
            }
            return Math.Pow(norm, 1.0 / p);
        }
    }
}
