using System;
using System.Collections.Generic;

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
