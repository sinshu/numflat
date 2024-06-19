using System;
using System.Collections.Generic;
using System.IO;
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

            using var uwhiten = new TemporalMatrix<double>(xs[0].Count, xs.Count);
            ref readonly var whiten = ref uwhiten.Item;

            using var uw2 = new TemporalMatrix<double>(componentCount, componentCount);
            ref readonly var w2 = ref uw2.Item;

            using var utransformed = new TemporalMatrix<double>(xs[0].Count, xs.Count);
            ref readonly var transformed = ref utransformed.Item;

            using var utmp = new TemporalVector2<double>(xs.Count);
            ref readonly var gwxs = ref utmp.Item1;
            ref readonly var gpwxs = ref utmp.Item2;

            var scale = pca.EigenValues.Map(Math.Sqrt);

            foreach (var (x, a) in xs.Zip(whiten.Cols))
            {
                pca.Transform(x, a);
                a.PointwiseDivInplace(scale);
            }
            Console.WriteLine(whiten.Cols.Covariance());

            // Vectors stored as rows.
            // Each row corresponds to a component.
            var random = new Random(42);
            var w1 = new Mat<double>(componentCount, componentCount);
            foreach (ref var value in w1.Memory.Span)
            {
                value = 2 * random.NextDouble() - 1;
            }

            Console.ReadKey();

            for (var ite = 0; ite < 30; ite++)
            {
                Orthogonalize(w1, w2);
                w2.CopyTo(w1);

                for (var c = 0; c < componentCount; c++)
                {
                    var wc = w1.Rows[c];
                    var wxs = transformed.Rows[c];
                    Mat.Mul(whiten, wc, wxs, true);
                    Vec.Map(wxs, G, gwxs);
                    Vec.Map(wxs, Gp, gpwxs);                 
                    var e1 = whiten.Cols.Zip(gwxs, (x, g) => x * g).Mean();
                    var e2 = gpwxs.Average();
                    var wc2 = e1 - e2 * wc;
                    wc2.CopyTo(wc);
                }

                Console.WriteLine(ite);
            }

            using (var writer = new StreamWriter("ica.csv"))
            {
                foreach (var y in whiten.Cols.Select(x => w1 * x))
                {
                    writer.WriteLine(string.Join(',', y));
                }
            }
        }

        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
        }

        private static void Orthogonalize(in Mat<double> source, in Mat<double> destination)
        {
            using var us = new TemporalVector<double>(source.RowCount);
            ref readonly var s = ref us.Item;
            var fs = s.GetUnsafeFastIndexer();

            using var uk = new TemporalMatrix<double>(source.RowCount, source.ColCount);
            ref readonly var k = ref uk.Item;

            using var utmp = new TemporalMatrix<double>(source.RowCount, source.ColCount);
            ref readonly var tmp = ref utmp.Item;

            SingularValueDecompositionDouble.Decompose(source, s, destination);
            for (var col = 0; col < destination.ColCount; col++)
            {
                Vec.Div(destination.Cols[col], fs[col], tmp.Cols[col]);
            }
            Mat.Mul(tmp, destination, k, false, true);
            Mat.Mul(k, source, destination, false, false);
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
