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
        /// Applies the DTW (dynamic time warping) matching algorithm to the given two sequences.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the first sequence.
        /// </typeparam>
        /// <typeparam name="U">
        /// The type of elements in the second sequence.
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
        /// The DTW distance between the input sequences.
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

            return Compute(xs, ys, distance, w, false).Distance;
        }

        /// <summary>
        /// Applies the DTW (dynamic time warping) matching algorithm to the given two sequences.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the first sequence.
        /// </typeparam>
        /// <typeparam name="U">
        /// The type of elements in the second sequence.
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
        /// A tuple containing the DTW distance and the alignment path between the input sequences.
        /// </returns>
        public static (double Distance, IndexPair[] Alignment) GetDistanceAndAlignment<T, U>(IReadOnlyList<T> xs, IReadOnlyList<U> ys, IDistance<T, U> distance, int w = int.MaxValue)
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

            var result = Compute(xs, ys, distance, w, true);
            return (result.Distance, result.Alignment!);
        }

        private static (double Distance, IndexPair[]? Alignment) Compute<T, U>(IReadOnlyList<T> xs, IReadOnlyList<U> ys, IDistance<T, U> distance, int w, bool computeAlignment)
        {
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

            if (computeAlignment)
            {
                var alignment = new List<IndexPair>();
                var x = n;
                var y = m;

                while (x > 0 || y > 0)
                {
                    alignment.Add(new IndexPair(x - 1, y - 1));

                    if (x == 0)
                    {
                        y--;
                    }
                    else if (y == 0)
                    {
                        x--;
                    }
                    else
                    {
                        var min = Min(dtw[x - 1, y], dtw[x, y - 1], dtw[x - 1, y - 1]);
                        if (min == dtw[x - 1, y - 1])
                        {
                            x--;
                            y--;
                        }
                        else if (min == dtw[x - 1, y])
                        {
                            x--;
                        }
                        else
                        {
                            y--;
                        }
                    }
                }

                alignment.Reverse();

                return (dtw[n, m], alignment.ToArray());
            }

            return (dtw[n, m], null);
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
