using System;
using System.Collections.Generic;
using System.Linq;
using NumFlat.Distributions;

namespace NumFlat.TimeSeries
{
    public sealed class HiddenMarkovModel
    {
        private Vec<double> initialProbabilities;
        private Mat<double> transitionMatrix;
        private IMultivariateDistribution<double>[] distributions;

        private Vec<double> logInitialProbabilities;
        private Mat<double> logTransitionMatrix;

        public HiddenMarkovModel(
            in Vec<double> initialProbabilities,
            in Mat<double> transitionMatrix,
            IReadOnlyList<IMultivariateDistribution<double>> distributions)
        {
            ThrowHelper.ThrowIfEmpty(initialProbabilities, nameof(initialProbabilities));
            ThrowHelper.ThrowIfEmpty(transitionMatrix, nameof(transitionMatrix));
            ThrowHelper.ThrowIfNull(distributions, nameof(distributions));

            var stateCount = initialProbabilities.Count;

            if (transitionMatrix.RowCount != stateCount)
            {
                throw new ArgumentException("The number of rows of the transition matrix must match the number of initial probabilities.");
            }

            if (transitionMatrix.ColCount != stateCount)
            {
                throw new ArgumentException("The number of columns of the transition matrix must match the number of initial probabilities.");
            }

            if (distributions.Count != stateCount)
            {
                throw new ArgumentException("The number of distributions must match the number of initial probabilities.");
            }

            if (distributions.Any(d => d == null))
            {
                throw new ArgumentException("All the distributions must be non-null.", nameof(distributions));
            }

            if (initialProbabilities.Any(x => x < 0))
            {
                throw new ArgumentException("All the values must be non-negative.", nameof(initialProbabilities));
            }

            if (Math.Abs(initialProbabilities.Sum() - 1) > 1.0E-14)
            {
                throw new ArgumentException("The sum of the initial probabilities must be one.", nameof(initialProbabilities));
            }

            if (transitionMatrix.SelectMany(row => row).Any(x => x < 0))
            {
                throw new ArgumentException("All the values must be non-negative.", nameof(transitionMatrix));
            }

            if (transitionMatrix.Rows.Any(row => Math.Abs(row.Sum() - 1) > 1.0E-14))
            {
                throw new ArgumentException("The sum of each row of the transition matrix must be one.", nameof(transitionMatrix));
            }

            this.initialProbabilities = initialProbabilities;
            this.transitionMatrix = transitionMatrix;
            this.distributions = distributions.ToArray();

            this.logInitialProbabilities = initialProbabilities.Map(Math.Log);
            this.logTransitionMatrix = transitionMatrix.Map(Math.Log);
        }

        public double Decode(IReadOnlyList<Vec<double>> observations, Span<int> path)
        {
            if (observations.Count == 0)
            {
                throw new ArgumentException("The observations must not be empty.", nameof(observations));
            }

            if (path.Length != observations.Count)
            {
                throw new ArgumentException("The length of path must match the number of observations.");
            }

            // Viterbi-forward algorithm.
            // https://github.com/accord-net/framework/blob/development/Sources/Accord.Statistics/Models/Markov/HiddenMarkovModel%601.cs

            var stateCount = initialProbabilities.Count;
            var length = observations.Count;

            using var us = new TemporalMatrix<int>(stateCount, length);
            var s = us.Item.GetUnsafeFastIndexer();

            using var ufwd = new TemporalMatrix<double>(stateCount, length);
            var fwd = ufwd.Item.GetUnsafeFastIndexer();

            var logInitProb = logInitialProbabilities.GetUnsafeFastIndexer();
            var logTransMat = logTransitionMatrix.GetUnsafeFastIndexer();

            int maxState;
            double maxWeight;
            double weight;

            // Base.
            for (var i = 0; i < stateCount; i++)
            {
                fwd[i, 0] = logInitProb[i] + distributions[i].LogPdf(observations[0]);
            }

            // Induction.
            for (var t = 1; t < length; t++)
            {
                var observation = observations[t];

                for (var j = 0; j < stateCount; j++)
                {
                    maxState = 0;
                    maxWeight = fwd[0, t - 1] + logTransMat[0, j];
                    for (var i = 1; i < stateCount; i++)
                    {
                        weight = fwd[i, t - 1] + logTransMat[i, j];
                        if (weight > maxWeight)
                        {
                            maxState = i;
                            maxWeight = weight;
                        }
                    }

                    fwd[j, t] = maxWeight + distributions[j].LogPdf(observation);
                    s[j, t] = maxState;
                }
            }

            // Find maximum value for time T - 1.
            maxState = 0;
            maxWeight = fwd[0, length - 1];
            for (var i = 1; i < stateCount; i++)
            {
                if (fwd[i, length - 1] > maxWeight)
                {
                    maxState = i;
                    maxWeight = fwd[i, length - 1];
                }
            }

            // Trackback.
            path[length - 1] = maxState;
            for (var t = length - 2; t >= 0; t--)
            {
                path[t] = s[path[t + 1], t + 1];
            }

            // Returns the sequence probability as an out parameter.
            return maxWeight;
        }

        public (int[] Path, double LogLikelihood) Decode(IReadOnlyList<Vec<double>> observations)
        {
            if (observations.Count == 0)
            {
                throw new ArgumentException("The observations must not be empty.", nameof(observations));
            }

            var path = new int[observations.Count];
            var logLikelihood = Decode(observations, path);
            return (path, logLikelihood);
        }

        public ref readonly Vec<double> InitialProbabilities => ref initialProbabilities;

        public ref readonly Mat<double> TransitionMatrix => ref transitionMatrix;

        public ref readonly Vec<double> LogInitialProbabilities => ref logInitialProbabilities;

        public ref readonly Mat<double> LogTransitionMatrix => ref logTransitionMatrix;
    }
}
