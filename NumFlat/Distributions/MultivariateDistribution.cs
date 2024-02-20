using System;
using System.Collections.Generic;
using System.Numerics;

namespace NumFlat.Distributions
{
    /// <summary>
    /// Provides common functionality across multivariate distributions.
    /// </summary>
    public static class MultivariateDistribution
    {
        /// <summary>
        /// Computes the maximum likelihood Gaussian distribution from the source vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="regularization">
        /// The amount of regularization.
        /// This value will be added to the diagonal elements of the covariance matrix.
        /// </param>
        /// <returns>
        /// The Gaussian distribution.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed in the model fitting.
        /// </exception>
        public static Gaussian ToGaussian(this IEnumerable<Vec<double>> xs, double regularization = 0.0)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (regularization < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(regularization), "The amount of regularization must be a non-negative value.");
            }

            return new Gaussian(xs, regularization);
        }

        /// <summary>
        /// Computes the maximum likelihood Gaussian distribution from the source vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="weights">
        /// The weights of the source vectors.
        /// </param>
        /// <param name="regularization">
        /// The amount of regularization.
        /// This value will be added to the diagonal elements of the covariance matrix.
        /// </param>
        /// <returns>
        /// The Gaussian distribution.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed in the model fitting.
        /// </exception>
        public static Gaussian ToGaussian(this IEnumerable<Vec<double>> xs, IEnumerable<double> weights, double regularization = 0.0)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));

            if (regularization < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(regularization), "The amount of regularization must be a non-negative value.");
            }

            return new Gaussian(xs, weights, regularization);
        }

        /// <summary>
        /// Computes the maximum likelihood diagonal Gaussian distribution from the source vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="regularization">
        /// The amount of regularization.
        /// This value will be added to the diagonal elements of the covariance matrix.
        /// </param>
        /// <returns>
        /// The diagonal Gaussian distribution.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed in the model fitting.
        /// </exception>
        public static DiagonalGaussian ToDiagonalGaussian(this IEnumerable<Vec<double>> xs, double regularization = 0.0)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (regularization < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(regularization), "The amount of regularization must be a non-negative value.");
            }

            return new DiagonalGaussian(xs, regularization);
        }

        /// <summary>
        /// Computes the maximum likelihood diagonal Gaussian distribution from the source vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="weights">
        /// The weights of the source vectors.
        /// </param>
        /// <param name="regularization">
        /// The amount of regularization.
        /// This value will be added to the diagonal elements of the covariance matrix.
        /// </param>
        /// <returns>
        /// The diagonal Gaussian distribution.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed in the model fitting.
        /// </exception>
        public static DiagonalGaussian ToDiagonalGaussian(this IEnumerable<Vec<double>> xs, IEnumerable<double> weights, double regularization = 0.0)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (regularization < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(regularization), "The amount of regularization must be a non-negative value.");
            }

            return new DiagonalGaussian(xs, weights, regularization);
        }

        internal static void ThrowIfInvalidSize<T>(IMultivariateDistribution<T> distribution, in Vec<T> x, string name) where T : unmanaged, INumberBase<T>
        {
            if (x.Count != distribution.Dimension)
            {
                throw new ArgumentException($"The PDF requires the length of the vector to be {distribution.Dimension}, but was {x.Count}.", name);
            }
        }
    }
}
