using System;
using System.Collections.Generic;

namespace NumFlat.TimeSeries
{
    /// <summary>
    /// Provides DTW (dynamic time warping), a sequence-to-sequence matching algorithm.
    /// </summary>
    public static class DynamicTimeWarping
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements in the first sequence.
        /// </typeparam>
        /// <typeparam name="U">
        /// The type of the elements in the second sequence.
        /// </typeparam>
        /// <param name="xs">
        /// The first input sequence.
        /// </param>
        /// <param name="ys">
        /// The second input sequence.
        /// </param>
        /// <param name="distance">
        /// An instance of <see cref="IDistance{T, U}"/> to compute the distance between two elements.
        /// </param>
        /// <param name="w">
        /// The window size constraint that limits how far the algorithm can look ahead or behind.
        /// By default, there is no constraint.
        /// </param>
        /// <returns>
        /// The distance between the input sequences.
        /// </returns>
        public static double GetDistance<T, U>(IReadOnlyList<T> xs, IReadOnlyList<U> ys, IDistance<T, U> distance, int w = int.MaxValue)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(ys, nameof(ys));
            ThrowHelper.ThrowIfNull(distance, nameof(distance));

            if (xs.Count == 0)
            {
                throw new ArgumentException("The sequence must not be empty.", nameof(xs));
            }

            if (ys.Count == 0)
            {
                throw new ArgumentException("The sequence must not be empty.", nameof(ys));
            }

            var n = xs.Count;
            var m = ys.Count;

            // adapt window size
            w = Math.Max(w, Math.Abs(n - m));
            w = Math.Min(w, n + m);

            var dtw = new double[n + 1, m + 1];
            for (var i = 0; i <= n; i++)
            {
                for (var j = 0; j <= m; j++)
                {
                    dtw[i, j] = double.PositiveInfinity;
                }
            }
            dtw[0, 0] = 0;

            for (var i = 1; i <= n; i++)
            {
                var start = Math.Max(1, i - w);
                var end = Math.Min(m, i + w);
                for (var j = start; j <= end; j++)
                {
                    var cost = distance.GetDistance(xs[i - 1], ys[j - 1]);
                    dtw[i, j] = cost + Min(
                        dtw[i - 1, j],      // insertion
                        dtw[i, j - 1],      // deletion
                        dtw[i - 1, j - 1]); // match
                }
            }

            return dtw[n, m];
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
