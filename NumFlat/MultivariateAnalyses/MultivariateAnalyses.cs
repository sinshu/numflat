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
        /// <param name="options">
        /// Specifies options for ICA.
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
        public static IndependentComponentAnalysis Ica(this IReadOnlyList<Vec<double>> xs, int componentCount, IndependentComponentAnalysisOptions? options = null, Random? random = null)
        {
            return new IndependentComponentAnalysis(xs, componentCount, options, random);
        }

        /// <summary>
        /// Performs non-negative matrix factorization (NMF).
        /// </summary>
        /// <param name="xs">
        /// The source vectors used to form matrix V, where each vector from the list is placed as a column vector in matrix V.
        /// </param>
        /// <param name="componentCount">
        /// The number of basis vectors to be estimated.
        /// </param>
        /// <param name="iterationCount">
        /// The number of iterations to perform for updating the solution.
        /// </param>
        /// <param name="random">
        /// A random number generator for the initialization.
        /// If null, <see cref="Random.Shared"/> is used.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="NonnegativeMatrixFactorization"/>.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public static NonnegativeMatrixFactorization Nmf(this IReadOnlyList<Vec<double>> xs, int componentCount, int iterationCount = 100, Random? random = null)
        {
            return new NonnegativeMatrixFactorization(xs, componentCount, iterationCount, random);
        }

        /// <summary>
        /// Performs logistic regression.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ys">
        /// The class indices for each source vector.
        /// </param>
        /// <param name="options">
        /// Specifies options for logistic regression.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="LogisticRegression"/>.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        /// <remarks>
        /// This logistic regression implementation assumes two classes.
        /// Therefore, only 0 or 1 are valid as class indices.
        /// </remarks>
        public static LogisticRegression LogisticRegression(this IReadOnlyList<Vec<double>> xs, IReadOnlyList<int> ys, LogisticRegressionOptions? options = null)
        {
            return new LogisticRegression(xs, ys, options);
        }
    }
}
