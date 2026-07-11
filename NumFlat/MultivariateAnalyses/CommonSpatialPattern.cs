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
        private readonly Vec<double> eigenValues;
        private readonly Mat<double> eigenVectors;

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

            var d = xs[0].Count;

            using var uzero = new TemporalVector<double>(d);
            ref readonly var zero = ref uzero.Item;
            zero.Clear();

            using var ucov = new TemporalMatrix3<double>(d, d);
            ref readonly var cov0 = ref ucov.Item1;
            ref readonly var cov1 = ref ucov.Item2;
            ref readonly var cpc = ref ucov.Item3;

            var class0 = new List<Vec<double>>();
            var class1 = new List<Vec<double>>();
            foreach (var (x, y) in xs.ThrowIfEmptyOrDifferentSize(nameof(xs)).Zip(ys))
            {
                if (y == 0)
                {
                    class0.Add(x);
                }
                else
                {
                    class1.Add(x);
                }
            }
            MathLinq.Covariance(class0, zero, cov0, 0);
            MathLinq.Covariance(class1, zero, cov1, 0);
            Mat.Add(cov0, cov1, cpc);

            GeneralizedEigenValueDecompositionDouble gevd;
            try
            {
                gevd = cov0.Gevd(cpc);
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the GEVD of the covariance matrices.", e);
            }

            this.eigenValues = gevd.D;
            this.eigenVectors = gevd.V;
        }

        /// <summary>
        /// Creates common spatial pattern (CSP) from fitted parameters.
        /// </summary>
        /// <param name="eigenValues">
        /// The eigenvalues obtained from the generalized eigenvalue decomposition.
        /// </param>
        /// <param name="eigenVectors">
        /// The eigenvectors obtained from the generalized eigenvalue decomposition.
        /// </param>
        /// <remarks>
        /// This constructor is intended primarily for deserializers that reconstruct a fitted model from persisted parameters.
        /// The given vector and matrix are stored directly, so they should not be mutated after construction.
        /// </remarks>
        public CommonSpatialPattern(in Vec<double> eigenValues, in Mat<double> eigenVectors)
        {
            ThrowHelper.ThrowIfEmpty(eigenValues, nameof(eigenValues));
            ThrowHelper.ThrowIfEmpty(eigenVectors, nameof(eigenVectors));

            if (eigenVectors.RowCount != eigenValues.Count || eigenVectors.ColCount != eigenValues.Count)
            {
                throw new ArgumentException($"The eigenvector matrix must be '{eigenValues.Count} x {eigenValues.Count}', but was '{eigenVectors.RowCount} x {eigenVectors.ColCount}'.", nameof(eigenVectors));
            }

            this.eigenValues = eigenValues;
            this.eigenVectors = eigenVectors;
        }

        /// <inheritdoc/>
        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            VectorToVectorTransform.ThrowIfInvalidSize(this, source, destination, nameof(source), nameof(destination));

            Mat.Mul(eigenVectors, source, destination, true);
        }

        /// <summary>
        /// Gets the eigenvalues obtained from the generalized eigenvalue decomposition.
        /// </summary>
        public ref readonly Vec<double> EigenValues => ref eigenValues;

        /// <summary>
        /// Gets the eigenvectors obtained from the generalized eigenvalue decomposition.
        /// </summary>
        public ref readonly Mat<double> EigenVectors => ref eigenVectors;

        /// <inheritdoc/>
        public int SourceDimension => eigenValues.Count;

        /// <inheritdoc/>
        public int DestinationDimension => eigenValues.Count;
    }
}
