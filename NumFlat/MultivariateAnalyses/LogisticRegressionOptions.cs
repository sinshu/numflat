using System;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Specifies options for logistic regression.
    /// </summary>
    public sealed class LogisticRegressionOptions
    {
        private double regularization;
        private int maxIterations;
        private double tolerance;

        /// <summary>
        /// Initializes a new instance of <see cref="LogisticRegressionOptions"/> with default parameters.
        /// </summary>
        public LogisticRegressionOptions()
        {
            regularization = 1.0E-6;
            maxIterations = 100;
            tolerance = 1.0E-4;
        }

        /// <summary>
        /// Gets or sets the amount of regularization.
        /// </summary>
        public double Regularization
        {
            get => regularization;

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"The regularization amount must be greater than or equal to zero, but was '{value}'.");
                }

                regularization = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of iterations to perform.
        /// </summary>
        public int MaxIterations
        {
            get => maxIterations;

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The maximum number of iterations must be greater than zero.");
                }

                maxIterations = value;
            }
        }

        /// <summary>
        /// Gets or sets the convergence tolerance.
        /// </summary>
        public double Tolerance
        {
            get => tolerance;

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The tolerance value must be greater than zero.");
                }

                tolerance = value;
            }
        }
    }
}
