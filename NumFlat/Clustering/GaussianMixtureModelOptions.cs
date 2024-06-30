using System;

namespace NumFlat.Clustering
{
    /// <summary>
    /// Specifies options for GMM.
    /// </summary>
    public sealed class GaussianMixtureModelOptions
    {
        private KMeansOptions? kMeansOptions;
        private double regularization;
        private int maxIterations;
        private double tolerance;

        /// <summary>
        /// Creates an instance of <see cref="GaussianMixtureModelOptions"/> with default parameters.
        /// </summary>
        public GaussianMixtureModelOptions()
        {
            kMeansOptions = null;
            regularization = 1.0E-6;
            maxIterations = 100;
            tolerance = 1.0E-3;
        }

        /// <summary>
        /// Specifies options for k-means.
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
        /// The amount of regularization.
        /// This value will be added to the diagonal elements of the covariance matrix.
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
