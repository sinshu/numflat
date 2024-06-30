using System;

namespace NumFlat.Clustering
{
    /// <summary>
    /// Specifies options for k-means.
    /// </summary>
    public sealed class KMeansOptions
    {
        private int tryCount;
        private int maxIterations;
        private double tolerance;

        /// <summary>
        /// Creates an instance of <see cref="KMeansOptions"/> with default parameters.
        /// </summary>
        public KMeansOptions()
        {
            tryCount = 3;
            maxIterations = 300;
            tolerance = 1.0E-4;
        }

        /// <summary>
        /// Runs the k-means algorithm a specified number of times and selects the model with the lowest error.
        /// </summary>
        public int TryCount
        {
            get => tryCount;

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("The number of attempts must be greater than zero.");
                }

                tryCount = value;
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
                    throw new ArgumentOutOfRangeException("The maximum number of iterations must be greater than zero.");
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
