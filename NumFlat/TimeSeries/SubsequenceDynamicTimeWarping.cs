using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.TimeSeries
{
    /// <summary>
    /// Implements sDTW (subsequence dynamic time warping),
    /// an algorithm to find the best matching subsequence within a longer sequence.
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

        /// <summary>
        /// Computes the sDTW cost matrix from the given query and long sequence.
        /// </summary>
        /// <param name="query">
        /// The query sequence.
        /// </param>
        /// <param name="longSequence">
        /// The long sequence.
        /// </param>
        /// <param name="dm">
        /// The DTW distance between the input sequences.
        /// </param>
        public SubsequenceDynamicTimeWarping(IReadOnlyList<T> query, IReadOnlyList<U> longSequence, DistanceMetric<T, U> dm)
        {
            ThrowHelper.ThrowIfNull(query, nameof(query));
            ThrowHelper.ThrowIfNull(longSequence, nameof(longSequence));
            ThrowHelper.ThrowIfNull(dm, nameof(dm));

            if (query.Count == 0)
            {
                throw new ArgumentException("The sequence must not be empty.", nameof(query));
            }

            if (longSequence.Count == 0)
            {
                throw new ArgumentException("The sequence must not be empty.", nameof(longSequence));
            }

            n = query.Count;
            m = longSequence.Count;
            costMatrix = GetCostMatrix(query, longSequence, dm);
        }

        /// <summary>
        /// Gets the alignment path,
        /// assuming that the last element of the query aligns to <c>lastIndex</c> in the long sequence.
        /// </summary>
        /// <param name="lastIndex">
        /// The position of the last element of the query in the long sequence.
        /// </param>
        /// <returns>
        /// The alignment path between the query and long sequence.
        /// </returns>
        /// <remarks>
        /// Note that <c>lastIndex</c> refers to the actual position of the last element, not one past it.
        /// </remarks>
        public IndexPair[] GetAlignment(int lastIndex)
        {
            if (lastIndex < 0 || lastIndex >= m)
            {
                throw new ArgumentOutOfRangeException(nameof(lastIndex), "The index must be within the valid range of the long sequence.");
            }

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

        /// <summary>
        /// Gets the cost matrix.
        /// </summary>
        public ref readonly Mat<double> CostMatrix => ref costMatrix;
    }



    /// <summary>
    /// Implements sDTW (subsequence dynamic time warping),
    /// an algorithm to find the best matching subsequence within a longer sequence.
    /// </summary>
    public static class SubsequenceDynamicTimeWarping
    {
        /// <summary>
        /// Computes the sDTW cost matrix from the given query and long sequence.
        /// </summary>
        /// <param name="query">
        /// The query sequence.
        /// </param>
        /// <param name="longSequence">
        /// The long sequence.
        /// </param>
        /// <param name="dm">
        /// The DTW distance between the input sequences.
        /// </param>
        /// <returns>
        /// A <see cref="SubsequenceDynamicTimeWarping{T, U}"/> instance that holds the cost matrix and alignment methods.
        /// </returns>
        public static SubsequenceDynamicTimeWarping<T, U> Compute<T, U>(IReadOnlyList<T> query, IReadOnlyList<U> longSequence, DistanceMetric<T, U> dm)
        {
            return new SubsequenceDynamicTimeWarping<T, U>(query, longSequence, dm);
        }

        /// <summary>
        /// Computes the best alignment path of the query sequence within the long sequence using sDTW (subsequence dynamic time warping).
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the query sequence.
        /// </typeparam>
        /// <typeparam name="U">
        /// The type of elements in the long sequence.
        /// </typeparam>
        /// <param name="query">
        /// The query sequence to be matched.
        /// </param>
        /// <param name="longSequence">
        /// The long sequence in which to search for the best matching subsequence.
        /// </param>
        /// <param name="dm">
        /// The distance metric used to compute the cost between elements.
        /// </param>
        /// <returns>
        /// An array of <see cref="IndexPair"/> representing the alignment path of the best matching subsequence.
        /// </returns>
        public static IndexPair[] GetBestAlignment<T, U>(IReadOnlyList<T> query, IReadOnlyList<U> longSequence, DistanceMetric<T, U> dm)
        {
            var sdtw = new SubsequenceDynamicTimeWarping<T, U>(query, longSequence, dm);

            var i = 0;
            var bestIndex = -1;
            var minValue = double.MaxValue;
            foreach (var value in sdtw.CostMatrix.Rows.Last())
            {
                if (value < minValue)
                {
                    bestIndex = i;
                    minValue = value;
                }
                i++;
            }

            return sdtw.GetAlignment(bestIndex);
        }
    }
}
