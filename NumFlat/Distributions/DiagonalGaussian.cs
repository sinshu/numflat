using System;
using System.Linq;

namespace NumFlat.Distributions
{
    /// <summary>
    /// Represents a diagonal Gaussian distribution.
    /// </summary>
    public sealed class DiagonalGaussian : IMultivariateDistribution<double>
    {
        private readonly Vec<double> mean;
        private readonly Vec<double> variance;
        private readonly Vec<double> inverseVariance;
        private readonly double logNormalizationTerm;

        /// <summary>
        /// Initializes a new diagonal Gaussian distribution from a mean vector and diagonal elements of a covariance matrix.
        /// </summary>
        /// <param name="mean">
        /// The mean vector.
        /// </param>
        /// <param name="variance">
        /// The diagonal elements of the covariance matrix.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed in the model fitting.
        /// </exception>
        public DiagonalGaussian(in Vec<double> mean, in Vec<double> variance)
        {
            ThrowHelper.ThrowIfEmpty(mean, nameof(mean));
            ThrowHelper.ThrowIfEmpty(variance, nameof(variance));
            ThrowHelper.ThrowIfDifferentSize(mean, variance);

            var logDeterminant = variance.Select(value => Math.Log(value)).Sum();
            var logNormalizationTerm = -(Math.Log(2 * Math.PI) * mean.Count + logDeterminant) / 2;

            var tolerance = Special.Eps(variance.Max()) * mean.Count;
            var inverseVariance = new Vec<double>(mean.Count);
            for (var i = 0; i < inverseVariance.Count; i++)
            {
                var value = variance[i];
                if (value > tolerance)
                {
                    inverseVariance[i] = 1 / value;
                }
                else
                {
                    throw new FittingFailureException("Variance is too small.");
                }
            }

            this.mean = mean;
            this.variance = variance;
            this.inverseVariance = inverseVariance;
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
            Vec.PointwiseMul(d, inverseVariance, isd);
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
            Vec.PointwiseMul(d, inverseVariance, isd);
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
        /// Gets the diagonal elements of the covariance matrix.
        /// </summary>
        public ref readonly Vec<double> Variance => ref variance;

        /// <inheritdoc/>
        public int Dimension => mean.Count;
    }
}
