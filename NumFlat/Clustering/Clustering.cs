using System;
using System.Collections.Generic;

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
    }
}
