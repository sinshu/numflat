﻿using System;

namespace NumFlat.Distributions
{
    /// <summary>
    /// Represents a Gaussian distribution.
    /// </summary>
    public sealed class Gaussian : IMultivariateDistribution<double>
    {
        private readonly Vec<double> mean;
        private readonly Mat<double> covariance;
        private readonly CholeskyDecompositionDouble cholesky;
        private readonly double logNormalizationTerm;

        /// <summary>
        /// Initializes a new Gaussian distribution from a mean vector and covariance matrix.
        /// </summary>
        /// <param name="mean">
        /// The mean vector.
        /// </param>
        /// <param name="covariance">
        /// The covariance matrix.
        /// </param>
        public Gaussian(in Vec<double> mean, in Mat<double> covariance)
        {
            ThrowHelper.ThrowIfEmpty(mean, nameof(mean));
            ThrowHelper.ThrowIfEmpty(covariance, nameof(covariance));
            ThrowHelper.ThrowIfNonSquare(covariance, nameof(covariance));

            if (covariance.RowCount != mean.Count)
            {
                throw new ArgumentException("The length of the mean vector must match the order of the covariance matrix.");
            }

            CholeskyDecompositionDouble cholesky;
            try
            {
                cholesky = covariance.Cholesky();
            }
            catch (Exception e)
            {
                throw new AggregateException("Failed to compute the Cholesky decomposition of the covariance matrix.", e);
            }

            var logNormalizationTerm = -(Math.Log(2 * Math.PI) * mean.Count + cholesky.LogDeterminant()) / 2;

            this.mean = mean;
            this.covariance = covariance;
            this.cholesky = cholesky;
            this.logNormalizationTerm = logNormalizationTerm;
        }

        /// <inheritdoc/>
        public double LogPdf(in Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            MultivariateDistribution.ThrowIfInvalidSize(this, x, nameof(x));

            using var ud = new TemporalVector<double>(mean.Count);
            ref readonly var d = ref ud.Item;
            Vec.Sub(x, mean, d);

            using var uisd = new TemporalVector<double>(mean.Count);
            ref readonly var isd = ref uisd.Item;
            cholesky.Solve(d, isd);

            return logNormalizationTerm - d * isd / 2;
        }

        /// <inheritdoc/>
        public double Pdf(in Vec<double> x)
        {
            return Math.Exp(LogPdf(x));
        }

        /// <summary>
        /// Gets the mean vector.
        /// </summary>
        public ref readonly Vec<double> Mean => ref mean;

        /// <summary>
        /// Gets the covariance matrix.
        /// </summary>
        public ref readonly Mat<double> Covariance => ref covariance;

        /// <inheritdoc/>
        public int Dimension => mean.Count;
    }
}
