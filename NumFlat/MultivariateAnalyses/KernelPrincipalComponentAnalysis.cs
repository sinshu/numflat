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
        private readonly Mat<double> sourceVectors;
        private readonly Kernel<Vec<double>, Vec<double>> kernel;
        private readonly Vec<double> kernelMeans;
        private readonly double totalMean;
        private readonly Mat<double> projection;

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
            var dimension = xs[0].Count;
            var sourceVectors = new Mat<double>(dimension, sampleCount);
            foreach (var (x, col) in xs.ThrowIfEmptyOrDifferentSize(nameof(xs)).Zip(sourceVectors.Cols))
            {
                x.CopyTo(col);
            }

            var kernelMatrix = new Mat<double>(sampleCount, sampleCount);
            var fkmat = kernelMatrix.GetUnsafeFastIndexer();
            for (var i = 0; i < sampleCount; i++)
            {
                var vec1 = sourceVectors.Cols[i];
                for (var j = 0; j <= i; j++)
                {
                    var vec2 = sourceVectors.Cols[j];
                    var value = kernel(vec1, vec2);
                    fkmat[i, j] = value;
                    fkmat[j, i] = value;
                }
            }

            var kernelMeans = kernelMatrix.Cols.Mean();
            var totalMean = kernelMeans.Average();
            var fkmean = kernelMeans.GetUnsafeFastIndexer();
            for (var i = 0; i < sampleCount; i++)
            {
                for (var j = 0; j < sampleCount; j++)
                {
                    fkmat[i, j] = fkmat[i, j] - fkmean[i] - fkmean[j] + totalMean;
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

            this.sourceVectors = sourceVectors;
            this.kernel = kernel;
            this.kernelMeans = kernelMeans;
            this.totalMean = totalMean;
            this.projection = projection;
        }

        /// <inheritdoc/>
        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (source.Count != sourceVectors.RowCount)
            {
                throw new ArgumentException($"The transform requires the length of the source vector to be {sourceVectors.RowCount}, but was {source.Count}.", nameof(source));
            }

            if (destination.Count > sourceVectors.ColCount)
            {
                throw new ArgumentException($"The transform requires the length of the destination vector to be within {sourceVectors.ColCount}, but was {destination.Count}.", nameof(destination));
            }

            using var utmp = new TemporalVector<double>(sourceVectors.ColCount);
            ref readonly var tmp = ref utmp.Item;
            var ftmp = tmp.GetUnsafeFastIndexer();

            var sum = 0.0;
            for (var i = 0; i < sourceVectors.ColCount; i++)
            {
                var value = kernel(sourceVectors.Cols[i], source);
                ftmp[i] = value;
                sum += value;
            }

            var mean = sum / sourceVectors.ColCount;
            var fkmean = kernelMeans.GetUnsafeFastIndexer();
            for (var i = 0; i < sourceVectors.ColCount; i++)
            {
                ftmp[i] = ftmp[i] - fkmean[i] - mean + totalMean;
            }

            var components = projection[.., ..destination.Count];
            Mat.Mul(components, tmp, destination, true);
        }

        /// <inheritdoc/>
        public int SourceDimension => sourceVectors.RowCount;

        /// <inheritdoc/>
        public int DestinationDimension => sourceVectors.ColCount;
    }
}
