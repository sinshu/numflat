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

        private KMeans(Vec<double>[] centroids)
        {
            this.centroids = centroids;
        }

        /// <summary>
        /// Cluster the feature vectors using the k-means algorithm.
        /// </summary>
        /// <param name="xs">
        /// The source feature vectors.
        /// </param>
        /// <param name="clusterCount">
        /// The number of desired clusters.
        /// </param>
        /// <param name="tryCount">
        /// Run the k-means algorithm the specified number of times and select the model with the lowest error.
        /// </param>
        /// <param name="random">
        /// A random number generator for the selection process.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public KMeans(IReadOnlyList<Vec<double>> xs, int clusterCount, int tryCount = 3, Random? random = null)
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

            if (random == null)
            {
                random = new Random();
            }

            var best = Enumerable.Range(0, tryCount).Select(i => GetModel(xs, clusterCount, random)).MinBy(model => model.Error);

            this.centroids = best.Model.centroids;
        }

        private static (KMeans Model, double Error) GetModel(IReadOnlyList<Vec<double>> xs, int clusterCount, Random random)
        {
            var model = GetInitialModel(xs, clusterCount, random);
            var error = model.GetSumOfSquaredDistances(xs);
            var threshold = error * 1.0E-12;
            while (true)
            {
                var nextModel = model.Update(xs);
                var nextError = nextModel.GetSumOfSquaredDistances(xs);
                if (Math.Abs(nextError - error) <= threshold)
                {
                    return (nextModel, nextError);
                }
                model = nextModel;
                error = nextError;
            }
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
            var predicted = -1;
            for (var i = 0; i < centroids.Length; i++)
            {
                var distance = x.DistanceSquared(centroids[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    predicted = i;
                }
            }

            return (predicted, minDistance);
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
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            var centroids = new Vec<double>[clusterCount];
            for (var i = 0; i < clusterCount; i++)
            {
                centroids[i] = GetNextInitialCentroid(xs, centroids.AsSpan(0, i), random);
            }

            return new KMeans(centroids);
        }

        private static Vec<double> GetNextInitialCentroid(IReadOnlyList<Vec<double>> xs, ReadOnlySpan<Vec<double>> centroids, Random random)
        {
            if (centroids.Length == 0)
            {
                return xs[random.Next(0, xs.Count)];
            }

            using var uprb = MemoryPool<double>.Shared.Rent(xs.Count);
            var prb = uprb.Memory.Span.Slice(0, xs.Count);

            var sum = 0.0;
            var i = 0;
            foreach (var x in xs.ThrowIfEmptyOrDifferentSize(nameof(xs)))
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
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            var nextCentroids = new Vec<double>[centroids.Length];
            for (var i = 0; i < nextCentroids.Length; i++)
            {
                nextCentroids[i] = new Vec<double>(centroids[0].Count);
            }

            using var ucounts = MemoryPool<int>.Shared.Rent(centroids.Length);
            var counts = ucounts.Memory.Span.Slice(0, centroids.Length);
            counts.Clear();

            foreach (var x in xs.ThrowIfEmptyOrDifferentSize(nameof(xs)))
            {
                var cls = PredictWithSquaredDistance(centroids, x).ClassIndex;
                nextCentroids[cls].AddInplace(x);
                counts[cls]++;
            }

            for (var i = 0; i < nextCentroids.Length; i++)
            {
                if (counts[i] == 0)
                {
                    throw new FittingFailureException("A cluster has no vector assigned.");
                }

                nextCentroids[i].DivInplace(counts[i]);
            }

            return new KMeans(nextCentroids);
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
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            var sum = 0.0;
            foreach (var x in xs.ThrowIfEmptyOrDifferentSize(nameof(xs)))
            {
                sum += PredictWithSquaredDistance(centroids, x).SquaredDistance;
            }
            return sum;
        }

        /// <summary>
        /// Gets the centroids.
        /// </summary>
        public IReadOnlyList<Vec<double>> Centroids => centroids;

        /// <inheritdoc/>
        public int VectorLength => centroids[0].Count;
    }
}
