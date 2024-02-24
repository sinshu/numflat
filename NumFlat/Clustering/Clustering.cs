using NumFlat.Distributions;
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
    }
}
