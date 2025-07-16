using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.Clustering
{
    /// <summary>
    /// Provides the k-medoids clustering algorithm.
    /// </summary>
    public sealed class KMedoids<T>
    {
        private DistanceMetric<T, T> dm;

        private int[] medoidIndices;
        private T[] medoids;

        /// <summary>
        /// Clusters the features using the k-medoids algorithm.
        /// </summary>
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
        public KMedoids(IReadOnlyList<T> xs, DistanceMetric<T, T> dm, int clusterCount, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(dm, nameof(dm));

            if (xs.Count == 0)
            {
                throw new ArgumentException("The source features must not be empty.", nameof(xs));
            }

            if (clusterCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(clusterCount), "The number of clusters must be greater than or equal to one.");
            }

            if (xs.Count < clusterCount)
            {
                throw new ArgumentException("The number of source features must be larger than the number of clusters.", nameof(xs));
            }

            this.dm = dm;

            if (clusterCount == 1)
            {
                var minDistance = double.MaxValue;
                var index = -1;
                for (var i = 0; i < xs.Count; i++)
                {
                    var target = xs[i];
                    var sum = 0.0;
                    for (var j = 0; j < xs.Count; j++)
                    {
                        var x = xs[j];
                        sum += dm(target, x);
                    }
                    if (sum < minDistance)
                    {
                        minDistance = sum;
                        index = i;
                    }
                }

                medoidIndices = [index];
                medoids = [xs[index]];
                return;
            }

            if (random == null)
            {
                random = Random.Shared;
            }

            medoidIndices = new int[clusterCount];
            for (var m = 0; m < clusterCount; m++)
            {
                medoidIndices[m] = GetNextInitialMedoid(xs, dm, medoidIndices.AsSpan(0, m), random);
            }

            Func<int, int, double> func = (i, j) => dm(xs[i], xs[j]);
            medoidIndices = FastPamLike(medoidIndices, xs.Count, clusterCount, func, random);

            medoids = new T[clusterCount];
            for (var m = 0; m < medoids.Length; m++)
            {
                medoids[m] = xs[medoidIndices[m]];
            }
        }

        /// <summary>
        /// Predicts the class of a feature and its distance to the corresponding medoid.
        /// </summary>
        /// <param name="x">
        /// The feature to be classified.
        /// </param>
        /// <returns>
        /// The index of the predicted class and the distance from the feature to the corresponding medoid.
        /// </returns>
        public (int ClassIndex, double Distance) PredictWithDistance(T x)
        {
            var nearestDistance = double.MaxValue;
            var predicted = -1;
            for (var m = 0; m < medoids.Length; m++)
            {
                var d = dm(x, medoids[m]);
                if (d < nearestDistance)
                {
                    nearestDistance = d;
                    predicted = m;
                }
            }
            return (predicted, nearestDistance);
        }

        /// <summary>
        /// Predicts the class of a feature.
        /// </summary>
        /// <param name="x">
        /// The feature to be classified.
        /// </param>
        /// <returns>
        /// The index of the predicted class.
        /// </returns>
        public int Predict(T x)
        {
            return PredictWithDistance(x).ClassIndex;
        }

        private static double GetNearestDistance(IReadOnlyList<T> xs, DistanceMetric<T, T> dm, ReadOnlySpan<int> medoids, T x)
        {
            var nearestDistance = double.MaxValue;
            for (var m = 0; m < medoids.Length; m++)
            {
                var d = dm(x, xs[medoids[m]]);
                if (d < nearestDistance)
                {
                    nearestDistance = d;
                }
            }
            return nearestDistance;
        }

        private static int GetNextInitialMedoid(IReadOnlyList<T> xs, DistanceMetric<T, T> dm, ReadOnlySpan<int> medoids, Random random)
        {
            if (medoids.Length == 0)
            {
                return random.Next(0, xs.Count);
            }

            using var uprb = new TemporalArray<double>(xs.Count);
            var prb = uprb.Item;

            var sum = 0.0;
            var i = 0;
            foreach (var x in xs)
            {
                var d = GetNearestDistance(xs, dm, medoids, x);
                sum += d;
                prb[i] = d;
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
                    return i;
                }
                i++;
            }

            return i - 1;
        }

        private static int[] FastPamLike(int[] medoids, int n, int k, Func<int, int, double> dm, Random random)
        {
            var nearest = new int[n];
            var dNearest = new double[n];
            var dSecond = new double[n];

            var removalLoss = new double[k];
            var dTd = new double[k];

            // repeat
            while (true)
            {
                var improved = false;

                // foreach xo do compute nearest(o), dnearest(o), dsecond(o);
                for (var o = 0; o < n; o++)
                {
                    var best = double.MaxValue;
                    var second = double.MaxValue;
                    var bestIndex = -1;

                    for (var i = 0; i < k; i++)
                    {
                        var m = medoids[i];
                        var d = (o == m) ? 0 : dm(o, m);
                        if (d < best)
                        {
                            second = best; best = d; bestIndex = i;
                        }
                        else if (d < second)
                        {
                            second = d;
                        }
                    }
                    nearest[o] = bestIndex;
                    dNearest[o] = best;
                    dSecond[o] = second;
                }

                // dTD-m1, ..., dTD-mk <- compute initial removal loss;
                Array.Clear(removalLoss);
                for (var o = 0; o < n; o++)
                {
                    removalLoss[nearest[o]] += dSecond[o] - dNearest[o];
                }

                // iterate over all non-medoids
                // foreach xc != { m1, ..., mk } do
                foreach (var xc in Enumerable.Range(0, n).Except(medoids))
                {
                    // use removal loss
                    // dTD <- (dTD-m1, ..., dTD-mk);
                    Array.Copy(removalLoss, dTd, k);

                    // shared accumulator
                    // dTD+xc <- 0;
                    var shared = 0.0;

                    // foreach xo do
                    for (var o = 0; o < n; o++)
                    {
                        // distance to new medoid
                        // doj <- d(xo, xc);
                        var dOj = dm(o, xc);

                        // case (i)
                        // if doj < dnearest(o) then
                        if (dOj < dNearest[o])
                        {
                            // dTD+xc <- dTD+xc + doj - dnearest(o);
                            shared += dOj - dNearest[o];

                            // dTD+nearest(o) <- dTD+nearest(o) + dnearest(o) − dsecond(o);
                            dTd[nearest[o]] += dNearest[o] - dSecond[o];
                        }
                        // case (ii) and (iii)
                        // else if doj < dsecond(o) then
                        else if (dOj < dSecond[o])
                        {
                            // dTD+nearest(o) <- dTD+nearest(o) + doj - dsecond(o);
                            dTd[nearest[o]] += dOj - dSecond[o];
                        }
                    }

                    // choose best medoid
                    // i <- argmin dTDi;
                    var best = ArgMin(dTd);

                    // add accumulator
                    // dTDi <- dTDi + dTD+xc;
                    var gain = dTd[best] + shared;

                    // eager swapping
                    // if dTDi < 0 then
                    if (gain < 0)
                    {
                        // swap roles of medoid m* and non-medoid xo;
                        medoids[best] = xc;

                        // Since medoid has been updated, this foreach should be redone.
                        improved = true;
                        break;
                    }
                }

                if (!improved)
                {
                    break;
                }
            }

            return medoids;
        }

        private static int ArgMin(double[] a)
        {
            var value = double.MaxValue;
            var index = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] < value)
                {
                    value = a[i];
                    index = i;
                }
            }
            return index;
        }

        /// <summary>
        /// Gets the distance measure to compute distances between features.
        /// </summary>
        public DistanceMetric<T, T> DistanceMetric => dm;

        /// <summary>
        /// Gets the indices of the medoids in the source features.
        /// </summary>
        public IReadOnlyList<int> MedoidIndices => medoidIndices;

        /// <summary>
        /// Gets the medoids.
        /// </summary>
        public IReadOnlyList<T> Medoids => medoids;
    }
}
