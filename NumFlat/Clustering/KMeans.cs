﻿using System;
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
        /// <param name="options">
        /// Specifies options for k-means.
        /// </param>
        /// <param name="random">
        /// A random number generator for the k-means++ initialization.
        /// If null, <see cref="Random.Shared"/> is used.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public KMeans(IReadOnlyList<Vec<double>> xs, int clusterCount, KMeansOptions? options = null, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            if (clusterCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(clusterCount), "The number of clusters must be greater than or equal to one.");
            }

            if (clusterCount == 1)
            {
                this.centroids = [xs.Mean()];
                return;
            }

            if (options == null)
            {
                options = new KMeansOptions();
            }

            if (random == null)
            {
                random = Random.Shared;
            }

            try
            {
                var candidates = Enumerable.Range(0, options.TryCount).Select(i => GetModel(xs, clusterCount, options, random));
                var best = candidates.MinBy(candidate => candidate.Error).Model;
                this.centroids = best.centroids;
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

        private static (KMeans Model, double Error) GetModel(IReadOnlyList<Vec<double>> xs, int clusterCount, KMeansOptions options, Random random)
        {
            var tolerance = options.Tolerance * xs.Variance().Average();
            var curr = (Model: GetInitialGuess(xs, clusterCount, random), Error: double.MaxValue);
            for (var i = 0; i < options.MaxIterations; i++)
            {
                var next = curr.Model.Update(xs);
                if (GetModelDiff(curr.Model, next.Model) <= tolerance)
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
            for (var c = 0; c < model1.centroids.Length; c++)
            {
                sum += model1.centroids[c].DistanceSquared(model2.centroids[c]);
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
            for (var c = 0; c < centroids.Length; c++)
            {
                var distance = x.DistanceSquared(centroids[c]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    predicted = c;
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
        public static KMeans GetInitialGuess(IReadOnlyList<Vec<double>> xs, int clusterCount, Random random)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(random, nameof(random));

            var centroids = new Vec<double>[clusterCount];
            for (var c = 0; c < clusterCount; c++)
            {
                centroids[c] = GetNextInitialCentroid(xs, centroids.AsSpan(0, c), random);
            }

            return new KMeans(centroids);
        }

        private static Vec<double> GetNextInitialCentroid(IReadOnlyList<Vec<double>> xs, ReadOnlySpan<Vec<double>> centroids, Random random)
        {
            if (centroids.Length == 0)
            {
                return xs[random.Next(0, xs.Count)];
            }

            using var uprb = new TemporalArray<double>(xs.Count);
            var prb = uprb.Item;

            var sum = 0.0;
            var i = 0;
            foreach (var x in xs.ThrowIfEmptyOrDifferentSize(centroids[0].Count, nameof(xs)))
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
        /// An updated k-means model and its sum of squared errors computed during the update process.
        /// </returns>
        public (KMeans Model, double Error) Update(IReadOnlyList<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            var nextCentroids = new Vec<double>[centroids.Length];
            for (var c = 0; c < nextCentroids.Length; c++)
            {
                nextCentroids[c] = new Vec<double>(centroids[0].Count);
            }

            using var ucounts = new TemporalArray<int>(centroids.Length);
            var counts = ucounts.Item;
            counts.Clear();

            var error = 0.0;
            foreach (var x in xs.ThrowIfEmptyOrDifferentSize(Dimension, nameof(xs)))
            {
                var (classIndex, distance) = PredictWithSquaredDistance(centroids, x);
                nextCentroids[classIndex].AddInplace(x);
                counts[classIndex]++;
                error += distance;
            }

            for (var c = 0; c < nextCentroids.Length; c++)
            {
                if (counts[c] == 0)
                {
                    throw new FittingFailureException("A cluster has no vector assigned.");
                }

                nextCentroids[c].DivInplace(counts[c]);
            }

            return (new KMeans(nextCentroids), error);
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
