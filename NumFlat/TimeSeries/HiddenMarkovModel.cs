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
