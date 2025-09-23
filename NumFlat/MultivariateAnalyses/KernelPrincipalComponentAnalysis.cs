using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides kernel principal component analysis (kernel PCA).
    /// </summary>
    public sealed class KernelPrincipalComponentAnalysis : IVectorToVectorTransform<double>
    {
        private readonly Vec<double>[] xs;
        private readonly Kernel<Vec<double>, Vec<double>> kernel;
        private readonly Vec<double> colMeans;
        private readonly double totalMean;
        private readonly Mat<double> projectionMatrix;
        private readonly int sourceDimension;
        private readonly int sampleCount;

        /// <summary>
        /// Performs kernel principal component analysis (kernel PCA).
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="kernel">
        /// The kernel function applied to the vectors..
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public KernelPrincipalComponentAnalysis(IReadOnlyList<Vec<double>> xs, Kernel<Vec<double>, Vec<double>> kernel)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(kernel, nameof(kernel));

            var sampleCount = xs.Count;
            var vectors = new Vec<double>[sampleCount];
            {
                var i = 0;
                foreach (var x in xs.ThrowIfEmptyOrDifferentSize(nameof(xs)))
                {
                    vectors[i] = x.Copy();
                    i++;
                }
            }

            var kernelMatrix = new Mat<double>(sampleCount, sampleCount);
            var fkm = kernelMatrix.GetUnsafeFastIndexer();
            for (var i = 0; i < sampleCount; i++)
            {
                for (var j = 0; j <= i; j++)
                {
                    var value = kernel(vectors[i], vectors[j]);
                    fkm[i, j] = value;
                    fkm[j, i] = value;
                }
            }

            var colMeans = kernelMatrix.Cols.Mean();
            var fcm = colMeans.GetUnsafeFastIndexer();
            var totalMean = colMeans.Average();
            for (var i = 0; i < sampleCount; i++)
            {
                for (var j = 0; j < sampleCount; j++)
                {
                    fkm[i, j] = fkm[i, j] - fcm[i] - fcm[j] + totalMean;
                }
            }

            EigenValueDecompositionDouble evd;
            try
            {
                evd = kernelMatrix.Evd();
            }
            catch (Exception e)
            {
                throw new FittingFailureException("Failed to compute the EVD of the kernel matrix.", e);
            }

            var rank = evd.Rank();
            var projection = evd.V;
            {
                var i = 0;
                foreach (var (eigenValue, col) in evd.D.Zip(projection.Cols))
                {
                    if (i < rank)
                    {
                        col.DivInplace(Math.Sqrt(eigenValue));
                    }
                    else
                    {
                        col.Clear();
                    }
                    i++;
                }
            }

            this.xs = vectors;
            this.kernel = kernel;
            this.colMeans = colMeans;
            this.totalMean = totalMean;
            this.projectionMatrix = projection;
            this.sourceDimension = vectors[0].Count;
            this.sampleCount = sampleCount;
        }

        /// <inheritdoc/>
        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (source.Count != sourceDimension)
            {
                throw new ArgumentException($"The transform requires the length of the source vector to be {sourceDimension}, but was {source.Count}.", nameof(source));
            }

            if (destination.Count > sampleCount)
            {
                throw new ArgumentException($"The transform requires the length of the destination vector to be within {sampleCount}, but was {destination.Count}.", nameof(destination));
            }

            using var utmp = new TemporalVector<double>(sampleCount);
            var kernelValues = utmp.Item;

            var sum = 0.0;
            for (var i = 0; i < sampleCount; i++)
            {
                var value = kernel(xs[i], source);
                kernelValues[i] = value;
                sum += value;
            }

            var mean = sum / sampleCount;
            for (var i = 0; i < sampleCount; i++)
            {
                kernelValues[i] = kernelValues[i] - colMeans[i] - mean + totalMean;
            }

            var components = projectionMatrix.Submatrix(0, 0, sampleCount, destination.Count);
            Mat.Mul(components, kernelValues, destination, true);
        }

        /// <inheritdoc/>
        public int SourceDimension => sourceDimension;

        /// <inheritdoc/>
        public int DestinationDimension => sampleCount;
    }
}
