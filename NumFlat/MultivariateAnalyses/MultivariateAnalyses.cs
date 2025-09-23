using System;
using System.Collections.Generic;
using System.Numerics;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides extension methods for multivariate analyses.
    /// </summary>
    public static class MultivariateAnalyses
    {
        /// <summary>
        /// Performs linear regression.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ys">
        /// The target values to be estimated.
        /// </param>
        /// <param name="regularization">
        /// The amount of regularization.
        /// The regularization method is L2 regularization excluding the bias term.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="NumFlat.MultivariateAnalyses.LinearRegression"/>.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public static LinearRegression LinearRegression(this IReadOnlyList<Vec<double>> xs, IReadOnlyList<double> ys, double regularization = 0.0)
        {
            return new LinearRegression(xs, ys, regularization);
        }

        /// <summary>
        /// Performs complex linear regression.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ys">
        /// The target values to be estimated.
        /// </param>
        /// <param name="regularization">
        /// The amount of regularization.
        /// The regularization method is L2 regularization excluding the bias term.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="NumFlat.MultivariateAnalyses.ComplexLinearRegression"/>.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public static ComplexLinearRegression LinearRegression(this IReadOnlyList<Vec<Complex>> xs, IReadOnlyList<Complex> ys, double regularization = 0.0)
        {
            return new ComplexLinearRegression(xs, ys, regularization);
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
        /// A new instance of <see cref="NumFlat.MultivariateAnalyses.LogisticRegression"/>.
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
        /// Applies classical MDS to the given distance matrix.
        /// </summary>
        /// <param name="distanceMatrix">
        /// The distance matrix.
        /// </param>
        /// <returns>
        /// The embedded coordinate matrix.
        /// If the distance matrix has dimensions <c>n * n</c>, this matrix has dimensions <c>n * n</c>,
        /// where the i-th row represents the coordinates of the i-th data point.
        /// </returns>
        public static Mat<double> ClassicalMds(this in Mat<double> distanceMatrix)
        {
            return ClassicalMultiDimensionalScaling.Fit(distanceMatrix);
        }

        /// <summary>
        /// Applies classical MDS to the given distance matrix.
        /// </summary>
        /// <param name="distanceMatrix">
        /// The distance matrix.
        /// </param>
        /// <param name="dimension">
        /// The number of dimensions in the embedding space.
        /// </param>
        /// <returns>
        /// The embedded coordinate matrix.
        /// If the distance matrix has dimensions <c>n * n</c>, this matrix has dimensions <c>n * <paramref name="dimension"/></c>,
        /// where the i-th row represents the coordinates of the i-th data point.
        /// </returns>
        public static Mat<double> ClassicalMds(this in Mat<double> distanceMatrix, int dimension)
        {
            return ClassicalMultiDimensionalScaling.Fit(distanceMatrix, dimension);
        }

        /// <summary>
        /// Performs kernel principal component analysis (kernel PCA).
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="kernel">
        /// The kernel function applied to the vectors.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="KernelPrincipalComponentAnalysis"/>.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public static KernelPrincipalComponentAnalysis KernelPca(this IReadOnlyList<Vec<double>> xs, Kernel<Vec<double>, Vec<double>> kernel)
        {
            return new KernelPrincipalComponentAnalysis(xs, kernel);
        }
    }
}
