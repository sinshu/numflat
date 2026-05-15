using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides linear discriminant analysis (LDA).
    /// </summary>
    public sealed class LinearDiscriminantAnalysis : IVectorToVectorTransform<double>
    {
        private readonly Vec<double> mean;
        private readonly Vec<double> eigenValues;
        private readonly Mat<double> eigenVectors;

        /// <summary>
        /// Performs linear discriminant analysis (LDA).
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ys">
        /// The class indices for each source vector.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public LinearDiscriminantAnalysis(IEnumerable<Vec<double>> xs, IEnumerable<int> ys)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(ys, nameof(ys));

            var groups = EnumerableHelper.GroupByClassIndex(xs, ys);

            Mat<double> sb;
            Mat<double> sw;
            try
            {
                var pairs = groups.Select(group => group.MeanAndCovariance()).ToArray();
                sb = pairs.Select(pair => pair.Mean).Covariance();
                sw = pairs.Select(pair => pair.Covariance).Mean();
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the covariance matrices.", e);
            }

            GeneralizedEigenValueDecompositionDouble gevd;
            try
            {
                gevd = sb.Gevd(sw);
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the GEVD of the covariance matrices.", e);
            }

            this.mean = xs.Mean();
            this.eigenValues = gevd.D;
            this.eigenVectors = gevd.V;
        }

        /// <summary>
        /// Creates linear discriminant analysis (LDA) from fitted parameters.
        /// </summary>
        /// <param name="mean">
        /// The mean vector of the source vectors.
        /// </param>
        /// <param name="eigenValues">
        /// The eigenvalues obtained from the generalized eigenvalue decomposition.
        /// </param>
        /// <param name="eigenVectors">
        /// The eigenvectors obtained from the generalized eigenvalue decomposition.
        /// </param>
        /// <remarks>
        /// This constructor is intended primarily for deserializers that reconstruct a fitted model from persisted parameters.
        /// The given vectors and matrix are stored directly, so they should not be mutated after construction.
        /// </remarks>
        public LinearDiscriminantAnalysis(in Vec<double> mean, in Vec<double> eigenValues, in Mat<double> eigenVectors)
        {
            ThrowHelper.ThrowIfEmpty(mean, nameof(mean));
            ThrowHelper.ThrowIfEmpty(eigenValues, nameof(eigenValues));
            ThrowHelper.ThrowIfEmpty(eigenVectors, nameof(eigenVectors));

            if (eigenValues.Count != mean.Count)
            {
                throw new ArgumentException($"The length of the eigenvalue vector must be {mean.Count}, but was {eigenValues.Count}.", nameof(eigenValues));
            }

            if (eigenVectors.RowCount != mean.Count || eigenVectors.ColCount != mean.Count)
            {
                throw new ArgumentException($"The eigenvector matrix must be {mean.Count} x {mean.Count}, but was {eigenVectors.RowCount} x {eigenVectors.ColCount}.", nameof(eigenVectors));
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

        /// <summary>
        /// Gets the mean vector of the source vectors.
        /// </summary>
        public ref readonly Vec<double> Mean => ref mean;

        /// <summary>
        /// Gets the eigenvalues obtained from the generalized eigenvalue decomposition.
        /// </summary>
        public ref readonly Vec<double> EigenValues => ref eigenValues;

        /// <summary>
        /// Gets the eigenvectors obtained from the generalized eigenvalue decomposition.
        /// </summary>
        public ref readonly Mat<double> EigenVectors => ref eigenVectors;

        /// <inheritdoc/>
        public int SourceDimension => mean.Count;

        /// <inheritdoc/>
        public int DestinationDimension => mean.Count;
    }
}
