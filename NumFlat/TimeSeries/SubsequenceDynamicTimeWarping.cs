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
        private int n;
        private int m;
        private Mat<double> costMatrix;

        public SubsequenceDynamicTimeWarping(IReadOnlyList<T> query, IReadOnlyList<U> longSequence, DistanceMetric<T, U> dm)
        {
            n = query.Count;
            m = longSequence.Count;
            costMatrix = GetCostMatrix(query, longSequence, dm);
        }

        public IndexPair[] GetAlignment(int lastIndex)
        {
            var fdtw = costMatrix.GetUnsafeFastIndexer();

            var alignment = new List<IndexPair>();
            var x = n - 1;
            var y = lastIndex;

            while (x > 0 && y > 0)
            {
                alignment.Add(new IndexPair(x, y));
                var min = Min(fdtw[x - 1, y], fdtw[x, y - 1], fdtw[x - 1, y - 1]);
                if (min == fdtw[x - 1, y - 1])
                {
                    x--;
                    y--;
                }
                else if (min == fdtw[x - 1, y])
                {
                    x--;
                }
                else
                {
                    y--;
                }
            }
            alignment.Add(new IndexPair(x, y));
            alignment.Reverse();

            return alignment.ToArray();
        }

        private static Mat<double> GetCostMatrix(IReadOnlyList<T> query, IReadOnlyList<U> longSequence, DistanceMetric<T, U> dm)
        {
            var dtw = new Mat<double>(query.Count + 1, longSequence.Count + 1);
            dtw[1.., ..].Fill(double.PositiveInfinity);

            var fdtw = dtw.GetUnsafeFastIndexer();
            for (var i = 1; i <= query.Count; i++)
            {
                for (var j = 1; j <= longSequence.Count; j++)
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
