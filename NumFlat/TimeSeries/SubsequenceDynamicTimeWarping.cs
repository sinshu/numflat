using System;
using System.Collections.Generic;

namespace NumFlat.TimeSeries
{
    /// <summary>
    /// Provides sDTW (subsequence dynamic time warping),
    /// a sequence alignment algorithm that finds the best matching subsequence within a longer sequence.
    /// </summary>
    public static class SubsequenceDynamicTimeWarping
    {
        /// <summary>
        /// Computes the cost matrix for sDTW (subsequence dynamic time warping),
        /// representing the pairwise distances between elements of a query sequence
        /// and a longer target sequence.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the query sequence.
        /// </typeparam>
        /// <typeparam name="U">
        /// The type of elements in the long target sequence.
        /// </typeparam>
        /// <param name="query">
        /// The query sequence to be matched.
        /// </param>
        /// <param name="longSequence">
        /// The longer sequence in which to search for a best-matching subsequence.
        /// </param>
        /// <param name="dm">
        /// The distance metric function to compute the local distance between two elements.
        /// </param>
        /// <returns>
        /// A matrix of costs, where each entry <c>[i, j]</c> represents the distance between
        /// <c>query[i]</c> and <c>longSequence[j]</c>.
        /// </returns>
        public static Mat<double> GetCostMatrix<T, U>(IReadOnlyList<T> query, IReadOnlyList<U> longSequence, DistanceMetric<T, U> dm)
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
