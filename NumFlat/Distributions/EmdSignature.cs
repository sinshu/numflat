using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.Distributions
{
    /// <summary>
    /// Represents a signature used in Earth Mover's Distance computation.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the features in the signature.
    /// </typeparam>
    public sealed class EmdSignature<T>
    {
        private IReadOnlyList<T> features;
        private IReadOnlyList<double> weights;

        /// <summary>
        /// Initializes a new instance of <see cref="EmdSignature{T}"/> with the specified features and weights.
        /// </summary>
        /// <param name="features">
        /// A list of features representing the signature.
        /// </param>
        /// <param name="weights">
        /// A list of weights corresponding to each feature.
        /// </param>
        public EmdSignature(IReadOnlyList<T> features, IReadOnlyList<double> weights)
        {
            ThrowHelper.ThrowIfNull(features, nameof(features));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));

            if (features.Count != weights.Count)
            {
                throw new ArgumentException("The number of features and weights must match.");
            }

            if (features.Count == 0)
            {
                throw new ArgumentException("At least one feature is required.");
            }

            if (weights.Any(w => w < 0))
            {
                throw new ArgumentException("Negative weight values are not allowed.");
            }

            this.features = features;
            this.weights = weights;
        }

        /// <summary>
        /// Gets the list of features in the signature.
        /// </summary>
        public IReadOnlyList<T> Features => features;

        /// <summary>
        /// Gets the list of weights corresponding to the features.
        /// </summary>
        public IReadOnlyList<double> Weights => weights;
    }
}
