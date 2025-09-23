using System;

namespace NumFlat
{
    /// <summary>
    /// Represents a kernel function between two objects.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the first argument.
    /// </typeparam>
    /// <typeparam name="U">
    /// The type of the second argument.
    /// </typeparam>
    /// <param name="x">
    /// The first object to compare.
    /// </param>
    /// <param name="y">
    /// The second object to compare.
    /// </param>
    /// <returns>
    /// The computed kernel value.
    /// </returns>
    public delegate double Kernel<T, U>(T x, U y);



    /// <summary>
    /// Provides kernel functions.
    /// </summary>
    public static class Kernel
    {
        /// <summary>
        /// Gets the linear kernel.
        /// </summary>
        public static Kernel<Vec<double>, Vec<double>> Linear => (Vec<double> x, Vec<double> y) =>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            return x * y;
        };

        /// <summary>
        /// Creates a polynomial kernel.
        /// </summary>
        /// <param name="degree">
        /// The degree of the polynomial.
        /// </param>
        /// <param name="constant">
        /// The constant term of the polynomial.
        /// </param>
        /// <returns>
        /// The polynomial kernel.
        /// </returns>
        public static Kernel<Vec<double>, Vec<double>> Polynomial(int degree, double constant)
        {
            if (degree <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(degree), "The degree must be greater than zero.");
            }

            return (Vec<double> x, Vec<double> y) =>
            {
                ThrowHelper.ThrowIfEmpty(x, nameof(x));
                ThrowHelper.ThrowIfEmpty(y, nameof(y));
                ThrowHelper.ThrowIfDifferentSize(x, y);

                return Math.Pow(x * y + constant, degree);
            };
        }

        /// <summary>
        /// Creates a Gaussian radial basis function (RBF) kernel.
        /// </summary>
        /// <param name="gamma">
        /// The gamma parameter of the kernel.
        /// </param>
        /// <returns>
        /// The Gaussian kernel.
        /// </returns>
        public static Kernel<Vec<double>, Vec<double>> Gaussian(double gamma)
        {
            if (gamma <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gamma), "The gamma parameter must be greater than zero.");
            }

            return (Vec<double> x, Vec<double> y) =>
            {
                ThrowHelper.ThrowIfEmpty(x, nameof(x));
                ThrowHelper.ThrowIfEmpty(y, nameof(y));
                ThrowHelper.ThrowIfDifferentSize(x, y);

                return Math.Exp(-gamma * x.DistanceSquared(y));
            };
        }
    }
}
