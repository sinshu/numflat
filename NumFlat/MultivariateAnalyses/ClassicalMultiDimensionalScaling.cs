using System;

namespace NumFlat.MultivariateAnalyses
{
    public static class ClassicalMultiDimensionalScaling
    {
        public static Mat<double> Fit(in Mat<double> distanceMatrix, int dimension)
        {
            var n = distanceMatrix.RowCount;

            using var utmp = new TemporalMatrix4<double>(n, n);
            ref readonly var d2 = ref utmp.Item1;
            ref readonly var j = ref utmp.Item2;
            ref readonly var b = ref utmp.Item3;
            ref readonly var tmp = ref utmp.Item4;

            using var uvec = new TemporalVector<double>(n);
            ref readonly var vec = ref uvec.Item;

            Mat.Map(distanceMatrix, x => x * x, d2);

            j.Fill(-1.0 / n);
            foreach (ref var value in j.EnumerateDiagonalElements())
            {
                value += 1;
            }

            Mat.Mul(j, d2, tmp, false, false);
            Mat.Mul(tmp, j, b, false, false);
            b.MulInplace(-0.5);

            EigenValueDecompositionDouble.Decompose(b, vec, tmp);

            //var evd = b.Evd();
            var x = tmp * vec.Map(Math.Sqrt).ToDiagonalMatrix();
            return x;
        }

        public static Mat<double> Fit(in Mat<double> distanceMatrix)
        {
            return Fit(distanceMatrix, distanceMatrix.RowCount);
        }
    }
}
