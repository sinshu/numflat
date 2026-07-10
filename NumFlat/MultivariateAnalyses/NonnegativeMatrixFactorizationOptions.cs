using System;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Specifies options for nonnegative matrix factorization fitting.
    /// </summary>
    public sealed class NonnegativeMatrixFactorizationOptions
    {
        private int maxIterations;
        private double tolerance;

        /// <summary>
        /// Initializes a new instance of <see cref="NonnegativeMatrixFactorizationOptions"/> with default parameters.
        /// </summary>
        public NonnegativeMatrixFactorizationOptions()
        {
            maxIterations = 200;
            tolerance = 1.0E-4;
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
