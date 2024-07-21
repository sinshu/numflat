using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NumFlat.Clustering;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides non-negative matrix factorization (NMF).
    /// </summary>
    public sealed class NonnegativeMatrixFactorization
    {
        public static (Mat<double> W, Mat<double> H) GetInitialGuess(IReadOnlyList<Vec<double>> xs, int componentCount, GaussianMixtureModelOptions? options = null, Random? random = null)
        {
            var pca = xs.Pca();

            using var utransformed = new TemporalMatrix<double>(componentCount, xs.Count);
            ref readonly var transformed = ref utransformed.Item;

            foreach (var (x, col) in xs.Zip(transformed.Cols))
            {
                pca.Transform(x, col);
            }

            var gmm = transformed.Cols.ToGmm(componentCount + 1, options, random);

            var origin = new Vec<double>(componentCount);
            pca.Transform(new Vec<double>(pca.SourceDimension), origin);

            var min = double.MaxValue;
            var discard = -1;
            for (var i = 0; i < gmm.ClassCount; i++)
            {
                var gaussian = gmm.Components[i].Gaussian;
                var distance = Vec.DistanceSquared(gaussian.Mean, origin);
                if (distance < min)
                {
                    min = distance;
                    discard = i;
                }
            }

            //Console.WriteLine("DISCARDED: " + discard);
            using (var writer = new StreamWriter("out.csv"))
            {
                for (var i = 0; i < gmm.Components.Count; i++)
                {
                    if (i != discard)
                    {
                        writer.Write(",C" + i);
                    }
                    else
                    {
                        writer.Write(",Discarded");
                    }
                }
                writer.WriteLine();
                foreach (var x in transformed.Cols)
                {
                    var c = gmm.Predict(x);
                    writer.Write(x[0]);
                    for (var k = 0; k <= c; k++)
                    {
                        writer.Write(",");
                    }
                    writer.WriteLine(x[1]);
                }
            }

            var w = new Mat<double>(pca.SourceDimension, componentCount);
            {
                var c = 0;
                for (var i = 0; i < gmm.ClassCount; i++)
                {
                    if (i == discard)
                    {
                        continue;
                    }
                    pca.InverseTransform(gmm.Components[i].Gaussian.Mean, w.Cols[c]);
                    c++;
                }
            }

            var h = new Mat<double>(componentCount, xs.Count);


            return (w, h);
        }

        /// <summary>
        /// Updates the matrices W and H using the multiplicative update rule for NMF.
        /// </summary>
        /// <param name="xs">
        /// The source vectors used to form matrix V, where each vector from the list is placed as a column vector in matrix V.
        /// </param>
        /// <param name="sourceW">
        /// The source matrix W, containing basis vectors as columns.
        /// </param>
        /// <param name="sourceH">
        /// The source matrix H, containing activation vectors as columns.
        /// </param>
        /// <param name="destinationW">
        /// The destination matrix where the updated W will be stored.
        /// </param>
        /// <param name="destinationH">
        /// The destination matrix where the updated H will be stored.
        /// </param>
        public static void Update(IReadOnlyList<Vec<double>> xs, in Mat<double> sourceW, in Mat<double> sourceH, in Mat<double> destinationW, in Mat<double> destinationH)
        {
            ThrowHelper.ThrowIfEmpty(sourceW, nameof(sourceW));
            ThrowHelper.ThrowIfEmpty(sourceH, nameof(sourceH));
            ThrowHelper.ThrowIfEmpty(destinationW, nameof(destinationW));
            ThrowHelper.ThrowIfEmpty(destinationH, nameof(destinationH));

            if (sourceW.RowCount != destinationW.RowCount || sourceW.ColCount != destinationW.ColCount)
            {
                throw new ArgumentException("The dimensions of 'sourceW' and 'destinationW' must match.");
            }

            if (sourceH.RowCount != destinationH.RowCount || sourceH.ColCount != destinationH.ColCount)
            {
                throw new ArgumentException("The dimensions of 'sourceH' and 'destinationH' must match.");
            }

            if (sourceW.ColCount != sourceH.RowCount)
            {
                throw new ArgumentException("'sourceW.ColCount' and 'sourceH.RowCount' must match.");
            }

            if (sourceH.ColCount != xs.Count)
            {
                throw new ArgumentException("'sourceH.ColCount' and 'xs.Count' must match.");
            }

            var dimension = sourceW.RowCount;
            var componentCount = sourceW.ColCount;
            var dataCount = xs.Count;
            var v = xs.ThrowIfEmptyOrDifferentSize(dimension, nameof(xs));

            using var utmp0 = new TemporalMatrix2<double>(componentCount, componentCount);
            ref readonly var wtw = ref utmp0.Item1;
            ref readonly var hht = ref utmp0.Item2;

            //
            // Update H.
            //

            using var utmp1 = new TemporalMatrix3<double>(componentCount, dataCount);
            ref readonly var wtv = ref utmp1.Item1;
            ref readonly var wtwh = ref utmp1.Item2;
            ref readonly var frac1 = ref utmp1.Item3;

            foreach (var (x, col) in v.Zip(wtv.Cols))
            {
                Mat.Mul(sourceW, x, col, true);
            }
            Mat.Mul(sourceW, sourceW, wtw, true, false);
            Mat.Mul(wtw, sourceH, wtwh, false, false);
            ClampSmallValues(wtwh);
            Mat.PointwiseDiv(wtv, wtwh, frac1);
            Mat.PointwiseMul(sourceH, frac1, destinationH);

            //
            // Update W.
            //

            using var utmp2 = new TemporalMatrix4<double>(dimension, componentCount);
            ref readonly var outer = ref utmp2.Item1;
            ref readonly var vht = ref utmp2.Item2;
            ref readonly var whht = ref utmp2.Item3;
            ref readonly var frac2 = ref utmp2.Item4;

            vht.Clear();
            foreach (var (x, col) in v.Zip(destinationH.Cols))
            {
                Vec.Outer(x, col, outer);
                vht.AddInplace(outer);
            }
            Mat.Mul(destinationH, destinationH, hht, false, true);
            Mat.Mul(sourceW, hht, whht, false, false);
            ClampSmallValues(whht);
            Mat.PointwiseDiv(vht, whht, frac2);
            Mat.PointwiseMul(sourceW, frac2, destinationW);
        }

        private static void ClampSmallValues(in Mat<double> mat)
        {
            foreach (var col in mat.Cols)
            {
                foreach (ref var value in col)
                {
                    if (value < 1.0E-9)
                    {
                        value = 1.0E-9;
                    }
                }
            }
        }
    }
}
