using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using NumFlat.Distributions;

namespace NumFlat.Clustering
{
    /// <summary>
    /// Provides the Gaussian mixture model.
    /// </summary>
    public class GaussianMixtureModel : IProbabilisticClassifier<double>, IMultivariateDistribution<double>
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
        /// <param name="options">
        /// Specifies options for GMM.
        /// </param>
        /// <param name="random">
        /// A random number generator for the k-means++ initialization.
        /// If null, <see cref="Random.Shared"/> is used.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        /// <remarks>
        /// An initial GMM is constructed with the k-means algorithm.
        /// </remarks>
        public GaussianMixtureModel(IReadOnlyList<Vec<double>> xs, int clusterCount, GaussianMixtureModelOptions? options = null, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            if (clusterCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(clusterCount), "The number of clusters must be greater than or equal to one.");
            }

            if (options == null)
            {
                options = new GaussianMixtureModelOptions();
            }

            if (clusterCount == 1)
            {
                this.components = [new Component(1.0, xs.ToGaussian(options.Regularization))];
                return;
            }

            try
            {
                var tolerance = options.Tolerance * xs.Count;
                var curr = (Model: xs.ToKMeans(clusterCount, options.KMeansOptions, random).ToGmm(xs), LogLikelihood: double.MinValue);
                for (var i = 0; i < options.MaxIterations; i++)
                {
                    var next = curr.Model.Update(xs, options.Regularization);
                    var change = Math.Abs(next.LogLikelihood - curr.LogLikelihood);
                    if (change <= tolerance)
                    {
                        this.components = next.Model.components;
                        return;
                    }
                    curr = next;
                }
                this.components = curr.Model.components;
            }
            catch (FittingFailureException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to fit the model.", e);
            }
        }

        /// <inheritdoc/>
        public int Predict(in Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            Classifier.ThrowIfInvalidSize(this, x, nameof(x));

            var maxScore = double.MinValue;
            var predicted = -1;
            for (var c = 0; c < ClassCount; c++)
            {
                var component = components[c];
                var score = component.LogWeight + component.Gaussian.LogPdf(x);
                if (score > maxScore)
                {
                    maxScore = score;
                    predicted = c;
                }
            }

            return predicted;
        }

        private void PredictLogProbability(in Vec<double> x, in Vec<double> destination)
        {
            var c = 0;
            foreach (ref var value in destination)
            {
                value = components[c].LogWeight + components[c].Gaussian.LogPdf(x);
                c++;
            }
        }

        private double PredictProbabilityWithLogSum(in Vec<double> x, in Vec<double> destination)
        {
            PredictLogProbability(x, destination);

            var logSum = Special.LogSum(destination);
            foreach (ref var value in destination)
            {
                value = Math.Exp(value - logSum);
            }
            return logSum;
        }

        /// <inheritdoc/>
        public void PredictProbability(in Vec<double> x, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ProbabilisticClassifier.ThrowIfInvalidSize(this, x, destination, nameof(x), nameof(destination));

            PredictProbabilityWithLogSum(x, destination);
        }

        /// <summary>
        /// Executes one iteration of the EM algorithm to update the GMM.
        /// </summary>
        /// <param name="xs">
        /// The source feature vectors.
        /// </param>
        /// <param name="regularization">
        /// The amount of regularization.
        /// This value will be added to the diagonal elements of the covariance matrix.
        /// </param>
        /// <returns>
        /// An updated GMM and its log-likelihood computed during the E-step.
        /// </returns>
        public (GaussianMixtureModel Model, double LogLikelihood) Update(IReadOnlyList<Vec<double>> xs, double regularization = 1.0E-6)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            if (regularization < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(regularization), "The amount of regularization must be a non-negative value.");
            }

            using var utmp = new TemporalMatrix<double>(ClassCount, xs.Count);
            ref readonly var tmp = ref utmp.Item;

            var likelihood = 0.0;
            foreach (var (x, scores) in xs.ThrowIfEmptyOrDifferentSize(Dimension, nameof(xs)).Zip(tmp.Cols))
            {
                likelihood += PredictProbabilityWithLogSum(x, scores);
            }

            try
            {
                var nextWeights = tmp.Rows.Select(row => row.Sum() / xs.Count);
                var nextGaussians = tmp.Rows.Select(row => xs.ToGaussian(row, regularization));
                var nextComponents = nextWeights.Zip(nextGaussians, (weight, gaussian) => new Component(weight, gaussian)).ToArray();
                return (new GaussianMixtureModel(nextComponents), likelihood);
            }
            catch (FittingFailureException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to fit the model.", e);
            }
        }

        /// <inheritdoc/>
        public double LogPdf(in Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            MultivariateDistribution.ThrowIfInvalidSize(this, x, nameof(x));

            using var utmp = new TemporalVector<double>(ClassCount);
            ref readonly var tmp = ref utmp.Item;

            PredictLogProbability(x, tmp);
            return Special.LogSum(tmp);
        }

        /// <inheritdoc/>
        public double Pdf(in Vec<double> x)
        {
            return Math.Exp(LogPdf(x));
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
            private readonly double weight;
            private readonly double logWeight;
            private readonly Gaussian gaussian;

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
