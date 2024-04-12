using System;
using System.Numerics;

namespace NumFlat
{
    public static partial class Vec
    {
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
    }
}
