using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    public sealed class IndependentComponentAnalysis : IVectorToVectorTransform<double>
    {
        private PrincipalComponentAnalysis pca;
        private int componentCount;
        private Mat<double> demixingMatrix;

        public IndependentComponentAnalysis(IReadOnlyList<Vec<double>> xs, int componentCount)
        {
            var pca = xs.Pca();

            using var ux = new TemporalMatrix2<double>(componentCount, xs.Count);
            ref readonly var whiten = ref ux.Item1;
            ref readonly var transformed = ref ux.Item2;

            using var uw = new TemporalMatrix2<double>(componentCount, componentCount);
            ref readonly var w = ref uw.Item1;
            ref readonly var prev = ref uw.Item2;

            using var utmp = new TemporalMatrix<double>(componentCount, pca.SourceDimension);
            ref readonly var tmp = ref utmp.Item;

            using var ug = new TemporalVector2<double>(xs.Count);
            ref readonly var gwxs = ref ug.Item1;
            ref readonly var gpwxs = ref ug.Item2;

            using var ue = new TemporalVector<double>(componentCount);
            ref readonly var e1 = ref ue.Item;

            var scale = pca.EigenValues.Subvector(0, componentCount).Map(Math.Sqrt);

            foreach (var (x, a) in xs.Zip(whiten.Cols))
            {
                pca.TruncatedTransform(x, a, componentCount);
                a.PointwiseDivInplace(scale);
            }

            var random = new Random(42);
            GetInitialW(random, w);
            Orthogonalize(w);

            for (var iter = 0; iter < 200; iter++)
            {
                w.CopyTo(prev);

                for (var c = 0; c < componentCount; c++)
                {
                    var wc = w.Rows[c];
                    var wxs = transformed.Rows[c];
                    Mat.Mul(whiten, wc, wxs, true);
                    Vec.Map(wxs, G, gwxs);
                    Vec.Map(wxs, Gp, gpwxs);
                    MultiplyAdd(whiten.Cols, gwxs, e1);
                    e1.DivInplace(xs.Count);
                    var e2 = gpwxs.Average();
                    Vec.Mul(wc, e2, wc);
                    Vec.Sub(e1, wc, wc);
                }
                Orthogonalize(w);

                if (GetMaximumAbsoluteChange(w, prev) < 1.0E-4)
                {
                    break;
                }
            }

            foreach (var (tmpRow, pcaCol, s) in tmp.Rows.Zip(pca.EigenVectors.Cols, scale))
            {
                Vec.Div(pcaCol, s, tmpRow);
            }
            var demixingMatrix = w * tmp;

            this.pca = pca;
            this.componentCount = componentCount;
            this.demixingMatrix = demixingMatrix;
        }

        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
            using var utmp = new TemporalVector<double>(source.Count);
            ref readonly var tmp = ref utmp.Item;

            Vec.Sub(source, pca.Mean, tmp);
            Mat.Mul(demixingMatrix, tmp, destination, false);
        }

        private static void GetInitialW(Random random, in Mat<double> destination)
        {
            var span = destination.Memory.Span;
            for (var i = 0; i < span.Length; i++)
            {
                var sum = 0.0;
                for (var j = 0; j < 12; j++)
                {
                    sum += random.NextDouble();
                }
                span[i] = sum - 6;
            }
        }

        private static void Orthogonalize(in Mat<double> w)
        {
            using var us = new TemporalVector<double>(w.RowCount);
            ref readonly var s = ref us.Item;
            var fs = s.GetUnsafeFastIndexer();

            using var uk = new TemporalMatrix<double>(w.RowCount, w.ColCount);
            ref readonly var k = ref uk.Item;

            using var uu = new TemporalMatrix<double>(w.RowCount, w.ColCount);
            ref readonly var u = ref uu.Item;

            using var utmp = new TemporalMatrix<double>(w.RowCount, w.ColCount);
            ref readonly var tmp = ref utmp.Item;

            SingularValueDecompositionDouble.Decompose(w, s, u);
            for (var col = 0; col < u.ColCount; col++)
            {
                Vec.Div(u.Cols[col], fs[col], tmp.Cols[col]);
            }
            Mat.Mul(tmp, u, k, false, true);
            Mat.Mul(k, w, tmp, false, false);
            tmp.CopyTo(w);
        }

        private static double GetMaximumAbsoluteChange(in Mat<double> w1, in Mat<double> w2)
        {
            using var utmp = new TemporalMatrix<double>(w1.RowCount, w1.ColCount);
            ref readonly var tmp = ref utmp.Item;
            var ft = tmp.GetUnsafeFastIndexer();

            Mat.Mul(w1, w2, tmp, true, false);

            var max = 0.0;
            for (var i = 0; i < w1.RowCount; i++)
            {
                max = Math.Max(Math.Abs(Math.Abs(ft[i, i]) - 1), max);
            }
            return max;
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

        private double G(double value)
        {
            return Math.Tanh(value);
        }

        private double Gp(double value)
        {
            var tanh = Math.Tanh(value);
            return 1 - tanh * tanh;
        }

        /// <inheritdoc/>
        public int SourceDimension => pca.SourceDimension;

        /// <inheritdoc/>
        public int DestinationDimension => componentCount;
    }
}
