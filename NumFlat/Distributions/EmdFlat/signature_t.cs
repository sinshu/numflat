using System;
using System.Collections.Generic;

namespace EmdFlat
{
    /// <summary>
    /// Represents a signature used in Earth Mover's Distance computation.
    /// </summary>
    /// <typeparam name="feature_t">
    /// The type of the features in the signature.
    /// </typeparam>
    public sealed class signature_t<feature_t>
    {
        /// <summary>
        /// Number of features in the signature.
        /// </summary>
        public int n;

        /// <summary>
        /// Pointer to the features vector.
        /// </summary>
        public IReadOnlyList<feature_t> Features;

        /// <summary>
        /// Pointer to the weights of the features.
        /// </summary>
        public IReadOnlyList<double> Weights;

        /// <summary>
        /// Initializes a new instance of <see cref="signature_t{feature_t}"/> with the specified features and weights.
        /// </summary>
        /// <param name="n">
        /// Number of features in the signature.
        /// </param>
        /// <param name="Features">
        /// Pointer to the features vector.
        /// </param>
        /// <param name="Weights">
        /// Pointer to the weights of the features.
        /// </param>
        public signature_t(int n, IReadOnlyList<feature_t> Features, IReadOnlyList<double> Weights)
        {
            this.n = n;
            this.Features = Features;
            this.Weights = Weights;
        }
    }
}
