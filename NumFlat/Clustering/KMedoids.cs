using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.Clustering
{
    public sealed class KMedoids<T>
    {
        // This implementation is based on the following thesis:
        // https://www.sciencedirect.com/science/article/pii/S0306437921000557

        public static (int[] Medoids, double Error) GetInitialGuessBuild(IReadOnlyList<T> items, IDistance<T, T> distance, int clusterCount)
        {
            // (TD, m1) <- (oo, null);
            var td = double.PositiveInfinity;
            var m0 = -1;

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
                    m0 = xc.Index;
                }
            }

            // initialize distance to nearest medoid
            // foreach xo != m1 do dnearest(o) <- d(m1, xo);
            var dNearest = new double[items.Count];
            foreach (var xo in EnumerateItemsExcept(items, m0))
            {
                dNearest[xo.Index] = distance.GetDistance(items[m0], xo.Item);
            }

            // choose remaining medoids
            // for i = 1...k-1 do
            List<int> ms = [m0];
            for (var i = 0; i < clusterCount - 1; i++)
            {
                // (dTD*, x*) <- (oo, null);
                var dTdBest = double.PositiveInfinity;
                var xBest = -1;

                // foreach xc != { m1, ..., mi } do
                foreach (var xc in EnumerateItemsExcept(items, ms))
                {
                    // dTD <- 0;
                    var dTd = 0.0;

                    // foreach xo != { m1, ..., mi, xc } do
                    foreach (var xo in EnumerateItemsExcept(items, ms, xc.Index))
                    {
                        // reduction in TD
                        // delta <- d(xo, xc) - dnearest(o);
                        var delta = distance.GetDistance(xo.Item, xc.Item) - dNearest[xo.Index];

                        // if delta < 0 then dTD <- dTD + delta;
                        if (delta < 0.0)
                        {
                            dTd += delta;
                        }
                    }

                    // best reduction
                    // if dTD < dTD* then
                    if (dTd < dTdBest)
                    {
                        // (dTD*, x*) <- (dTD, xc)
                        dTdBest = dTd;
                        xBest = xc.Index;
                    }
                }

                // (TD, mi+1) <- (TD + dTD*, x*);
                td += dTdBest;
                ms.Add(xBest);

                // update distances to nearest medoid
                // foreach xo != { m1, ..., mi+1, } do
                var mLast = ms.Last();
                foreach (var xo in EnumerateItemsExcept(items, ms))
                {
                    // dnearest(o) <- min { dnearest(o), d(xo, mi+1) };
                    var tmp = dNearest[xo.Index];
                    dNearest[xo.Index] = Math.Min(tmp, distance.GetDistance(xo.Item, items[mLast]));
                }
            }

            // return TD, { m1, ..., mk };
            return (ms.ToArray(), td);
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

        private static IEnumerable<(int Index, T Item)> EnumerateItemsExcept(IEnumerable<T> items, IReadOnlyList<int> excludeIndices, int anotherExcludeIndex)
        {
            foreach (var tuple in EnumerateItemsExcept(items, excludeIndices))
            {
                if (tuple.Index != anotherExcludeIndex)
                {
                    yield return tuple;
                }
            }
        }
    }
}
