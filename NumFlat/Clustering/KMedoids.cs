using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.Clustering
{
    public sealed class KMedoids<T>
    {
        public static int[] GetInitialGuessBuild(IReadOnlyList<T> items, IDistance<T, T> distance, int clusterCount)
        {
            // (TD, m1) <- (oo, null);
            var td = double.PositiveInfinity;
            List<int> m = [-1];

            // choose the first medoid
            // foreach xc do
            foreach (var xc in EnumerateItems(items))
            {
                // TDj <- 0;
                var tdj = 0.0;

                // foreach xo != xc do TDj <- TDj + d(xo, xc);
                foreach (var xo in EnumerateItemsExcept(items, xc.Index))
                {
                    tdj += distance.GetDistance(xo.Item, xc.Item);
                }

                // smallest distance sum
                // if TDj < TD then
                if (tdj < td)
                {
                    // (TD, m1) <- (TDj, xc)
                    td = tdj;
                    m[0] = xc.Index;
                }
            }

            // initialize distance to nearest medoid
            // foreach xo != m1 do dnearest(o) <- d(m1, xo);
            var dNearest = new double[items.Count];
            var m0 = m[0];
            foreach (var xo in EnumerateItemsExcept(items, m0))
            {
                dNearest[xo.Index] = distance.GetDistance(items[m0], xo.Item);
            }

            // choose remaining medoids
            // for i = 1...k-1 do
            for (var i = 1; i < clusterCount; i++)
            {
                // (dTD*, x*) <- (oo, null);
                var dTdBest = double.PositiveInfinity;
                var xBest = -1;

                // foreach xc != {m1, ..., mi}
            }
        }

        private static IEnumerable<(int Index, T Item)> EnumerateItems(IEnumerable<T> items)
        {
            var count = 0;
            foreach (var item in items)
            {
                yield return (count, item);
                count++;
            }
        }

        private static IEnumerable<(int Index, T Item)> EnumerateItemsExcept(IEnumerable<T> items, int excludeIndex)
        {
            foreach (var tuple in EnumerateItems(items))
            {
                if (tuple.Index != excludeIndex)
                {
                    yield return tuple;
                }
            }
        }

        private static IEnumerable<(int Index, T Item)> EnumerateItemsExcept(IEnumerable<T> items, IReadOnlyList<int> excludeIndices)
        {
            foreach (var tuple in EnumerateItems(items))
            {
                if (excludeIndices.All(index => index != tuple.Index))
                {
                    yield return tuple;
                }
            }
        }
    }
}
