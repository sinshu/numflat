using System;
using System.Collections.Generic;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides principal component analysis (PCA).
    /// </summary>
    public sealed class PrincipalComponentAnalysis : IVectorToVectorInverseTransform<double>
    {
        private readonly Vec<double> mean;
        private readonly EigenValueDecompositionDouble evd;

        /// <summary>
        /// Performs principal component analysis (PCA).
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public PrincipalComponentAnalysis(IEnumerable<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            Vec<double> mean;
            Mat<double> covariance;
            try
            {
                (mean, covariance) = xs.MeanAndCovariance();
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the covariance matrix.", e);
            }

            EigenValueDecompositionDouble evd;
            try
            {
                evd = covariance.Evd();
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the EVD of the covariance matrix.", e);
            }

            this.mean = mean;
            this.evd = evd;
        }

        /// <inheritdoc/>
        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (source.Count != mean.Count)
            {
                throw new ArgumentException($"The transform requires the length of the source vector to be {mean.Count}, but was {source.Count}.", nameof(source));
            }

            if (destination.Count > mean.Count)
            {
                throw new ArgumentException($"The transform requires the length of the destination vector to be within {mean.Count}, but was {destination.Count}.", nameof(destination));
            }

            using var utmp = new TemporalVector<double>(source.Count);
            ref readonly var tmp = ref utmp.Item;

            Vec.Sub(source, mean, tmp);
            Mat.Mul(evd.V.Submatrix(0, 0, mean.Count, destination.Count), tmp, destination, true);
        }

        /// <inheritdoc/>
        public void InverseTransform(in Vec<double> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (source.Count > mean.Count)
            {
                throw new ArgumentException($"The transform requires the length of the source vector to be within {mean.Count}, but was {source.Count}.", nameof(source));
            }

            if (destination.Count != mean.Count)
            {
                throw new ArgumentException($"The transform requires the length of the destination vector to be {mean.Count}, but was {destination.Count}.", nameof(destination));
            }

            Mat.Mul(evd.V.Submatrix(0, 0, mean.Count, source.Count), source, destination, false);
            destination.AddInplace(mean);
        }

        /// <summary>
        /// Gets the mean vector of the source vectors.
        /// </summary>
        public ref readonly Vec<double> Mean => ref mean;

        /// <summary>
        /// Gets the eigenvalues of the covariance matrix.
        /// </summary>
        public ref readonly Vec<double> EigenValues => ref evd.D;

        /// <summary>
        /// Gets the eigenvectors of the covariance matrix.
        /// </summary>
        public ref readonly Mat<double> EigenVectors => ref evd.V;

        /// <inheritdoc/>
        public int SourceDimension => mean.Count;

        /// <inheritdoc/>
        public int DestinationDimension => mean.Count;
    }
}
