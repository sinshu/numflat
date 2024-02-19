using System;

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
        /// <exception cref="FittingFailureException">
        /// Failed in the model fitting.
        /// </exception>
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
                throw new FittingFailureException("Failed to compute the Cholesky decomposition of the covariance matrix.", e);
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

            using var utmp = new TemporalVector2<double>(mean.Count);
            ref readonly var d = ref utmp.Item1;
            ref readonly var isd = ref utmp.Item2;

            Vec.Sub(x, mean, d);
            cholesky.Solve(d, isd);
            return logNormalizationTerm - d * isd / 2;
        }

        /// <inheritdoc/>
        public double Pdf(in Vec<double> x)
        {
            return Math.Exp(LogPdf(x));
        }

        /// <summary>
        /// Computes the squared Mahalanobis distance.
        /// </summary>
        /// <param name="x">
        /// The vector to compute the Mahalanobis distance.
        /// </param>
        /// <returns>
        /// The squared Mahalanobis distance.
        /// </returns>
        public double MahalanobisSquared(in Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            MultivariateDistribution.ThrowIfInvalidSize(this, x, nameof(x));

            using var utmp = new TemporalVector2<double>(mean.Count);
            ref readonly var d = ref utmp.Item1;
            ref readonly var isd = ref utmp.Item2;

            Vec.Sub(x, mean, d);
            cholesky.Solve(d, isd);
            return d * isd;
        }

        /// <summary>
        /// Computes the Mahalanobis distance.
        /// </summary>
        /// <param name="x">
        /// The vector to compute the Mahalanobis distance.
        /// </param>
        /// <returns>
        /// The Mahalanobis distance.
        /// </returns>
        public double Mahalanobis(in Vec<double> x)
        {
            return Math.Sqrt(MahalanobisSquared(x));
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
