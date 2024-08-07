﻿using System;
using System.Numerics;

namespace NumFlat.Distributions
{
    /// <summary>
    /// Provides common functionality across multivariate distributions.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in vectors.
    /// </typeparam>
    public interface IMultivariateDistribution<T> where T : unmanaged, INumberBase<T>
    {
        /// <summary>
        /// Computes the log probability density function of the given vector.
        /// </summary>
        /// <param name="x">
        /// The source vector.
        /// </param>
        /// <returns>
        /// The value of the log probability density function.
        /// </returns>
        public double LogPdf(in Vec<T> x);

        /// <summary>
        /// Computes the probability density function of the given vector.
        /// </summary>
        /// <param name="x">
        /// The source vector.
        /// </param>
        /// <returns>
        /// The value of the probability density function.
        /// </returns>
        public double Pdf(in Vec<T> x);

        /// <summary>
        /// Generates a random vector from the distribution.
        /// </summary>
        /// <param name="random">
        /// The random number generator to use.
        /// </param>
        /// <param name="destination">
        /// The destination of the generated random vector.
        /// </param>
        public void Generate(Random random, in Vec<T> destination);

        /// <summary>
        /// Gets the dimension of the distribution.
        /// </summary>
        public int Dimension { get; }
    }
}
