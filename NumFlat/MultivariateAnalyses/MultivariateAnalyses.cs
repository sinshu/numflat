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
        /// Performs principal component analysis (PCA).
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="PrincipalComponentAnalysis"/>.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public static PrincipalComponentAnalysis Pca(this IEnumerable<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return new PrincipalComponentAnalysis(xs);
        }

        /// <summary>
        /// Performs linear discriminant analysis (LDA).
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ys">
        /// The class indices for each source vector.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="LinearDiscriminantAnalysis"/>.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public static LinearDiscriminantAnalysis Lda(this IEnumerable<Vec<double>> xs, IEnumerable<int> ys)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(ys, nameof(ys));

            return new LinearDiscriminantAnalysis(xs, ys);
        }

        /// <summary>
        /// Performs independent component analysis (ICA).
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="componentCount">
        /// The number of independent components to be extracted.
        /// </param>
        /// <param name="random">
        /// A random number generator for initialization.
        /// If null, <see cref="Random.Shared"/> is used.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="IndependentComponentAnalysis"/>.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public static IndependentComponentAnalysis Ica(this IReadOnlyList<Vec<double>> xs, int componentCount, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            return new IndependentComponentAnalysis(xs, componentCount, random);
        }
    }
}
