using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;

namespace NumFlat.MultivariateAnalyses
{
    public sealed class IndependentComponentAnalysis : IVectorToVectorTransform<double>
    {
        private PrincipalComponentAnalysis pca;
        private int componentCount;
        private Mat<double> w;
        private Mat<double> demixingMatrix;

        public IndependentComponentAnalysis(IReadOnlyList<Vec<double>> xs, int componentCount)
        {
            using var uwhiten = new TemporalMatrix<double>(componentCount, xs.Count);
            ref readonly var whiten = ref uwhiten.Item;

            using var uprev = new TemporalMatrix<double>(componentCount, componentCount);
            ref readonly var prev = ref uprev.Item;

            using var utransformed = new TemporalMatrix<double>(componentCount, xs.Count);
            ref readonly var transformed = ref utransformed.Item;

            using var utmp = new TemporalVector2<double>(xs.Count);
            ref readonly var gwxs = ref utmp.Item1;
            ref readonly var gpwxs = ref utmp.Item2;

            using var ue1 = new TemporalVector<double>(componentCount);
            ref readonly var e1 = ref ue1.Item;

            var pca = xs.Pca();
            var scale = pca.EigenValues.Map(Math.Sqrt);

            foreach (var (x, a) in xs.Zip(whiten.Cols))
            {
                pca.Transform(x, a);
                a.PointwiseDivInplace(scale);
            }

            var random = new Random(57);
            var w = GetInitialW(componentCount, random);
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

            var demixingMatrix = new Mat<double>(componentCount, componentCount);
            foreach (var (dmRow, pcaCol, s) in prev.Rows.Zip(pca.EigenVectors.Cols, scale))
            {
                Vec.Div(pcaCol, s, dmRow);
            }
            Mat.Mul(w, prev, demixingMatrix, false, false);

            this.pca = pca;
            this.componentCount = componentCount;
            this.w = w;
            this.demixingMatrix = demixingMatrix;
        }

        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
            using var utmp = new TemporalVector<double>(source.Count);
            ref readonly var tmp = ref utmp.Item;

            Vec.Sub(source, pca.Mean, tmp);
            Mat.Mul(demixingMatrix, tmp, destination, false);
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
