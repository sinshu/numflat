using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    public sealed class NonnegativeMatrixFactorization
    {
        public static void Update(IReadOnlyList<Vec<double>> xs, in Mat<double> sourceW, in Mat<double> sourceH, in Mat<double> destinationW, in Mat<double> destinationH)
        {
            //
            // Update H.
            //

            var dimension = sourceW.RowCount;
            var componentCount = sourceW.ColCount;
            var dataCount = xs.Count;

            using var uwtw = new TemporalMatrix<double>(componentCount, componentCount);
            ref readonly var wtw = ref uwtw.Item;

            using var utmp1 = new TemporalMatrix3<double>(componentCount, dataCount);
            ref readonly var wtv = ref utmp1.Item1;
            ref readonly var wtwh = ref utmp1.Item2;
            ref readonly var frac1 = ref utmp1.Item3;

            foreach (var (x, col) in xs.Zip(wtv.Cols))
            {
                Mat.Mul(sourceW, x, col, true);
            }
            Mat.Mul(sourceW, sourceW, wtw, true, false);
            Mat.Mul(wtw, sourceH, wtwh, false, false);
            Mat.PointwiseDiv(wtv, wtwh, frac1);
            Mat.PointwiseMul(sourceH, frac1, destinationH);

            //
            // Update W.
            //

            using var uhht = new TemporalMatrix<double>(componentCount, componentCount);
            ref readonly var hht = ref uhht.Item;

            using var utmp2 = new TemporalMatrix4<double>(dimension, componentCount);
            ref readonly var outer = ref utmp2.Item1;
            ref readonly var vht = ref utmp2.Item2;
            ref readonly var whht = ref utmp2.Item3;
            ref readonly var frac2 = ref utmp2.Item4;

            vht.Clear();
            foreach (var (x, col) in xs.Zip(destinationH.Cols))
            {
                Vec.Outer(x, col, outer);
                vht.AddInplace(outer);
            }
            Mat.Mul(destinationH, destinationH, hht, false, true);
            Mat.Mul(sourceW, hht, whht, false, false);
            Mat.PointwiseDiv(vht, whht, frac2);
            Mat.PointwiseMul(sourceW, frac2, destinationW);
        }
    }
}
