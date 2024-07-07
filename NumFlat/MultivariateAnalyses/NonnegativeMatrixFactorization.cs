using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    public sealed class NonnegativeMatrixFactorization
    {
        public static void Update(IReadOnlyList<Vec<double>> xs, in Mat<double> sourceW, in Mat<double> sourceH, in Mat<double> destinationW, in Mat<double> destinationH)
        {
            var wtv = new Mat<double>(sourceW.ColCount, xs.Count);
            foreach (var (x, col) in xs.Zip(wtv.Cols))
            {
                Mat.Mul(sourceW, x, col, true);
            }

            var wtwh = sourceW.Transpose() * sourceW * sourceH;

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
