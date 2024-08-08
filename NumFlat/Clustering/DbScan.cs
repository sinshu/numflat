using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.Clustering
{
    /// <summary>
    /// Provides the DBSCAN clustering algorithm.
    /// </summary>
    public static class DbScan
    {
        public static void Fit<T>(IReadOnlyList<T> samples, IDistance<T, T> distance, double eps, int minSamples, Span<int> destination)
        {
        }
    }
}
