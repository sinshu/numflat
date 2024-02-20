using System;
using System.Collections.Generic;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides principal component analysis.
    /// </summary>
    public sealed class PrincipalComponentAnalysis : IVectorToVectorTransform<double>, IVectorToVectorInverseTransform<double>
    {
        private readonly Vec<double> mean;
        private readonly Vec<double> eigenValues;
        private readonly Mat<double> eigenVectors;

        /// <summary>
        /// Performs principal component analysis.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed in the model fitting.
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

            var eigenValues = new Vec<double>(mean.Count);
            var eigenVectors = new Mat<double>(mean.Count, mean.Count);
            try
            {
                SingularValueDecompositionDouble.Decompose(covariance, eigenValues, eigenVectors);
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the EVD of the covariance matrix.", e);
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
            VectorToVectorTransform.ThrowIfInvalidSize(this, source, destination, nameof(source), nameof(destination));

            using var utmp = new TemporalVector<double>(source.Count);
            ref readonly var tmp = ref utmp.Item;

            Vec.Sub(source, mean, tmp);
            Mat.Mul(eigenVectors, tmp, destination, true);
        }

        /// <inheritdoc/>
        public void InverseTransform(in Vec<double> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            VectorToVectorInverseTransform.ThrowIfInvalidSize(this, source, destination, nameof(source), nameof(destination));

            Mat.Mul(eigenVectors, source, destination, false);
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
        public int SourceVectorLength => mean.Count;

        /// <inheritdoc/>
        public int DestinationVectorLength => mean.Count;
    }
}
