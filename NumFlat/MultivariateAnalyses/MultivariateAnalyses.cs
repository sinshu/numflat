using System;
using System.Collections.Generic;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides extension methods for multivariate analyses.
    /// </summary>
    public static class MultivariateAnalyses
    {
        /// <summary>
        /// Performs principal component analysis.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="PrincipalComponentAnalysis"/>.
        /// </returns>
        public static PrincipalComponentAnalysis Pca(this IEnumerable<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return new PrincipalComponentAnalysis(xs);
        }
    }
}
