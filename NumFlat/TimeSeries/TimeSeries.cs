using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NumFlat.TimeSeries
{
    /// <summary>
    /// Provides utility methods for time-series data.
    /// </summary>
    public static partial class TimeSeries
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
        /// <param name="dm">
        /// An instance of <see cref="DistanceMetric{T, U}"/> to compute the distance between two elements.
        /// </param>
        /// <returns>
        /// The DTW distance between the input sequences.
        /// </returns>
        /// <remarks>
        /// For additional options, see the <see cref="DynamicTimeWarping"/> class.
        /// </remarks>
        public static double Dtw<T, U>(IReadOnlyList<T> xs, IReadOnlyList<U> ys, DistanceMetric<T, U> dm)
        {
            return DynamicTimeWarping.GetDistance(xs, ys, dm, int.MaxValue);
        }

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
        /// An instance of <see cref="DistanceMetric{T, U}"/> to compute the distance between two elements.
        /// </param>
        /// <returns>
        /// A <see cref="SubsequenceDynamicTimeWarping{T, U}"/> instance that holds the cost matrix and alignment methods.
        /// </returns>
        /// <remarks>
        /// For additional options, see the <see cref="SubsequenceDynamicTimeWarping"/> class.
        /// </remarks>
        public static SubsequenceDynamicTimeWarping<T, U> Sdtw<T, U>(IReadOnlyList<T> query, IReadOnlyList<U> longSequence, DistanceMetric<T, U> dm)
        {
            return new SubsequenceDynamicTimeWarping<T, U>(query, longSequence, dm);
        }

        /// <summary>
        /// Computes a barycenter sequence from the source sequences using DTW barycenter averaging.
        /// </summary>
        /// <param name="sourceSequences">
        /// The source sequences. Each element in every sequence must be a non-empty vector of the same dimension.
        /// </param>
        /// <returns>
        /// The computed barycenter sequence.
        /// </returns>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the barycenter.
        /// </exception>
        /// <remarks>
        /// For additional options, see the <see cref="DtwBarycenterAveraging"/> class.
        /// </remarks>
        public static IReadOnlyList<Vec<double>> DtwBarycenterAverage(IReadOnlyList<IReadOnlyList<Vec<double>>> sourceSequences)
        {
            return DtwBarycenterAveraging.Fit(sourceSequences);
        }
    }
}
