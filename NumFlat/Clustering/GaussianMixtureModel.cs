using System;
using System.Collections.Generic;
using System.Linq;
using NumFlat.Distributions;

namespace NumFlat.Clustering
{
    /// <summary>
    /// Provides the Gaussian mixture model.
    /// </summary>
    public class GaussianMixtureModel : IProbabilisticClassifier<double>
    {
        private readonly Component[] components;

        private GaussianMixtureModel(Component[] components)
        {
            this.components = components;
        }

        /// <summary>
        /// Initializes a new Gaussian mixture model by the given Gaussian components.
        /// </summary>
        /// <param name="components">
        /// The source Gaussian components.
        /// </param>
        public GaussianMixtureModel(IEnumerable<Component> components)
        {
            this.components = components.ToArray();
        }

        /// <summary>
        /// Clusters the feature vectors as a Gaussian mixture model (GMM) using the expectation-maximization (EM) algorithm.
        /// </summary>
        /// <param name="xs">
        /// The source feature vectors.
        /// </param>
        /// <param name="clusterCount">
        /// The number of desired clusters.
        /// </param>
        /// <param name="regularization">
        /// The amount of regularization.
        /// This value will be added to the diagonal elements of the covariance matrix.
        /// </param>
        /// <param name="kMeansTryCount">
        /// Runs the k-means algorithm a specified number of times and selects the initial model with the lowest error.
        /// </param>
        /// <param name="random">
        /// A random number generator for the k-means++ initialization.
        /// If null, a <see cref="Random"/> object instantiated with the default constructor will be used.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        /// <remarks>
        /// An initial GMM is constructed with the k-means++ algorithm.
        /// </remarks>
        public GaussianMixtureModel(IReadOnlyList<Vec<double>> xs, int clusterCount, double regularization = 0.0, int kMeansTryCount = 3, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            if (clusterCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(clusterCount), "The number of clusters must be greater than or equal to one.");
            }

            if (regularization < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(regularization), "The amount of regularization must be a non-negative value.");
            }

            if (kMeansTryCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(kMeansTryCount), "The number of attempts must be greater than or equal to one.");
            }

            if (clusterCount == 1)
            {
                this.components = new Component[] { new Component(1.0, xs.ToGaussian(regularization)) };
                return;
            }

            if (random == null)
            {
                random = new Random();
            }

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int Predict(in Vec<double> x)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void PredictProbability(in Vec<double> x, in Vec<double> destination)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the Gaussian components.
        /// </summary>
        public IReadOnlyList<Component> Components => components;

        /// <inheritdoc/>
        public int Dimension => components[0].Gaussian.Dimension;

        /// <inheritdoc/>
        public int ClassCount => components.Length;



        /// <summary>
        /// Represents a Gaussian component in a Gaussian mixture model.
        /// </summary>
        public class Component
        {
            private double weight;
            private double logWeight;
            private Gaussian gaussian;

            /// <summary>
            /// Initializes a new Gaussian component.
            /// </summary>
            /// <param name="weight">
            /// The weight of the component.
            /// </param>
            /// <param name="gaussian">
            /// The distribution of the component.
            /// </param>
            public Component(double weight, Gaussian gaussian)
            {
                ThrowHelper.ThrowIfNull(gaussian, nameof(gaussian));

                if (weight < 0)
                {
                    throw new ArgumentException("Negative weight values are not allowed.");
                }

                this.weight = weight;
                this.logWeight = Math.Log(weight);
                this.gaussian = gaussian;
            }

            /// <summary>
            /// Gets the weight of the component.
            /// </summary>
            public double Weight => weight;

            /// <summary>
            /// Gets the log weight of the component.
            /// </summary>
            public double LogWeight => logWeight;

            /// <summary>
            /// Gets the distribution of the component.
            /// </summary>
            public Gaussian Gaussian => gaussian;
        }
    }
}
