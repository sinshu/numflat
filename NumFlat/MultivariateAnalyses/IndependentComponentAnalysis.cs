using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    public sealed class IndependentComponentAnalysis : IVectorToVectorTransform<double>
    {
        private PrincipalComponentAnalysis pca;
        private int componentCount;

        public IndependentComponentAnalysis(IReadOnlyList<Vec<double>> xs, int componentCount)
        {
            this.pca = xs.Pca();
            this.componentCount = componentCount;

            var scale = pca.EigenValues.Map(Math.Sqrt);
            var whiten = xs.Select(x => pca.Transform(x).PointwiseDiv(scale)).ToArray();

            Console.WriteLine(whiten.Covariance());

            // Vectors stored as rows.
            // Each row corresponds to a component.
            var w = MatrixBuilder.Identity<double>(componentCount);
            Console.WriteLine(w);
        }

        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
        }

        /// <inheritdoc/>
        public int SourceDimension => pca.SourceDimension;

        /// <inheritdoc/>
        public int DestinationDimension => componentCount;
    }
}
