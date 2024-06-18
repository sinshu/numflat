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

            for (var c = 0; c < componentCount; c++)
            {
                var wc = w.Rows[c];
                var wxs = xs.Select(x => wc * x).ToArray();
                var gwxs = wxs.Select(G).ToArray();
                var gpwxs = wxs.Select(Gp).ToArray();
                var e1 = xs.Zip(gwxs, (x, g) => x * g).Mean();
                var e2 = gpwxs.Average();
                var wc2 = e1 - e2 * wc;
                wc2.CopyTo(wc);
            }

            Console.WriteLine(w);
        }

        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
        }

        /// <inheritdoc/>
        public int SourceDimension => pca.SourceDimension;

        /// <inheritdoc/>
        public int DestinationDimension => componentCount;

        private static double G(double value)
        {
            return Math.Tanh(value);
        }

        private static double Gp(double value)
        {
            var tanh = Math.Tanh(value);
            return 1 - tanh * tanh;
        }
    }
}
