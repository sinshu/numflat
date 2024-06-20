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
        private Mat<double> w;

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

            using var ue1 = new TemporalVector<double>(componentCount);
            ref readonly var e1 = ref ue1.Item;

            var scale = pca.EigenValues.Map(Math.Sqrt);

            foreach (var (x, a) in xs.Zip(whiten.Cols))
            {
                pca.Transform(x, a);
                a.PointwiseDivInplace(scale);
            }

            var random = new Random(42);
            var w1 = GetInitialW(componentCount, random);
            Orthogonalize(w1, w2);
            w2.CopyTo(w1);

            for (var ite = 0; ite < 30; ite++)
            {
                Orthogonalize(w1, w2);
                w2.CopyTo(w1);
                for (var c = 0; c < componentCount; c++)
                {
                    var w1c = w1.Rows[c];
                    var w2c = w2.Rows[c];
                    var wxs = transformed.Rows[c];
                    Mat.Mul(whiten, w1c, wxs, true);
                    Vec.Map(wxs, G, gwxs);
                    Vec.Map(wxs, Gp, gpwxs);
                    MultiplyAdd(whiten.Cols, gwxs, e1);
                    e1.DivInplace(xs.Count);
                    var e2 = gpwxs.Average();
                    Vec.Mul(w1c, e2, w1c);
                    Vec.Sub(e1, w1c, w1c);
                }
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

        private static Mat<double> GetInitialW(int componentCount, Random random)
        {
            var w = new Mat<double>(componentCount, componentCount);
            var span = w.Memory.Span;
            for (var i = 0; i < span.Length; i++)
            {
                var sum = 0.0;
                for (var j = 0; j < 12; j++)
                {
                    sum += random.NextDouble();
                }
                span[i] = sum - 6;
            }
            return w;
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

        private static void MultiplyAdd(IEnumerable<Vec<double>> xs, IEnumerable<double> coefficients, Vec<double> destination)
        {
            destination.Clear();
            foreach (var (x, coefficient) in xs.Zip(coefficients))
            {
                var sx = x.Memory.Span;
                var sd = destination.Memory.Span;
                var px = 0;
                var pd = 0;
                while (pd < sd.Length)
                {
                    sd[pd] += coefficient * sx[px];
                    px += x.Stride;
                    pd += destination.Stride;
                }
            }
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
