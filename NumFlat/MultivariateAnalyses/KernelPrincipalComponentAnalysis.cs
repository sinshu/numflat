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
        private readonly Vec<double>[] vectors;
        private readonly Kernel<Vec<double>, Vec<double>> kernel;
        private readonly Vec<double> colMeans;
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

            this.vectors = vectors;
            this.kernel = kernel;
            this.colMeans = colMeans;
            this.totalMean = totalMean;
            this.projection = projection;
        }

        /// <inheritdoc/>
        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (source.Count != vectors[0].Count)
            {
                throw new ArgumentException($"The transform requires the length of the source vector to be {vectors[0].Count}, but was {source.Count}.", nameof(source));
            }

            if (destination.Count > vectors.Length)
            {
                throw new ArgumentException($"The transform requires the length of the destination vector to be within {vectors.Length}, but was {destination.Count}.", nameof(destination));
            }

            using var utmp = new TemporalVector<double>(vectors.Length);
            ref readonly var tmp = ref utmp.Item;
            var ft = tmp.GetUnsafeFastIndexer();

            var sum = 0.0;
            for (var i = 0; i < vectors.Length; i++)
            {
                var value = kernel(vectors[i], source);
                ft[i] = value;
                sum += value;
            }

            var mean = sum / vectors.Length;
            var fcm = colMeans.GetUnsafeFastIndexer();
            for (var i = 0; i < vectors.Length; i++)
            {
                ft[i] = ft[i] - fcm[i] - mean + totalMean;
            }

            var components = projection[..vectors.Length, ..destination.Count];
            Mat.Mul(components, tmp, destination, true);
        }

        /// <inheritdoc/>
        public int SourceDimension => vectors[0].Count;

        /// <inheritdoc/>
        public int DestinationDimension => vectors.Length;
    }
}
