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
            var svd = covariance.Svd();

            this.mean = mean;
            this.covariance = covariance;
            this.svd = svd;
        }

        /// <summary>
        /// Transforms a vector.
        /// </summary>
        /// <param name="x">
        /// The source vector to be transformed.
        /// </param>
        /// <param name="destination">
        /// The destination of the transformed vector.
        /// </param>
        public void Transform(in Vec<double> x, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            if (x.Count != mean.Count)
            {
                throw new ArgumentException("The length of the source vector does not meet the requirement.");
            }

            using var utmp = new TemporalVector<double>(x.Count);
            ref readonly var tmp = ref utmp.Item;

            Vec.Sub(x, mean, tmp);
            Mat.Mul(svd.U, tmp, destination, true);
        }

        /// <summary>
        /// Transforms a vector.
        /// </summary>
        /// <param name="x">
        /// The source vector to be transformed.
        /// </param>
        /// <returns>
        /// The transformed vector.
        /// </returns>
        public Vec<double> Transform(in Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            if (x.Count != mean.Count)
            {
                throw new ArgumentException("The length of the source vector does not meet the requirement.");
            }

            var destination = new Vec<double>(x.Count);
            Transform(x, destination);
            return destination;
        }
    }
}
