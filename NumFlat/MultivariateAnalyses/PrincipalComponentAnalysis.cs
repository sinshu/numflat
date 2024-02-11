using System;
using System.Collections.Generic;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides principal component analysis.
    /// </summary>
    public class PrincipalComponentAnalysis
    {
        private Vec<double> mean;
        private Mat<double> covariance;
        private EigenValueDecompositionDouble evd;

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
            var evd = covariance.Evd();

            this.mean = mean;
            this.covariance = covariance;
            this.evd = evd;
        }

        /// <summary>
        /// Transforms a vector.
        /// </summary>
        /// <param name="x">
        /// The vector to be transformed.
        /// </param>
        /// <returns>
        /// The transformed vector.
        /// </returns>
        public Vec<double> Transform(in Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            return evd.V * (x - mean);
        }
    }
}
