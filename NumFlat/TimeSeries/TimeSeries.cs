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
        public static SubsequenceDynamicTimeWarping<T, U> Sdtw<T, U>(IReadOnlyList<T> query, IReadOnlyList<U> longSequence, DistanceMetric<T, U> dm)
        {
            return new SubsequenceDynamicTimeWarping<T, U>(query, longSequence, dm);
        }
    }
}
