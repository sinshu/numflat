using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides linear discriminant analysis.
    /// </summary>
    public sealed class LinearDiscriminantAnalysis : IVectorToVectorTransform<double>
    {
        private readonly Vec<double> mean;
        private readonly GeneralizedEigenValueDecompositionDouble gevd;

        /// <summary>
        /// Performs linear discriminant analysis.
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

            var groups = ArgumentHelper.GroupByClassIndex(xs, ys);

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
                Special.ReverseEigenValueOrder(gevd);
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the GEVD of the covariance matrices.", e);
            }

            this.mean = xs.Mean();
            this.gevd = gevd;
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
            Mat.Mul(gevd.V, tmp, destination, true);
        }

        /// <summary>
        /// Gets the mean vector of the source vectors.
        /// </summary>
        public ref readonly Vec<double> Mean => ref mean;

        /// <summary>
        /// Gets the eigenvalues obtained from the generalized eigenvalue decomposition.
        /// </summary>
        public ref readonly Vec<double> EigenValues => ref gevd.D;

        /// <summary>
        /// Gets the eigenvectors obtained from the generalized eigenvalue decomposition.
        /// </summary>
        public ref readonly Mat<double> EigenVectors => ref gevd.V;

        /// <inheritdoc/>
        public int SourceVectorLength => mean.Count;

        /// <inheritdoc/>
        public int DestinationVectorLength => mean.Count;
    }
}
