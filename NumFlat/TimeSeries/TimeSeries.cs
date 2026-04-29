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

        /// <summary>
        /// Finds local peaks in a vector while enforcing a minimum distance between adjacent detected peaks.
        /// </summary>
        /// <param name="x">
        /// The input vector.
        /// </param>
        /// <param name="distance">
        /// Required minimal horizontal distance (in samples) between neighbouring peaks.
        /// Must be greater than or equal to 1.
        /// </param>
        /// <returns>
        /// An array of index-value pairs representing detected peaks.
        /// </returns>
        public static IndexValuePair<double>[] FindPeaksWithDistanceConstraint(in this Vec<double> x, int distance)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            if (distance < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(distance), "distance must be greater than or equal to 1.");
            }

            var peaks = x.FindPeaks();
            if (peaks.Length <= 1 || distance == 1)
            {
                return peaks;
            }

            // Compatible with scipy.signal.find_peaks(distance=...):
            // remove lower-priority peaks first, where priority is peak height.
            var order = Enumerable.Range(0, peaks.Length)
                .OrderByDescending(i => peaks[i].Value)
                .ThenByDescending(i => peaks[i].Index)
                .ToArray();

            var keep = Enumerable.Repeat(true, peaks.Length).ToArray();
            foreach (var i in order)
            {
                if (!keep[i])
                {
                    continue;
                }

                for (var j = i - 1; j >= 0; j--)
                {
                    if (peaks[i].Index - peaks[j].Index >= distance)
                    {
                        break;
                    }

                    keep[j] = false;
                }

                for (var j = i + 1; j < peaks.Length; j++)
                {
                    if (peaks[j].Index - peaks[i].Index >= distance)
                    {
                        break;
                    }

                    keep[j] = false;
                }
            }

            return peaks.Where((peak, i) => keep[i]).ToArray();
        }
    }
}
