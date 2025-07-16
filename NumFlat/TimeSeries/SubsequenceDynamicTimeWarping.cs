using EmdFlat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.TimeSeries
{
    /// <summary>
    /// Provides sDTW (subsequence dynamic time warping),
    /// a sequence alignment algorithm that finds the best matching subsequence within a longer sequence.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the query sequence.
    /// </typeparam>
    /// <typeparam name="U">
    /// The type of elements in the long sequence.
    /// </typeparam>
    public class SubsequenceDynamicTimeWarping<T, U>
    {
        private Mat<double> costMatrix;

        public SubsequenceDynamicTimeWarping(IReadOnlyList<T> query, IReadOnlyList<U> longSequence, DistanceMetric<T, U> dm)
        {
            costMatrix = GetCostMatrix(query, longSequence, dm);
        }

        private static Mat<double> GetCostMatrix(IReadOnlyList<T> query, IReadOnlyList<U> longSequence, DistanceMetric<T, U> dm)
        {
            var n = query.Count;
            var m = longSequence.Count;

            var dtw = new Mat<double>(n + 1, m + 1);
            dtw[1.., ..].Fill(double.PositiveInfinity);

            var fdtw = dtw.GetUnsafeFastIndexer();
            for (var i = 1; i <= n; i++)
            {
                for (var j = 1; j <= m; j++)
                {
                    var cost = dm(query[i - 1], longSequence[j - 1]);
                    fdtw[i, j] = cost + Min(
                        fdtw[i - 1, j],      // insertion
                        fdtw[i, j - 1],      // deletion
                        fdtw[i - 1, j - 1]); // match
                }
            }

            return dtw[1.., 1..];
        }

        private static double Min(double a, double b, double c)
        {
            var min = (a < b) ? a : b;
            return (min < c) ? min : c;
        }

        public ref readonly Mat<double> CostMatrix => ref costMatrix;
    }
}
