using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides common spatial pattern (CSP).
    /// </summary>
    public sealed class CommonSpatialPattern : IVectorToVectorTransform<double>
    {
        private readonly GeneralizedEigenValueDecompositionDouble gevd;
        private readonly int sourceDimension;

        /// <summary>
        /// Performs common spatial pattern (CSP).
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
        /// <remarks>
        /// This CSP implementation assumes two classes.
        /// Therefore, only 0 or 1 are valid as class indices.
        /// </remarks>
        public CommonSpatialPattern(IReadOnlyList<Vec<double>> xs, IReadOnlyList<int> ys)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(ys, nameof(ys));

            if (xs.Count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one vector.", nameof(xs));
            }

            if (xs[0].Count == 0)
            {
                throw new ArgumentException("Empty vectors are not allowed.", nameof(xs));
            }

            if (xs.Count != ys.Count)
            {
                throw new ArgumentException("The number of source vectors and class indices must match.");
            }

            if (ys.Min() != 0 || ys.Max() != 1)
            {
                if (ys.All(y => y == 0) || ys.All(y => y == 1))
                {
                    throw new ArgumentException("All class indices have the same value. The class indices must include both 0 and 1.", nameof(ys));
                }
                else
                {
                    throw new ArgumentException("The class indices must be either 0 or 1.", nameof(ys));
                }
            }

            var dimension = xs[0].Count;
            var class0 = new List<Vec<double>>();
            var class1 = new List<Vec<double>>();

            for (var i = 0; i < xs.Count; i++)
            {
                var x = xs[i];
                if (x.Count == 0)
                {
                    throw new ArgumentException("Empty vectors are not allowed.", nameof(xs));
                }

                if (x.Count != dimension)
                {
                    throw new ArgumentException("All the vectors must have the same length.", nameof(xs));
                }

                if (ys[i] == 0)
                {
                    class0.Add(x);
                }
                else
                {
                    class1.Add(x);
                }
            }

            Mat<double> covariance0;
            Mat<double> covariance1;
            try
            {
                covariance0 = class0.MeanAndCovariance().Covariance;
                covariance1 = class1.MeanAndCovariance().Covariance;
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the covariance matrices.", e);
            }

            var compositeCovariance = covariance0.Copy();
            compositeCovariance.AddInplace(covariance1);

            GeneralizedEigenValueDecompositionDouble gevd;
            try
            {
                gevd = covariance0.Gevd(compositeCovariance);
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the GEVD of the covariance matrices.", e);
            }

            this.gevd = gevd;
            this.sourceDimension = dimension;
        }

        /// <inheritdoc/>
        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            VectorToVectorTransform.ThrowIfInvalidSize(this, source, destination, nameof(source), nameof(destination));

            Mat.Mul(gevd.V, source, destination, true);
        }

        /// <summary>
        /// Gets the eigenvalues obtained from the generalized eigenvalue decomposition.
        /// </summary>
        public ref readonly Vec<double> EigenValues => ref gevd.D;

        /// <summary>
        /// Gets the eigenvectors obtained from the generalized eigenvalue decomposition.
        /// </summary>
        public ref readonly Mat<double> EigenVectors => ref gevd.V;

        /// <inheritdoc/>
        public int SourceDimension => sourceDimension;

        /// <inheritdoc/>
        public int DestinationDimension => sourceDimension;
    }
}
