﻿using NumFlat.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.Clustering
{
    /// <summary>
    /// Provides common functionality across classification methods.
    /// </summary>
    public static class Clustering
    {
        /// <summary>
        /// Clusters the feature vectors using the k-means algorithm.
        /// </summary>
        /// <param name="xs">
        /// The source feature vectors.
        /// </param>
        /// <param name="clusterCount">
        /// The number of desired clusters.
        /// </param>
        /// <param name="tryCount">
        /// Runs the k-means algorithm a specified number of times and selects the model with the lowest error.
        /// </param>
        /// <param name="random">
        /// A random number generator for the k-means++ initialization.
        /// If null, a <see cref="Random"/> object instantiated with the default constructor will be used.
        /// </param>
        /// <returns>
        /// A k-means model computed from the source vectors.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public static KMeans ToKMeans(this IReadOnlyList<Vec<double>> xs, int clusterCount, int tryCount = 3, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            if (clusterCount <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(clusterCount), "The number of clusters must be greater than or equal to two.");
            }

            if (tryCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tryCount), "The number of attempts must be greater than or equal to one.");
            }

            return new KMeans(xs, clusterCount, tryCount, random);
        }

        /// <summary>
        /// Clusters the feature vectors as a Gaussian mixture model (GMM) using the expectation-maximization (EM) algorithm.
        /// </summary>
        /// <param name="xs">
        /// The source feature vectors.
        /// </param>
        /// <param name="clusterCount">
        /// The number of desired clusters.
        /// </param>
        /// <param name="regularization">
        /// The amount of regularization.
        /// This value will be added to the diagonal elements of the covariance matrix.
        /// </param>
        /// <param name="kMeansTryCount">
        /// Runs the k-means algorithm a specified number of times and selects the initial model with the lowest error.
        /// </param>
        /// <param name="random">
        /// A random number generator for the k-means++ initialization.
        /// If null, a <see cref="Random"/> object instantiated with the default constructor will be used.
        /// </param>
        /// <returns>
        /// A GMM computed from the source vectors.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        /// <remarks>
        /// An initial GMM is constructed with the k-means algorithm.
        /// </remarks>
        public static GaussianMixtureModel ToGmm(this IReadOnlyList<Vec<double>> xs, int clusterCount, double regularization = 1.0E-6, int kMeansTryCount = 3, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            if (clusterCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(clusterCount), "The number of clusters must be greater than or equal to one.");
            }

            if (regularization < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(regularization), "The amount of regularization must be a non-negative value.");
            }

            if (kMeansTryCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(kMeansTryCount), "The number of attempts must be greater than or equal to one.");
            }

            return new GaussianMixtureModel(xs, clusterCount, regularization, kMeansTryCount, random);
        }

        /// <summary>
        /// Clusters the feature vectors as a diagonal Gaussian mixture model (diagonal GMM) using the expectation-maximization (EM) algorithm.
        /// </summary>
        /// <param name="xs">
        /// The source feature vectors.
        /// </param>
        /// <param name="clusterCount">
        /// The number of desired clusters.
        /// </param>
        /// <param name="regularization">
        /// The amount of regularization.
        /// This value will be added to the diagonal elements of the covariance matrix.
        /// </param>
        /// <param name="kMeansTryCount">
        /// Runs the k-means algorithm a specified number of times and selects the initial model with the lowest error.
        /// </param>
        /// <param name="random">
        /// A random number generator for the k-means++ initialization.
        /// If null, a <see cref="Random"/> object instantiated with the default constructor will be used.
        /// </param>
        /// <returns>
        /// A GMM computed from the source vectors.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        /// <remarks>
        /// An initial GMM is constructed with the k-means algorithm.
        /// </remarks>
        public static DiagonalGaussianMixtureModel ToDiagonalGmm(this IReadOnlyList<Vec<double>> xs, int clusterCount, double regularization = 1.0E-6, int kMeansTryCount = 3, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            if (clusterCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(clusterCount), "The number of clusters must be greater than or equal to one.");
            }

            if (regularization < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(regularization), "The amount of regularization must be a non-negative value.");
            }

            if (kMeansTryCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(kMeansTryCount), "The number of attempts must be greater than or equal to one.");
            }

            return new DiagonalGaussianMixtureModel(xs, clusterCount, regularization, kMeansTryCount, random);
        }

        /// <summary>
        /// Converts a k-means model to a Gaussian mixture model.
        /// </summary>
        /// <param name="kMeans">
        /// The source k-means model.
        /// </param>
        /// <param name="xs">
        /// The source feature vectors.
        /// </param>
        /// <returns>
        /// The Gaussian mixture model converted from the k-means model.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public static GaussianMixtureModel ToGmm(this KMeans kMeans, IReadOnlyList<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(kMeans, nameof(kMeans));
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            try
            {
                var groups = ArgumentHelper.GroupByClassIndex(xs, xs.Select(x => kMeans.Predict(x)));
                var weights = groups.Select(group => (double)group.Count).ToVector();
                weights.DivInplace(weights.Sum());
                var gaussians = groups.Select(group => group.ToGaussian());
                var components = weights.Zip(gaussians, (weight, gaussian) => new GaussianMixtureModel.Component(weight, gaussian));
                return new GaussianMixtureModel(components);
            }
            catch (FittingFailureException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to fit the model.", e);
            }
        }

        /// <summary>
        /// Converts a k-means model to a Gaussian mixture model.
        /// </summary>
        /// <param name="kMeans">
        /// The source k-means model.
        /// </param>
        /// <param name="xs">
        /// The source feature vectors.
        /// </param>
        /// <returns>
        /// The Gaussian mixture model converted from the k-means model.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public static DiagonalGaussianMixtureModel ToDiagonalGmm(this KMeans kMeans, IReadOnlyList<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(kMeans, nameof(kMeans));
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            try
            {
                var groups = ArgumentHelper.GroupByClassIndex(xs, xs.Select(x => kMeans.Predict(x)));
                var weights = groups.Select(group => (double)group.Count).ToVector();
                weights.DivInplace(weights.Sum());
                var gaussians = groups.Select(group => group.ToDiagonalGaussian());
                var components = weights.Zip(gaussians, (weight, gaussian) => new DiagonalGaussianMixtureModel.Component(weight, gaussian));
                return new DiagonalGaussianMixtureModel(components);
            }
            catch (FittingFailureException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to fit the model.", e);
            }
        }
    }
}
