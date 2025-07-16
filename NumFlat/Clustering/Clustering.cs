using System;
using System.Collections.Generic;
using System.Linq;
using NumFlat.Distributions;

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
        /// <param name="options">
        /// Specifies options for k-means.
        /// </param>
        /// <param name="random">
        /// A random number generator for the k-means++ initialization.
        /// If null, <see cref="Random.Shared"/> is used.
        /// </param>
        /// <returns>
        /// A k-means model computed from the source vectors.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public static KMeans ToKMeans(this IReadOnlyList<Vec<double>> xs, int clusterCount, KMeansOptions? options = null, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            if (clusterCount <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(clusterCount), "The number of clusters must be greater than or equal to two.");
            }

            return new KMeans(xs, clusterCount, options, random);
        }

        /// <summary>
        /// Clusters the features using the k-medoids algorithm.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the source features.
        /// </typeparam>
        /// <param name="xs">
        /// The source features.
        /// </param>
        /// <param name="dm">
        /// The distance measure to compute distances between features.
        /// </param>
        /// <param name="clusterCount">
        /// The number of desired clusters.
        /// </param>
        /// <param name="random">
        /// A random number generator for the k-medoids++ initialization.
        /// If null, <see cref="Random.Shared"/> is used.
        /// </param>
        /// <returns>
        /// A k-medoids model computed from the source features.
        /// </returns>
        public static KMedoids<T> ToKMedoids<T>(this IReadOnlyList<T> xs, DistanceMetric<T, T> dm, int clusterCount, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(dm, nameof(dm));

            if (clusterCount <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(clusterCount), "The number of clusters must be greater than or equal to two.");
            }

            return new KMedoids<T>(xs, dm, clusterCount, random);
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
        /// <param name="options">
        /// Specifies options for GMM.
        /// </param>
        /// <param name="random">
        /// A random number generator for the k-means++ initialization.
        /// If null, <see cref="Random.Shared"/> is used.
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
        public static GaussianMixtureModel ToGmm(this IReadOnlyList<Vec<double>> xs, int clusterCount, GaussianMixtureModelOptions? options = null, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            if (clusterCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(clusterCount), "The number of clusters must be greater than or equal to one.");
            }

            return new GaussianMixtureModel(xs, clusterCount, options, random);
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
        /// <param name="options">
        /// Specifies options for GMM.
        /// </param>
        /// <param name="random">
        /// A random number generator for the k-means++ initialization.
        /// If null, <see cref="Random.Shared"/> is used.
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
        public static DiagonalGaussianMixtureModel ToDiagonalGmm(this IReadOnlyList<Vec<double>> xs, int clusterCount, GaussianMixtureModelOptions? options = null, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            if (clusterCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(clusterCount), "The number of clusters must be greater than or equal to one.");
            }

            return new DiagonalGaussianMixtureModel(xs, clusterCount, options, random);
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
                var groups = EnumerableHelper.GroupByClassIndex(xs, xs.Select(x => kMeans.Predict(x)));
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
                var groups = EnumerableHelper.GroupByClassIndex(xs, xs.Select(x => kMeans.Predict(x)));
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

        /// <summary>
        /// Applies the DBSCAN (density-based spatial clustering of applications with noise) algorithm 
        /// to the given set of points.
        /// </summary>
        /// <param name="xs">
        /// The collection of points to be clustered.
        /// </param>
        /// <param name="eps">
        /// The maximum distance between two points for them to be considered as in the same neighborhood.
        /// </param>
        /// <param name="minPoints">
        /// The minimum number of points required to form a cluster.
        /// </param>
        public static int[] DbScan(this IReadOnlyList<Vec<double>> xs, double eps, int minPoints)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (xs.Count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one point.", nameof(xs));
            }

            if (eps <= 0)
            {
                throw new ArgumentException("The eps must be a non-negative value.", nameof(eps));
            }

            if (minPoints <= 0)
            {
                throw new ArgumentException("The minimum number of points must be a non-negative value.", nameof(minPoints));
            }

            var result = new int[xs.Count];
            NumFlat.Clustering.DbScan.Fit(xs, DistanceMetric.Euclidean, eps, minPoints, result);
            return result;
        }
    }
}
