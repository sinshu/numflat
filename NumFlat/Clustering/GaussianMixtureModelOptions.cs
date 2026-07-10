using System;

namespace NumFlat.Clustering
{
    /// <summary>
    /// Specifies options for Gaussian mixture model fitting.
    /// </summary>
    public sealed class GaussianMixtureModelOptions
    {
        private KMeansOptions? kMeansOptions;
        private double regularization;
        private int maxIterations;
        private double tolerance;

        /// <summary>
        /// Initializes a new instance of <see cref="GaussianMixtureModelOptions"/> with default parameters.
        /// </summary>
        public GaussianMixtureModelOptions()
        {
            kMeansOptions = null;
            regularization = 1.0E-6;
            maxIterations = 100;
            tolerance = 1.0E-3;
        }

        /// <summary>
        /// Gets or sets the options for k-means initialization.
        /// </summary>
        public KMeansOptions? KMeansOptions
        {
            get => kMeansOptions;

            set
            {
                kMeansOptions = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of regularization.
        /// This value will be added to the diagonal elements of the covariance matrix.
        /// </summary>
        public double Regularization
        {
            get => regularization;

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The amount of regularization must be a non-negative value.");
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
