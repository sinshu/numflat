using System;
using System.Collections.Generic;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides kernel principal component analysis (kernel PCA).
    /// </summary>
    public sealed class KernelPrincipalComponentAnalysis : IVectorToVectorTransform<double>
    {
        private const double EigenvalueTolerance = 1.0E-12;

        private readonly Vec<double>[] xs;
        private readonly Kernel<Vec<double>, Vec<double>> kernel;
        private readonly Vec<double> rowMeans;
        private readonly double totalMean;
        private readonly Mat<double> projectionMatrix;
        private readonly Vec<double> eigenValues;
        private readonly Mat<double> eigenVectors;
        private readonly int sourceDimension;
        private readonly int sampleCount;

        /// <summary>
        /// Performs kernel principal component analysis (kernel PCA).
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="kernel">
        /// The kernel function applied to the vectors. If null, the linear kernel is used.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public KernelPrincipalComponentAnalysis(IReadOnlyList<Vec<double>> xs, Kernel<Vec<double>, Vec<double>>? kernel = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(xs, nameof(xs));

            var kernelFunction = kernel ?? Kernel.Linear;

            var sampleCount = xs.Count;
            var vectors = new Vec<double>[sampleCount];
            var dimension = -1;

            for (var i = 0; i < sampleCount; i++)
            {
                var x = xs[i];
                ThrowHelper.ThrowIfEmpty(x, nameof(xs));

                if (i == 0)
                {
                    dimension = x.Count;
                }
                else if (x.Count != dimension)
                {
                    throw new ArgumentException("All the vectors must have the same length.", nameof(xs));
                }

                vectors[i] = x;
            }

            var kernelMatrix = new Mat<double>(sampleCount, sampleCount);
            for (var i = 0; i < sampleCount; i++)
            {
                for (var j = 0; j <= i; j++)
                {
                    var value = kernelFunction(vectors[i], vectors[j]);
                    kernelMatrix[i, j] = value;
                    kernelMatrix[j, i] = value;
                }
            }

            var rowMeans = new Vec<double>(sampleCount);
            for (var i = 0; i < sampleCount; i++)
            {
                var sum = 0.0;
                for (var j = 0; j < sampleCount; j++)
                {
                    sum += kernelMatrix[i, j];
                }

                rowMeans[i] = sum / sampleCount;
            }

            var totalMean = 0.0;
            for (var i = 0; i < sampleCount; i++)
            {
                totalMean += rowMeans[i];
            }

            totalMean /= sampleCount;

            for (var i = 0; i < sampleCount; i++)
            {
                for (var j = 0; j < sampleCount; j++)
                {
                    kernelMatrix[i, j] = kernelMatrix[i, j] - rowMeans[i] - rowMeans[j] + totalMean;
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

            var eigenValues = evd.D.Copy();
            var eigenVectors = evd.V.Copy();
            var projectionMatrix = eigenVectors.Copy();

            for (var i = 0; i < sampleCount; i++)
            {
                var value = eigenValues[i];
                if (value > EigenvalueTolerance)
                {
                    projectionMatrix.Cols[i].MulInplace(1.0 / Math.Sqrt(value));
                }
                else
                {
                    eigenValues[i] = 0.0;
                    projectionMatrix.Cols[i].Clear();
                }
            }

            this.xs = vectors;
            this.kernel = kernelFunction;
            this.rowMeans = rowMeans;
            this.totalMean = totalMean;
            this.eigenValues = eigenValues;
            this.eigenVectors = eigenVectors;
            this.projectionMatrix = projectionMatrix;
            this.sourceDimension = dimension;
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
                kernelValues[i] = kernelValues[i] - rowMeans[i] - mean + totalMean;
            }

            var components = projectionMatrix.Submatrix(0, 0, sampleCount, destination.Count);
            Mat.Mul(components, kernelValues, destination, true);
        }

        /// <summary>
        /// Gets the eigenvalues of the centered kernel matrix.
        /// </summary>
        public ref readonly Vec<double> EigenValues => ref eigenValues;

        /// <summary>
        /// Gets the eigenvectors of the centered kernel matrix.
        /// </summary>
        public ref readonly Mat<double> EigenVectors => ref eigenVectors;

        /// <inheritdoc/>
        public int SourceDimension => sourceDimension;

        /// <inheritdoc/>
        public int DestinationDimension => sampleCount;
    }
}
