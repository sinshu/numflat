using System;
using System.Collections.Generic;
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
        private readonly Vec<double> standardDeviation;
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
        /// Failed to fit the model.
        /// </exception>
        public DiagonalGaussian(in Vec<double> mean, in Vec<double> variance)
        {
            ThrowHelper.ThrowIfEmpty(mean, nameof(mean));
            ThrowHelper.ThrowIfEmpty(variance, nameof(variance));
            ThrowHelper.ThrowIfDifferentSize(mean, variance);

            var (inverseVariance, logNormalizationTerm) = GetInverseVarianceAndLogNormalizationTerm(mean, variance);

            this.mean = mean;
            this.variance = variance;
            this.standardDeviation = variance.Map(Math.Sqrt);
            this.inverseVariance = inverseVariance;
            this.logNormalizationTerm = logNormalizationTerm;
        }

        /// <summary>
        /// Computes the maximum likelihood diagonal Gaussian distribution from the source vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="regularization">
        /// The amount of regularization.
        /// This value will be added to the diagonal elements of the covariance matrix.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public DiagonalGaussian(IEnumerable<Vec<double>> xs, double regularization = 0.0)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (regularization < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(regularization), "The amount of regularization must be a non-negative value.");
            }

            Vec<double> mean;
            Vec<double> variance;
            try
            {
                (mean, variance) = xs.MeanAndVariance(0);
                variance.AddInplace(regularization);
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the pointwise variance.", e);
            }

            var (inverseVariance, logNormalizationTerm) = GetInverseVarianceAndLogNormalizationTerm(mean, variance);

            this.mean = mean;
            this.variance = variance;
            this.inverseVariance = inverseVariance;
            this.logNormalizationTerm = logNormalizationTerm;
        }

        /// <summary>
        /// Computes the maximum likelihood diagonal Gaussian distribution from the source vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="weights">
        /// The weights of the source vectors.
        /// </param>
        /// <param name="regularization">
        /// The amount of regularization.
        /// This value will be added to the diagonal elements of the covariance matrix.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public DiagonalGaussian(IEnumerable<Vec<double>> xs, IEnumerable<double> weights, double regularization = 0.0)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));

            if (regularization < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(regularization), "The amount of regularization must be a non-negative value.");
            }

            Vec<double> mean;
            Vec<double> variance;
            try
            {
                (mean, variance) = xs.MeanAndVariance(weights, 0);
                variance.AddInplace(regularization);
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the pointwise variance.", e);
            }

            var (inverseVariance, logNormalizationTerm) = GetInverseVarianceAndLogNormalizationTerm(mean, variance);

            this.mean = mean;
            this.variance = variance;
            this.inverseVariance = inverseVariance;
            this.logNormalizationTerm = logNormalizationTerm;
        }

        private static (Vec<double>, double) GetInverseVarianceAndLogNormalizationTerm(in Vec<double> mean, in Vec<double> variance)
        {
            var logDeterminant = variance.Select(value => Math.Log(value)).Sum();
            var logNormalizationTerm = -(Math.Log(2 * Math.PI) * mean.Count + logDeterminant) / 2;

            var inverseVariance = new Vec<double>(mean.Count);
            var fv = variance.GetUnsafeFastIndexer();
            var fiv = inverseVariance.GetUnsafeFastIndexer();
            for (var i = 0; i < inverseVariance.Count; i++)
            {
                var value = fv[i];
                if (value > 1.0E-14) // np.finfo(np.float64).resolution * 10
                {
                    fiv[i] = 1 / value;
                }
                else
                {
                    throw new FittingFailureException("Variance is too small.");
                }
            }

            return (inverseVariance, logNormalizationTerm);
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

        /// <inheritdoc/>
        public void Generate(Random random, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfNull(random, nameof(random));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            MultivariateDistribution.ThrowIfInvalidSize(this, destination, nameof(destination));

            var fm = mean.GetUnsafeFastIndexer();
            var fs = standardDeviation.GetUnsafeFastIndexer();
            var fd = destination.GetUnsafeFastIndexer();
            for (var i = 0; i < mean.Count; i++)
            {
                fd[i] = fm[i] + random.NextGaussian() * fs[i];
            }
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
        /// Computes the Bhattacharyya distance.
        /// </summary>
        /// <param name="x">
        /// The Gaussian to compute the Bhattacharyya distance.
        /// </param>
        /// <returns>
        /// The Bhattacharyya distance.
        /// </returns>
        public double Bhattacharyya(DiagonalGaussian x)
        {
            ThrowHelper.ThrowIfNull(x, nameof(x));

            if (x.mean.Count != this.mean.Count)
            {
                throw new ArgumentException("The distributions must have the same dimension.");
            }

            using var utmp = new TemporalVector3<double>(this.mean.Count);
            ref readonly var sigma = ref utmp.Item1;
            ref readonly var d = ref utmp.Item2;
            ref readonly var tmp = ref utmp.Item3;

            Vec.Add(this.variance, x.variance, sigma);
            sigma.MulInplace(0.5);

            Vec.Sub(x.mean, this.mean, d);
            Vec.PointwiseDiv(d, sigma, tmp);
            var left = Vec.Dot(d, tmp) / 8;
            var right = (LogDeterminant(sigma) - (LogDeterminant(x.variance) + LogDeterminant(this.variance)) / 2) / 2;

            return left + right;
        }

        private static double LogDeterminant(in Vec<double> variance)
        {
            var sum = 0.0;
            foreach (var value in variance)
            {
                sum += Math.Log(value);
            }
            return sum;
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
