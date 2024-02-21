using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.Clustering
{
    /// <summary>
    /// Provides the k-means clustering algorithm.
    /// </summary>
    public sealed class KMeans : IClassifier<double>
    {
        private readonly Vec<double>[] centroids;
        private readonly int iterations;

        private KMeans(Vec<double>[] centroids, int iterations)
        {
            this.centroids = centroids;
            this.iterations = iterations;
        }

        /// <inheritdoc/>
        public int Predict(in Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            Classifier.ThrowIfInvalidSize(this, x, nameof(x));

            return PredictWithSquaredDistance(centroids, x).ClassIndex;
        }

        private static (int ClassIndex, double SquaredDistance) PredictWithSquaredDistance(ReadOnlySpan<Vec<double>> centroids, in Vec<double> x)
        {
            var minDistance = double.MaxValue;
            var minIndex = -1;
            for (var i = 0; i < centroids.Length; i++)
            {
                var distance = x.DistanceSquared(centroids[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minIndex = i;
                }
            }

            return (minIndex, minDistance);
        }

        /// <summary>
        /// Executes one iteration of the k-means algorithm to update centroids.
        /// </summary>
        /// <param name="xs">
        /// The source feature vectors.
        /// </param>
        /// <returns>
        /// The result of the update.
        /// </returns>
        public KMeans Update(IReadOnlyList<Vec<double>> xs)
        {
            var nextCentroids = new Vec<double>[centroids.Length];
            for (var i = 0; i < nextCentroids.Length; i++)
            {
                nextCentroids[i] = new Vec<double>(centroids[0].Count);
            }

            using var ucounts = MemoryPool<int>.Shared.Rent(centroids.Length);
            var counts = ucounts.Memory.Span.Slice(0, centroids.Length);
            counts.Clear();

            foreach (var x in xs)
            {
                var cls = PredictWithSquaredDistance(centroids, x).ClassIndex;
                nextCentroids[cls].AddInplace(x);
                counts[cls]++;
            }

            for (var i = 0; i < nextCentroids.Length; i++)
            {
                nextCentroids[i].DivInplace(counts[i]);
            }

            return new KMeans(nextCentroids, iterations + 1);
        }

        /// <summary>
        /// Initializes a k-means model using the k-means++ algorithm.
        /// </summary>
        /// <param name="xs">
        /// The source feature vectors.
        /// </param>
        /// <param name="clusterCount">
        /// The number of desired clusters.
        /// </param>
        /// <param name="random">
        /// A random number generator for the selection process.
        /// </param>
        /// <returns>
        /// An initial k-means model.
        /// </returns>
        public static KMeans GetInitialModel(IReadOnlyList<Vec<double>> xs, int clusterCount, Random random)
        {
            var centroids = new Vec<double>[clusterCount];
            for (var i = 0; i < clusterCount; i++)
            {
                centroids[i] = GetNextInitialCentroid(xs, centroids.AsSpan(0, i), random);
            }

            return new KMeans(centroids, 0);
        }

        private static Vec<double> GetNextInitialCentroid(IReadOnlyList<Vec<double>> xs, ReadOnlySpan<Vec<double>> centroids, Random random)
        {
            if (centroids.Length == 0)
            {
                return xs[random.Next(0, xs.Count)];
            }

            using var uprb = MemoryPool<double>.Shared.Rent(xs.Count);
            var prb = uprb.Memory.Span;

            var sum = 0.0;
            var i = 0;
            foreach (var x in xs)
            {
                var distance = PredictWithSquaredDistance(centroids, x).SquaredDistance;
                sum += distance;
                prb[i] = distance;
                i++;
            }

            var target = sum * random.NextDouble();
            var position = 0.0;
            i = 0;
            foreach (var value in prb)
            {
                position += value;
                if (position > target)
                {
                    return xs[i];
                }
                i++;
            }

            return xs[i - 1];
        }

        /// <summary>
        /// Computes the sum of squared distances between each feature vector and its nearest centroid.
        /// </summary>
        /// <param name="xs">
        /// The source feature vectors.
        /// </param>
        /// <returns>
        /// The total sum of squared distances for all vectors to their nearest centroids.
        /// </returns>
        public double GetSumOfSquaredDistances(IReadOnlyList<Vec<double>> xs)
        {
            var sum = 0.0;
            foreach (var x in xs)
            {
                sum += PredictWithSquaredDistance(centroids, x).SquaredDistance;
            }
            return sum;
        }

        /// <summary>
        /// Gets the centroids.
        /// </summary>
        public IReadOnlyList<Vec<double>> Centroids => centroids;

        /// <summary>
        /// Gets the number of iterations.
        /// </summary>
        public int Iterations => iterations;

        /// <inheritdoc/>
        public int VectorLength => centroids[0].Count;
    }
}
