using System;

namespace NumFlat.Clustering
{
    /// <summary>
    /// Specifies options for k-means fitting.
    /// </summary>
    public sealed class KMeansOptions
    {
        private int tryCount;
        private int maxIterations;
        private double tolerance;

        /// <summary>
        /// Initializes a new instance of <see cref="KMeansOptions"/> with default parameters.
        /// </summary>
        public KMeansOptions()
        {
            tryCount = 3;
            maxIterations = 300;
            tolerance = 1.0E-4;
        }

        /// <summary>
        /// Gets or sets the number of k-means attempts. The model with the lowest error is selected.
        /// </summary>
        public int TryCount
        {
            get => tryCount;

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The number of attempts must be greater than zero.");
                }

                tryCount = value;
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
