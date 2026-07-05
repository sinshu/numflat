using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.TimeSeries
{
    /// <summary>
    /// Provides DTW barycenter averaging for vector-valued time series.
    /// </summary>
    public static class DtwBarycenterAveraging
    {
        /// <summary>
        /// Computes a barycenter sequence from the source sequences using DTW barycenter averaging.
        /// </summary>
        /// <param name="sourceSequences">
        /// The source sequences. Each element in every sequence must be a non-empty vector of the same dimension.
        /// </param>
        /// <param name="options">
        /// Specifies options for DTW barycenter averaging.
        /// If null, default options are used.
        /// </param>
        /// <returns>
        /// The computed barycenter sequence.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the barycenter.
        /// </exception>
        public static IReadOnlyList<Vec<double>> Fit(IReadOnlyList<IReadOnlyList<Vec<double>>> sourceSequences, DtwBarycenterAveragingOptions? options = null)
        {
            ThrowIfInvalidSequences(sourceSequences, nameof(sourceSequences));
            options ??= new DtwBarycenterAveragingOptions();

            try
            {
                var currentBarycenter = GetInitialGuess(sourceSequences).Select(vector => vector.Copy()).ToArray();
                var previousCost = double.PositiveInfinity;
                for (var i = 0; i < options.MaxIterations; i++)
                {
                    var (updatedBarycenter, averageCost) = Update(currentBarycenter, sourceSequences);
                    if (Math.Abs(previousCost - averageCost) < options.Tolerance)
                    {
                        return updatedBarycenter;
                    }
                    if (previousCost < averageCost)
                    {
                        return updatedBarycenter;
                    }

                    currentBarycenter = updatedBarycenter;
                    previousCost = averageCost;
                }

                return currentBarycenter;
            }
            catch (FittingFailureException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to fit the barycenter.", e);
            }
        }

        /// <summary>
        /// Selects the source sequence with the smallest total DTW distance to the other source sequences.
        /// </summary>
        /// <param name="sourceSequences">
        /// The source sequences.
        /// </param>
        /// <returns>
        /// The selected initial barycenter sequence.
        /// </returns>
        public static IReadOnlyList<Vec<double>> GetInitialGuess(IReadOnlyList<IReadOnlyList<Vec<double>>> sourceSequences)
        {
            ThrowIfInvalidSequences(sourceSequences, nameof(sourceSequences));

            var totalDistances = new double[sourceSequences.Count];
            for (var i = 0; i < sourceSequences.Count; i++)
            {
                for (var j = i + 1; j < sourceSequences.Count; j++)
                {
                    var distance = DynamicTimeWarping.GetDistance(sourceSequences[i], sourceSequences[j], DistanceMetric.EuclideanSquared);
                    totalDistances[i] += distance;
                    totalDistances[j] += distance;
                }
            }

            var bestIndex = -1;
            var bestDistance = double.MaxValue;
            for (var i = 0; i < sourceSequences.Count; i++)
            {
                if (totalDistances[i] < bestDistance)
                {
                    bestDistance = totalDistances[i];
                    bestIndex = i;
                }
            }

            return sourceSequences[bestIndex];
        }

        /// <summary>
        /// Executes one iteration of DTW barycenter averaging to update a barycenter sequence.
        /// </summary>
        /// <param name="currentBarycenter">
        /// The current barycenter sequence to update.
        /// </param>
        /// <param name="sourceSequences">
        /// The source sequences.
        /// </param>
        /// <returns>
        /// The updated barycenter sequence and the average DTW alignment cost computed during the update process.
        /// </returns>
        public static (Vec<double>[] Barycenter, double AverageCost) Update(IReadOnlyList<Vec<double>> currentBarycenter, IReadOnlyList<IReadOnlyList<Vec<double>>> sourceSequences)
        {
            ThrowIfInvalidSequence(currentBarycenter, nameof(currentBarycenter));
            var dimension = currentBarycenter[0].Count;
            ThrowIfInvalidSequences(sourceSequences, nameof(sourceSequences), dimension);

            var sums = new Vec<double>[currentBarycenter.Count];
            var counts = new int[currentBarycenter.Count];
            for (var i = 0; i < sums.Length; i++)
            {
                sums[i] = new Vec<double>(dimension);
            }

            var cost = 0.0;
            foreach (var sourceSequence in sourceSequences)
            {
                var (distance, alignment) = DynamicTimeWarping.GetDistanceAndAlignment(currentBarycenter, sourceSequence, DistanceMetric.EuclideanSquared);
                cost += distance;
                foreach (var pair in alignment)
                {
                    sums[pair.First].AddInplace(sourceSequence[pair.Second]);
                    counts[pair.First]++;
                }
            }

            for (var i = 0; i < sums.Length; i++)
            {
                if (counts[i] == 0)
                {
                    sums[i] = currentBarycenter[i].Copy();
                }
                else
                {
                    sums[i].DivInplace(counts[i]);
                }
            }

            return (sums, cost / sourceSequences.Count);
        }

        private static void ThrowIfInvalidSequences(IReadOnlyList<IReadOnlyList<Vec<double>>> sourceSequences, string paramName, int? dimension = null)
        {
            ThrowHelper.ThrowIfNull(sourceSequences, paramName);
            if (sourceSequences.Count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one sequence.", paramName);
            }

            foreach (var sequence in sourceSequences)
            {
                ThrowIfInvalidSequence(sequence, paramName, dimension);
                dimension ??= sequence[0].Count;
            }
        }

        private static void ThrowIfInvalidSequence(IReadOnlyList<Vec<double>> sequence, string paramName, int? dimension = null)
        {
            ThrowHelper.ThrowIfNull(sequence, paramName);
            ThrowHelper.ThrowIfEmpty(sequence, paramName);

            var expectedDimension = dimension;
            foreach (var vector in sequence)
            {
                ThrowHelper.ThrowIfEmpty(vector, paramName);
                expectedDimension ??= vector.Count;
                if (vector.Count != expectedDimension.Value)
                {
                    throw new ArgumentException("All vectors must have the same length.", paramName);
                }
            }
        }
    }
}
