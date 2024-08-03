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
        /// Creates an instance of <see cref="LogisticRegressionOptions"/> with default parameters.
        /// </summary>
        public LogisticRegressionOptions()
        {
            regularization = 1.0E-6;
            maxIterations = 100;
            tolerance = 1.0E-4;
        }

        /// <summary>
        /// The amount of regularization.
        /// </summary>
        public double Regularization
        {
            get => regularization;

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("The amount of regularization must be a non-negative value.");
                }

                regularization = value;
            }
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
