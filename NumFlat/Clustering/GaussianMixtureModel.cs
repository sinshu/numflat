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

        /// <inheritdoc/>
        public int Dimension => throw new NotImplementedException();

        /// <inheritdoc/>
        public int ClassCount => throw new NotImplementedException();



        /// <summary>
        /// Represents a Gaussian component in a Gaussian mixture model.
        /// </summary>
        public class Component
        {
            private double weight;
            private Gaussian gaussian;

            /// <summary>
            /// Initialize a new Gaussian component.
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
                this.gaussian = gaussian;
            }

            /// <summary>
            /// Gets the weight of the component.
            /// </summary>
            public double Weight => weight;

            /// <summary>
            /// Gets the distribution of the component.
            /// </summary>
            public Gaussian Gaussian => gaussian;
        }
    }
}
