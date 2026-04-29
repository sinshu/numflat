using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NumFlat.TimeSeries
{
    /// <summary>
    /// Provides utility methods for time-series data.
    /// </summary>
    public static class TimeSeries
    {
        /// <summary>
        /// Finds local peaks in a vector.
        /// </summary>
        /// <param name="x">
        /// The input vector.
        /// </param>
        /// <returns>
        /// An array of index-value pairs representing detected peaks.
        /// </returns>
        public static IndexValuePair<double>[] FindPeaks(in this Vec<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            if (x.Count < 3)
            {
                return Array.Empty<IndexValuePair<double>>();
            }

            var fx = x.GetUnsafeFastIndexer();

            var peaks = new List<IndexValuePair<double>>();

            var i = 1;
            var iMax = x.Count - 1;

            while (i < iMax)
            {
                if (fx[i - 1] < fx[i])
                {
                    var iAhead = i + 1;
                    while (iAhead < iMax && fx[iAhead] == fx[i])
                    {
                        iAhead++;
                    }

                    if (fx[iAhead] < fx[i])
                    {
                        var left = i;
                        var right = iAhead - 1;
                        var center = (left + right) / 2;
                        peaks.Add(new IndexValuePair<double>(center, fx[center]));

                        i = iAhead;
                    }
                }

                i++;
            }

            return peaks.ToArray();
        }
    }
}
