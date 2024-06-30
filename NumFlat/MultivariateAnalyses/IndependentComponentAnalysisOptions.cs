using System;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Specifies options for ICA.
    /// </summary>
    public sealed class IndependentComponentAnalysisOptions
    {
        private int maxIterations;
        private double tolerance;

        /// <summary>
        /// Creates an instance of <see cref="IndependentComponentAnalysisOptions"/> with default parameters.
        /// </summary>
        public IndependentComponentAnalysisOptions()
        {
            maxIterations = 200;
            tolerance = 1.0E-4;
        }

        /// <summary>
        /// The maximum number of iterations to perform.
        /// </summary>
        public int MaxIterations
        {
            get => maxIterations;

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("The max number of iterations must be greater than zero.");
                }

                maxIterations = value;
            }
        }

        /// <summary>
        /// Specify the amount of change to be considered converging.
        /// </summary>
        public double Tolerance
        {
            get => tolerance;

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("The tolerance value must be greater than zero.");
                }

                tolerance = value;
            }
        }
    }
}
