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
        private readonly Vec<double> eigenValues;
        private readonly Mat<double> eigenVectors;

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
            this.eigenValues = evd.D;
            this.eigenVectors = evd.V;
        }

        /// <summary>
        /// Creates principal component analysis (PCA) from fitted parameters.
        /// </summary>
        /// <param name="mean">
        /// The mean vector of the source vectors.
        /// </param>
        /// <param name="eigenValues">
        /// The eigenvalues of the covariance matrix.
        /// </param>
        /// <param name="eigenVectors">
        /// The eigenvectors of the covariance matrix.
        /// </param>
        /// <remarks>
        /// This constructor is intended primarily for deserializers that reconstruct a fitted model from persisted parameters.
        /// The given vectors and matrix are stored directly, so they should not be mutated after construction.
        /// </remarks>
        public PrincipalComponentAnalysis(in Vec<double> mean, in Vec<double> eigenValues, in Mat<double> eigenVectors)
        {
            ThrowHelper.ThrowIfEmpty(mean, nameof(mean));
            ThrowHelper.ThrowIfEmpty(eigenValues, nameof(eigenValues));
            ThrowHelper.ThrowIfEmpty(eigenVectors, nameof(eigenVectors));

            if (eigenValues.Count != mean.Count)
            {
                throw new ArgumentException($"The length of the eigenvalue vector must be '{mean.Count}', but was '{eigenValues.Count}'.", nameof(eigenValues));
            }

            if (eigenVectors.RowCount != mean.Count || eigenVectors.ColCount != mean.Count)
            {
                throw new ArgumentException($"The eigenvector matrix must be '{mean.Count} x {mean.Count}', but was '{eigenVectors.RowCount} x {eigenVectors.ColCount}'.", nameof(eigenVectors));
            }

            this.mean = mean;
            this.eigenValues = eigenValues;
            this.eigenVectors = eigenVectors;
        }

        /// <inheritdoc/>
        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (source.Count != mean.Count)
            {
                throw new ArgumentException($"The transform requires the length of the source vector to be '{mean.Count}', but was '{source.Count}'.", nameof(source));
            }

            if (destination.Count > mean.Count)
            {
                throw new ArgumentException($"The destination vector length must be less than or equal to '{mean.Count}', but was '{destination.Count}'.", nameof(destination));
            }

            using var utmp = new TemporalVector<double>(source.Count);
            ref readonly var tmp = ref utmp.Item;

            Vec.Sub(source, mean, tmp);
            Mat.Mul(eigenVectors.Submatrix(0, 0, mean.Count, destination.Count), tmp, destination, true);
        }

        /// <inheritdoc/>
        public void InverseTransform(in Vec<double> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (source.Count > mean.Count)
            {
                throw new ArgumentException($"The source vector length must be less than or equal to '{mean.Count}', but was '{source.Count}'.", nameof(source));
            }

            if (destination.Count != mean.Count)
            {
                throw new ArgumentException($"The transform requires the length of the destination vector to be '{mean.Count}', but was '{destination.Count}'.", nameof(destination));
            }

            Mat.Mul(eigenVectors.Submatrix(0, 0, mean.Count, source.Count), source, destination, false);
            destination.AddInplace(mean);
        }

        /// <summary>
        /// Gets the mean vector of the source vectors.
        /// </summary>
        public ref readonly Vec<double> Mean => ref mean;

        /// <summary>
        /// Gets the eigenvalues of the covariance matrix.
        /// </summary>
        public ref readonly Vec<double> EigenValues => ref eigenValues;

        /// <summary>
        /// Gets the eigenvectors of the covariance matrix.
        /// </summary>
        public ref readonly Mat<double> EigenVectors => ref eigenVectors;

        /// <inheritdoc/>
        public int SourceDimension => mean.Count;

        /// <inheritdoc/>
        public int DestinationDimension => mean.Count;
    }
}
