using System;

namespace NumFlat.TimeSeries
{
    /// <summary>
    /// Specifies options for DTW barycenter averaging.
    /// </summary>
    public sealed class DtwBarycenterAveragingOptions
    {
        private int maxIterations;
        private double tolerance;

        /// <summary>
        /// Initializes a new instance of <see cref="DtwBarycenterAveragingOptions"/> with default parameters.
        /// </summary>
        public DtwBarycenterAveragingOptions()
        {
            maxIterations = 30;
            tolerance = 1.0E-5;
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
