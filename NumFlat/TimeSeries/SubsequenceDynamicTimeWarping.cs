using System;
using System.Collections.Generic;

namespace NumFlat.TimeSeries
{
    /// <summary>
    /// Provides DTW (dynamic time warping), a sequence-to-sequence matching algorithm.
    /// </summary>
    public static class SubsequenceDynamicTimeWarping
    {
        public static Mat<double> ComputeCostMatrix<T, U>(IReadOnlyList<T> xs, IReadOnlyList<U> ys, DistanceMetric<T, U> dm)
        {
            var n = xs.Count;
            var m = ys.Count;

            var dtw = new Mat<double>(n + 1, m + 1);
            dtw[1.., ..].Fill(double.PositiveInfinity);

            var fdtw = dtw.GetUnsafeFastIndexer();
            for (var i = 1; i <= n; i++)
            {
                for (var j = 1; j <= m; j++)
                {
                    var cost = dm(xs[i - 1], ys[j - 1]);
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



        /// <summary>
        /// Represents a pair of indices.
        /// </summary>
        public struct IndexPair
        {
            private int first;
            private int second;

            internal IndexPair(int first, int second)
            {
                this.first = first;
                this.second = second;
            }

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            public override string ToString()
            {
                return "(" + first + ", " + second + ")";
            }

            /// <summary>
            /// The first index.
            /// </summary>
            public int First => first;

            /// <summary>
            /// The second index.
            /// </summary>
            public int Second => second;
        }
    }
}
