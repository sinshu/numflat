using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides independent component analysis (ICA).
    /// </summary>
    public sealed class IndependentComponentAnalysis : IVectorToVectorTransform<double>, IVectorToVectorInverseTransform<double>
    {
        private int sourceDimension;
        private int componentCount;
        private Vec<double> mean;
        private Mat<double> demixingMatrix;
        private Mat<double> mixingMatrix;

        /// <summary>
        /// Performs independent component analysis (ICA).
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="componentCount">
        /// The number of independent components to be extracted.
        /// </param>
        /// <param name="random">
        /// A random number generator for initialization.
        /// If null, <see cref="Random.Shared"/> is used.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public IndependentComponentAnalysis(IReadOnlyList<Vec<double>> xs, int componentCount, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            var pca = xs.Pca();

            if (!(1 <= componentCount && componentCount < pca.SourceDimension))
            {
                throw new ArgumentException($"The number of components must be between one and the source dimension {pca.SourceDimension}.", nameof(componentCount));
            }

            using var ux = new TemporalMatrix2<double>(componentCount, xs.Count);
            ref readonly var whiten = ref ux.Item1;
            ref readonly var transformed = ref ux.Item2;

            using var uw = new TemporalMatrix2<double>(componentCount, componentCount);
            ref readonly var w = ref uw.Item1;
            ref readonly var prev = ref uw.Item2;

            using var uwm = new TemporalMatrix<double>(componentCount, pca.SourceDimension);
            ref readonly var whiteningMatrix = ref uwm.Item;

            using var ug = new TemporalVector2<double>(xs.Count);
            ref readonly var gwxs = ref ug.Item1;
            ref readonly var gpwxs = ref ug.Item2;

            using var ue = new TemporalVector<double>(componentCount);
            ref readonly var e1 = ref ue.Item;

            using var utmp = new TemporalVector<double>(pca.SourceDimension);
            ref readonly var tmp = ref utmp.Item;

            var scale = pca.EigenValues.Subvector(0, componentCount).Map(Math.Sqrt);

            foreach (var (x, a) in xs.Zip(whiten.Cols))
            {
                pca.TruncatedTransform(x, a, componentCount);
                a.PointwiseDivInplace(scale);
            }

            GetInitialW(random != null ? random : Random.Shared, w);
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

            foreach (var (wmRow, pcaCol, s) in whiteningMatrix.Rows.Zip(pca.EigenVectors.Cols, scale))
            {
                Vec.Div(pcaCol, s, wmRow);
            }
            var demixingMatrix = w * whiteningMatrix;
            foreach (var (x, y) in xs.Zip(transformed.Cols))
            {

                Vec.Sub(x, pca.Mean, tmp);
                Mat.Mul(demixingMatrix, tmp, y, false);
            }
            foreach (var (wmRow, values) in demixingMatrix.Zip(transformed.Rows))
            {
                wmRow.DivInplace(values.Norm());
            }

            this.sourceDimension = pca.SourceDimension;
            this.mean = pca.Mean;
            this.componentCount = componentCount;
            this.demixingMatrix = demixingMatrix;
            this.mixingMatrix = demixingMatrix.PseudoInverse();
        }

        /// <inheritdoc/>
        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            VectorToVectorTransform.ThrowIfInvalidSize(this, source, destination, nameof(source), nameof(destination));

            using var utmp = new TemporalVector<double>(source.Count);
            ref readonly var tmp = ref utmp.Item;

            Vec.Sub(source, mean, tmp);
            Mat.Mul(demixingMatrix, tmp, destination, false);
        }

        /// <inheritdoc/>
        public void InverseTransform(in Vec<double> source, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            VectorToVectorInverseTransform.ThrowIfInvalidSize(this, source, destination, nameof(source), nameof(destination));

            Mat.Mul(mixingMatrix, source, destination, false);
            destination.AddInplace(mean);
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

            using var utmp = new TemporalMatrix3<double>(w.RowCount, w.ColCount);
            ref readonly var k = ref utmp.Item1;
            ref readonly var u = ref utmp.Item2;
            ref readonly var tmp = ref utmp.Item3;

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

        /// <summary>
        /// Gets the mean vector of the source vectors.
        /// </summary>
        public ref readonly Vec<double> Mean => ref mean;

        /// <summary>
        /// Gets the demixing matrix.
        /// </summary>
        public ref readonly Mat<double> DemixingMatrix => ref demixingMatrix;

        /// <summary>
        /// Gets the mixing matrix.
        /// </summary>
        public ref readonly Mat<double> MixingMatrix => ref mixingMatrix;

        /// <inheritdoc/>
        public int SourceDimension => sourceDimension;

        /// <inheritdoc/>
        public int DestinationDimension => componentCount;
    }
}
