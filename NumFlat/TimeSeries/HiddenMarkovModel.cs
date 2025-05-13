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
                throw new ArgumentException("All the distributions must be non-null.");
            }

            if (Math.Abs(initialProbabilities.Sum() - 1) > 1.0E-14)
            {
                throw new ArgumentException("The sum of the initial probabilities must be one.");
            }

            if (transitionMatrix.Rows.Any(row => Math.Abs(row.Sum() - 1) > 1.0E-14))
            {
                throw new ArgumentException("The sum of each row of the transition matrix must be one.");
            }

            this.initialProbabilities = initialProbabilities;
            this.transitionMatrix = transitionMatrix;
            this.distributions = distributions.ToArray();

            this.logInitialProbabilities = initialProbabilities.Map(Math.Log);
            this.logTransitionMatrix = transitionMatrix.Map(Math.Log);
        }

        public int[] Decode(IReadOnlyList<Vec<double>> observations)
        {

        }

        public ref readonly Vec<double> InitialProbabilities => ref initialProbabilities;

        public ref readonly Mat<double> TransitionMatrix => ref transitionMatrix;

        public ref readonly Vec<double> LogInitialProbabilities => ref logInitialProbabilities;

        public ref readonly Mat<double> LogTransitionMatrix => ref logTransitionMatrix;
    }
}
