using System;
using System.Collections.Generic;

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
        /// Failed to fit the model.
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

            var (cholesky, logNormalizationTerm) = GetCholeskyAndLogNormalizationTerm(mean, covariance); 

            this.mean = mean;
            this.covariance = covariance;
            this.cholesky = cholesky;
            this.logNormalizationTerm = logNormalizationTerm;
        }

        /// <summary>
        /// Computes the maximum likelihood Gaussian distribution from the source vectors.
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
        public Gaussian(IEnumerable<Vec<double>> xs, double regularization = 0.0)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (regularization < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(regularization), "The amount of regularization must be a non-negative value.");
            }

            Vec<double> mean;
            Mat<double> covariance;
            try
            {
                (mean, covariance) = xs.MeanAndCovariance();
                Special.IncreaseDiagonalElementsInplace(covariance, regularization);
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the covariance matrix.", e);
            }

            var (cholesky, logNormalizationTerm) = GetCholeskyAndLogNormalizationTerm(mean, covariance);

            this.mean = mean;
            this.covariance = covariance;
            this.cholesky = cholesky;
            this.logNormalizationTerm = logNormalizationTerm;
        }

        /// <summary>
        /// Computes the maximum likelihood Gaussian distribution from the source vectors.
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
        public Gaussian(IEnumerable<Vec<double>> xs, IEnumerable<double> weights, double regularization = 0.0)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));

            if (regularization < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(regularization), "The amount of regularization must be a non-negative value.");
            }

            Vec<double> mean;
            Mat<double> covariance;
            try
            {
                (mean, covariance) = xs.MeanAndCovariance(weights);
                Special.IncreaseDiagonalElementsInplace(covariance, regularization);
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the covariance matrix.", e);
            }

            var (cholesky, logNormalizationTerm) = GetCholeskyAndLogNormalizationTerm(mean, covariance);

            this.mean = mean;
            this.covariance = covariance;
            this.cholesky = cholesky;
            this.logNormalizationTerm = logNormalizationTerm;
        }

        private static (CholeskyDecompositionDouble, double) GetCholeskyAndLogNormalizationTerm(in Vec<double> mean, in Mat<double> covariance)
        {
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

            return (cholesky, logNormalizationTerm);
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
        /// Computes the Bhattacharyya distance.
        /// </summary>
        /// <param name="x">
        /// The Gaussian to compute the Bhattacharyya distance.
        /// </param>
        /// <returns>
        /// The Bhattacharyya distance.
        /// </returns>
        public double Bhattacharyya(Gaussian x)
        {
            ThrowHelper.ThrowIfNull(x, nameof(x));

            if (x.mean.Count != this.mean.Count)
            {
                throw new ArgumentException("The distributions must have the same dimension.");
            }

            using var ul = new TemporalMatrix<double>(this.covariance.RowCount, this.covariance.ColCount);
            ref readonly var l = ref ul.Item;
            Mat.Add(this.covariance, x.covariance, l);
            l.MulInplace(0.5);
            CholeskyDecompositionDouble.Decompose(l, l);

            using var utmp = new TemporalVector2<double>(this.mean.Count);
            ref readonly var d = ref utmp.Item1;
            ref readonly var tmp = ref utmp.Item2;

            Vec.Sub(x.mean, this.mean, d);
            CholeskyDecompositionDouble.Solve(l, d, tmp);
            var left = Vec.Dot(d, tmp) / 8;
            var right = (LogDeterminant(l) - (this.cholesky.LogDeterminant() + x.cholesky.LogDeterminant()) / 2) / 2;

            return left + right;
        }

        private static double LogDeterminant(in Mat<double> l)
        {
            var fl = l.GetUnsafeFastIndexer();
            var value = 0.0;
            for (var i = 0; i < l.RowCount; i++)
            {
                value += Math.Log(fl[i, i]);
            }
            return 2 * value;
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
