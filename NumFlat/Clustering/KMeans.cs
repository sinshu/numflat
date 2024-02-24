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
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public KMeans(IReadOnlyList<Vec<double>> xs, int clusterCount, int tryCount = 3, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            if (clusterCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(clusterCount), "The number of clusters must be greater than or equal to one.");
            }

            if (tryCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tryCount), "The number of attempts must be greater than or equal to one.");
            }

            if (clusterCount == 1)
            {
                this.centroids = new Vec<double>[] { xs.Mean() };
                return;
            }

            if (random == null)
            {
                random = new Random();
            }

            try
            {
                var tolerance = 1.0E-4 * xs.Variance().Average(); // 1.0E-4 is the default value of sklearn.cluster.KMeans.
                var models = Enumerable.Range(0, tryCount).Select(i => GetModel(xs, clusterCount, random, tolerance));
                var best = models.MinBy(model => GetError(xs, model.centroids));
                this.centroids = best!.centroids;
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

        private static KMeans GetModel(IReadOnlyList<Vec<double>> xs, int clusterCount, Random random, double tolerance)
        {
            var curr = GetInitialModel(xs, clusterCount, random);
            for (var i = 0; i < 300; i++) // 300 is the default value of sklearn.cluster.KMeans.
            {
                var next = curr.Update(xs);
                if (GetModelDiff(curr, next) <= tolerance)
                {
                    return next;
                }
                curr = next;
            }
            return curr;
        }

        private static double GetModelDiff(KMeans model1, KMeans model2)
        {
            var sum = 0.0;
            for (var i = 0; i < model1.centroids.Length; i++)
            {
                sum += model1.centroids[i].DistanceSquared(model2.centroids[i]);
            }
            return sum;
        }

        /// <inheritdoc/>
        public int Predict(in Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            Classifier.ThrowIfInvalidSize(this, x, nameof(x));

            return PredictWithSquaredDistance(centroids, x).ClassIndex;
        }

        /// <summary>
        /// Predicts the class of a feature vector and its distance to the corresponding centroid.
        /// </summary>
        /// <param name="x">
        /// The feature vector to be classified.
        /// </param>
        /// <returns>
        /// The index of the predicted class and the distance from the vector to the corresponding centroid.
        /// </returns>
        public (int ClassIndex, double Distance) PredictWithDistance(in Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            Classifier.ThrowIfInvalidSize(this, x, nameof(x));

            var (classIndex, squaredDistance) = PredictWithSquaredDistance(centroids, x);
            return (classIndex, Math.Sqrt(squaredDistance));
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
            ThrowHelper.ThrowIfNull(random, nameof(random));

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
        /// An updated k-means model.
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

        private static double GetError(IReadOnlyList<Vec<double>> xs, ReadOnlySpan<Vec<double>> centroids)
        {
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
        public int Dimension => centroids[0].Count;

        /// <inheritdoc/>
        public int ClassCount => centroids.Length;
    }
}
