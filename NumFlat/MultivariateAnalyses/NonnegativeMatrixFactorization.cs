using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    public sealed class NonnegativeMatrixFactorization
    {
        public static void Update(IReadOnlyList<Vec<double>> xs, in Mat<double> sourceW, in Mat<double> sourceH, in Mat<double> destinationW, in Mat<double> destinationH)
        {
            using var uwtw = new TemporalMatrix<double>(sourceW.ColCount, sourceW.ColCount);
            ref readonly var wtw = ref uwtw.Item;

            using var utmp1 = new TemporalMatrix2<double>(sourceW.ColCount, xs.Count);
            ref readonly var wtv = ref utmp1.Item1;
            ref readonly var wtwh = ref utmp1.Item2;

            foreach (var (x, col) in xs.Zip(wtv.Cols))
            {
                Mat.Mul(sourceW, x, col, true);
            }
            Mat.Mul(sourceW, sourceW, wtw, true, false);
            Mat.Mul(wtw, sourceH, wtwh, false, false);

            var frac1 = wtv.PointwiseDiv(wtwh);
            Mat.PointwiseMul(sourceH, frac1, destinationH);

            var vht = new Mat<double>(sourceW.RowCount, sourceW.ColCount);
            foreach (var (x, col) in xs.Zip(destinationH.Cols))
            {
                var outer = x.Outer(col);
                vht.AddInplace(outer);
            }

            var whht = sourceW * destinationH * destinationH.Transpose();

            var frac2 = vht.PointwiseDiv(whht);
            Mat.PointwiseMul(sourceW, frac2, destinationW);
        }
    }
}
