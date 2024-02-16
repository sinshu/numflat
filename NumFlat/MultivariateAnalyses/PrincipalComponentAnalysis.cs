using System;
using System.Collections.Generic;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides principal component analysis.
    /// </summary>
    public class PrincipalComponentAnalysis : IVectorToVectorTransform<double>
    {
        private Vec<double> mean;
        private SingularValueDecompositionDouble svd;

        /// <summary>
        /// Performs principal component analysis.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        public PrincipalComponentAnalysis(IEnumerable<Vec<double>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            var (mean, covariance) = xs.MeanAndCovariance();

            this.mean = mean;
            this.svd = covariance.Svd();
        }

        /// <summary>
        /// Transforms a vector.
        /// </summary>
        /// <param name="source">
        /// The source vector to be transformed.
        /// </param>
        /// <param name="destination">
        /// The destination of the transformed vector.
        /// </param>
        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfDifferentSize(source, destination);
            this.ThrowIfInvalidSize(source);

            using var utmp = new TemporalVector<double>(source.Count);
            ref readonly var tmp = ref utmp.Item;

            Vec.Sub(source, mean, tmp);
            Mat.Mul(svd.U, tmp, destination, true);
        }

        /// <inheritdoc/>
        public int SourceVectorLength => mean.Count;

        /// <inheritdoc/>
        public int DestinationVectorLength => mean.Count;
    }
}
