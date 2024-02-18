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
        /// <returns>
        /// The Gaussian distribution.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed in the model fitting.
        /// </exception>
        public static Gaussian ToGaussian(this IEnumerable<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            Vec<double> mean;
            Mat<double> covariance;
            try
            {
                (mean, covariance) = xs.MeanAndCovariance();
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the covariance matrix.", e);
            }

            return new Gaussian(mean, covariance);
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
        /// <returns>
        /// The Gaussian distribution.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed in the model fitting.
        /// </exception>
        public static Gaussian ToGaussian(this IEnumerable<Vec<double>> xs, IEnumerable<double> weights)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            Vec<double> mean;
            Mat<double> covariance;
            try
            {
                (mean, covariance) = xs.MeanAndCovariance(weights);
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the covariance matrix.", e);
            }

            return new Gaussian(mean, covariance);
        }

        /// <summary>
        /// Computes the maximum likelihood diagonal Gaussian distribution from the source vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <returns>
        /// The diagonal Gaussian distribution.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed in the model fitting.
        /// </exception>
        public static DiagonalGaussian ToDiagonalGaussian(this IEnumerable<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            Vec<double> mean;
            Vec<double> variance;
            try
            {
                (mean, variance) = xs.MeanAndVariance();
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the pointwise variance.", e);
            }

            return new DiagonalGaussian(mean, variance);
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
        /// <returns>
        /// The diagonal Gaussian distribution.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed in the model fitting.
        /// </exception>
        public static DiagonalGaussian ToDiagonalGaussian(this IEnumerable<Vec<double>> xs, IEnumerable<double> weights)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            Vec<double> mean;
            Vec<double> variance;
            try
            {
                (mean, variance) = xs.MeanAndVariance(weights);
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the pointwise variance.", e);
            }

            return new DiagonalGaussian(mean, variance);
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
