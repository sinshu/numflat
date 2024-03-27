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
            foreach (var value in x.GetUnsafeFastIndexer())
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
            foreach (var value in x.GetUnsafeFastIndexer())
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
            foreach (var value in x.GetUnsafeFastIndexer())
            {
                norm += Math.Pow(value.Magnitude, p);
            }
            return Math.Pow(norm, 1.0 / p);
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
            foreach (var value in x.GetUnsafeFastIndexer())
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
            foreach (var value in x.GetUnsafeFastIndexer())
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
            foreach (var value in x.GetUnsafeFastIndexer())
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
            foreach (var value in x.GetUnsafeFastIndexer())
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
            foreach (var value in x.GetUnsafeFastIndexer())
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
            foreach (var value in x.GetUnsafeFastIndexer())
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
        /// Normalizes the vector.
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
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Noramlize(in Vec<float> x, in Vec<float> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            var norm = x.Norm();
            Vec.Div(x, norm, destination);
        }

        /// <summary>
        /// Normalizes the vector.
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
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Noramlize(in Vec<double> x, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            var norm = x.Norm();
            Vec.Div(x, norm, destination);
        }

        /// <summary>
        /// Normalizes the vector.
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
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Noramlize(in Vec<Complex> x, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            var norm = x.Norm();
            Vec.Div(x, norm, destination);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="x">
        /// The vector to be normalized.
        /// </param>
        /// <param name="destination">
        /// The destination of the normalized vector.
        /// </param>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Noramlize(in Vec<float> x, in Vec<float> destination, float p)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            var norm = x.Norm(p);
            Vec.Div(x, norm, destination);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="x">
        /// The vector to be normalized.
        /// </param>
        /// <param name="destination">
        /// The destination of the normalized vector.
        /// </param>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Noramlize(in Vec<double> x, in Vec<double> destination, double p)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            var norm = x.Norm(p);
            Vec.Div(x, norm, destination);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="x">
        /// The vector to be normalized.
        /// </param>
        /// <param name="destination">
        /// The destination of the normalized vector.
        /// </param>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Noramlize(in Vec<Complex> x, in Vec<Complex> destination, double p)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            var norm = x.Norm(p);
            Vec.Div(x, norm, destination);
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
